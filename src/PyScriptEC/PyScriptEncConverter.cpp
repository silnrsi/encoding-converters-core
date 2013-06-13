// PyScriptEncConverter.cpp
//
// Rewritten by Jim Kornelsen for Linux on Nov 15 2011.
//
// 05-Dec-11 JDK  Put into a namespace.
// 20-May-13 JDK  Linux requires strdup() instead of _strdup().
//
// sudo apt-get install python-dev
// g++ PyScriptEncConverter.cpp -o PyScriptEC.exe -I/usr/include/python2.7/ -lpython2.7

#include <time.h>
#include <sys/types.h>
#include <sys/stat.h>

#ifdef _DEBUG
#undef _DEBUG
#define _RESTORE_DEBUG
#endif
#include <Python.h>
#ifdef _RESTORE_DEBUG
#define _DEBUG
#endif

#include "CEncConverter.h"
#include "PyScriptEncConverter.h"

#ifdef VERBOSE_DEBUGGING && _WIN32
#include "Windows.h"		// for OutputDebugString
#include "WinBase.h"
#endif

// Uncomment the following line if you want verbose debugging output
//#define VERBOSE_DEBUGGING

#ifndef _MSC_VER
#define _strdup strdup
#endif
#define _CRT_SECURE_NO_WARNINGS

// Keep this in a namespace so that it doesn't get confused with functions that
// have the same name in other converters, for example Load().
namespace PyScriptEC
{
    bool initialized=false;  // true after Initialize() is called

    PyObject*   m_pFunc     = 0;
    PyObject*   m_pModule   = 0;
    PyObject*   m_pArgs     = 0;
    int         m_nArgCount = 0;

    time_t     m_timeLastModified   = 0;
    char *     m_strFileSpec        = NULL;
    char *     m_strScriptDir       = NULL;
    char *     m_strScriptFile      = NULL;
    char *     m_strScriptName      = NULL;
    char *     m_strFuncName        = NULL;

    PyStringDataType    m_eStringDataTypeIn;
    PyStringDataType    m_eStringDataTypeOut;

    int ErrorOccurred();    // defined later

    bool IsModuleLoaded() { return (m_pModule != 0); };

	// this version is for where we use sprintf to print it to the sprintfBuffer, so just pass *that* buffer to the debug function
	void DebugOutput(const char* str)
	{
#ifdef VERBOSE_DEBUGGING
#ifdef _WIN32
		OutputDebugStringA(str);
#else
		fprintf(stderr, str);
#endif
#endif
	}

	char sprintfBuffer[1000];	// buffer used below
	void DebugOutput(int dontcare)
	{
		DebugOutput((const char*)sprintfBuffer);
	}

    void ResetPython()
    {
        DebugOutput("ResetPython() BEGIN\n");

		if( m_pArgs != 0 )
        {
            Py_DecRef(m_pArgs);
            m_pArgs = 0;
        }

        if( IsModuleLoaded() )
        {
            // this means we *were* doing something, so release everything (not the 
            //  func itself, but the module and then Finalize)
            DebugOutput("releasing what we were doing...\n");

			Py_DecRef(m_pModule);
            m_pModule = 0;

            // reset the function pointer as well (just good practice)
            m_pFunc = 0;

            if( PyErr_Occurred() )
                PyErr_Clear();  // without this, the Finalize normally throws a fatal exception.
            
            Py_Finalize();
        }
        DebugOutput("ResetPython() END\n");

	}

#ifdef _MSC_VER
    // Copy the first len characters of src into a dynamically allocated string.
    char * strndup(const char * src, size_t len)
    {
        if (src == NULL)
            return NULL;
        if (len < 0)
            len = 0;
        else if (len > strlen(src))
            len = strlen(src);
        char * dst = (char *)malloc(len + 1);
#if !be106
		// use the safe version
        strncpy_s(dst, len + 1, src, len);
#else
        strncpy(dst, src, len);
#endif
        dst[len] = 0;
        return dst;
    }
#endif

    int Load(void)
    {
        DebugOutput("PyScript.CppLoad() BEGIN\n");

		int hr = 0;

        struct stat attrib;
        DebugOutput(sprintf(sprintfBuffer, "Checking for file '%s'\n", m_strFileSpec));

		if(stat(m_strFileSpec, &attrib) != 0 || !S_ISREG(attrib.st_mode))
        {
            DebugOutput("PyScript: Invalid script path");

			return /* CantOpenReadMap = */ -11;
        }
        DebugOutput("Script file exists.\n");


        // see if the file has been changed (and reload if so)
        if (m_timeLastModified != 0)
        {
            if (attrib.st_mtime > m_timeLastModified)
            {
                // unload it and then we'll reload it later
                ResetPython();
            }
        }
        DebugOutput("Setting time last modified.\n");

		m_timeLastModified = attrib.st_mtime;

        // if we've already initialized Python, then we're done.
        if( IsModuleLoaded() )
            return hr;

        // put the rest of the arguments into an array for later processing
        char ** astrArgs = NULL; 
        int nAstrArgs = 0;

        // hook up to the Python DLL
        DebugOutput("Initializing python...\n");

		Py_Initialize();
        DebugOutput("Initialized.\n");


        // next add the path to the sys.path
        if (m_strScriptDir != NULL)
        {
            char strCmd[1000];
#ifdef MSC_VER
			_snprintf_s(strCmd, 1000, "import sys\nsys.path.append('%s')", m_strScriptDir);
#else
            snprintf(strCmd, 1000, "import sys\nsys.path.append('%s')", m_strScriptDir);
#endif
            strCmd[999] = 0;    // just in case...
            DebugOutput(sprintf(sprintfBuffer, "Running this python command:\n%s\n", strCmd));

			PyRun_SimpleString(strCmd);
            DebugOutput("Finished python command.\n");

		}

        // turn the filename into a Python object (Python import doesn't like .py extension)
        DebugOutput(sprintf(sprintfBuffer, "ScriptFile '%s'\n", m_strScriptFile));

		char * pszExtension = strrchr(m_strScriptFile, '.');
        if (!strcmp(pszExtension, ".py"))
            m_strScriptName = strndup(m_strScriptFile, strlen(m_strScriptFile) - 3);
        else
            m_strScriptName = _strdup(m_strScriptFile);

        // get the module point by the name
        DebugOutput(sprintf(sprintfBuffer, "ScriptName '%s'\n", m_strScriptName));

		m_pModule = PyImport_ImportModule(m_strScriptName);
        if( m_pModule == 0 )
        {
            // gracefully disconnect from Python
            // This calls PyErr_Clear() if needed, without which Py_Finalize()
            // throws a fatal exception.
            ErrorOccurred();
            Py_Finalize();

            DebugOutput(sprintf(sprintfBuffer, "PyScript: Unable to import script module '%s'! Is it locked? Does it have a syntax error? Is a Python distribution installed?", m_strScriptName));

			return /* CantOpenReadMap = */ -11;
        }

        PyObject* pDict = PyModule_GetDict(m_pModule);

        // if the user didn't give us a function name, then use the
        //  default function name, 'Convert'
        if(nAstrArgs > 1)
        {
            m_strFuncName = _strdup(astrArgs[1]);
        }
        else
        {
            m_strFuncName = _strdup("Convert");
        }
        m_pFunc = PyDict_GetItemString(pDict, m_strFuncName);
        
        if(!MyPyCallable_Check(m_pFunc))
        {
            // gracefully disconnect from Python
            DebugOutput(sprintf(sprintfBuffer, "PyScript: no callable function named '%s' in script module '%s'!",
											 m_strFuncName, m_strScriptName));

			ResetPython();
            return /* NameNotFound = */ -7;
        }

        // finally, if the user configured any additional parameters to be passed
        //  directly to the module, add those to the arguments we're going to pass.
        if(nAstrArgs > 2)
        {
            // make it size + 1 (for the data as the last value--filled in by
            //  PreConvert)
            m_nArgCount = nAstrArgs - 1;
            m_pArgs = PyTuple_New(m_nArgCount);
            for(int i = 2; i < nAstrArgs; i++)
            {
                char * strArg = astrArgs[i];
                PyObject* pValue = PyUnicode_FromUnicode (
                                   (const Py_UNICODE*)(const char *)strArg,strlen(strArg));
                if (pValue == 0) 
                {
                    // gracefully disconnect from Python
                    ResetPython();
                    DebugOutput(sprintf(sprintfBuffer, "PyScript: Can't convert optional fixed parameter '%s' to a Python unicode string", strArg));

					return -1;
                }

                // put it into the argument tuple (pValue reference is "stolen" here)
                PyTuple_SetItem(m_pArgs, i - 2, pValue);
            }
        }
        else
        {
            // we need at least one for the data value to be passed
            m_nArgCount = 1;
            m_pArgs = PyTuple_New(m_nArgCount);
        }

        DebugOutput(sprintf(sprintfBuffer, "m_nArgCount = %d", m_nArgCount));
        DebugOutput("Load() END\n");

		return hr;
    }

    int Initialize(char * strScript, char * strDir)
    {
        DebugOutput("PyScript.CppInitialize() BEGIN\n");
        DebugOutput(sprintf(sprintfBuffer, "strScript '%s'\n", strScript));
        DebugOutput(sprintf(sprintfBuffer, "strDir '%s'\n", strDir));

		if (initialized)
        {
            if (!strcmp(strDir, m_strScriptDir) && !strcmp(strScript, m_strScriptFile))
                return 0;
            ResetPython();
        }
        m_timeLastModified = time(NULL);
        m_eStringDataTypeIn = m_eStringDataTypeOut = eUCS2;		// assume the caller is giving us utf-16 (the converter only supports Unicode-to-Unicode converters

        if (m_strScriptFile != NULL && strcmp(strScript, m_strScriptFile))
        {
            free((void *)m_strScriptFile);
            m_strScriptFile = NULL;
        }
        if (m_strScriptFile == NULL)
            m_strScriptFile = _strdup(strScript);

        if (m_strScriptDir != NULL && strcmp(strDir, m_strScriptDir))
        {
            free((void *)m_strScriptDir);
            m_strScriptDir = NULL;
        }
        if (m_strScriptDir == NULL)
            m_strScriptDir  = _strdup(strDir);

        if (m_strFileSpec != NULL)
            free((void *)m_strFileSpec);
        size_t stringSize = sizeof(char) * (strlen(strDir) + strlen(strScript) + 2);
        m_strFileSpec = (char *)malloc(stringSize);
#ifdef _MSC_VER
        _snprintf_s(m_strFileSpec, stringSize, stringSize, "%s\\%s", strDir, strScript);
#else
        snprintf(m_strFileSpec, stringSize, "%s/%s", strDir, strScript);
#endif
        m_strFileSpec[stringSize - 1] = 0;

        // do the load at this point; not that we need it, but for checking that everything's okay.
        initialized = true;
        return Load();
    }

    // call to clean up resources when we've been inactive for some time.
    void InactivityWarning()
    {
        DebugOutput("CPyScriptEncConverter::InactivityWarning\n");

		ResetPython();
    }

    int PreConvert
    (
        int     eInEncodingForm,
        int&    eInFormEngine,
        int     eOutEncodingForm,
        int&    eOutFormEngine,
        int&    eNormalizeOutput,
        bool    bForward,
        int     nInactivityWarningTimeOut
    )
    {
        int hr = 0;
        if(hr == 0)
        {
            // I don't think Python does "bi-directional", so this code ignores the 
            //  the direction.
            switch(eInEncodingForm)
            {
                case EncodingForm_LegacyBytes:
                case EncodingForm_LegacyString:
                    eInFormEngine = EncodingForm_LegacyBytes;
                    m_eStringDataTypeIn = eBytes;
                    break;

                case EncodingForm_UTF8Bytes:
                case EncodingForm_UTF8String:
                case EncodingForm_UTF16BE:
                case EncodingForm_UTF16:
                    eInFormEngine = EncodingForm_UTF16;
                    m_eStringDataTypeIn = eUCS2;
                    break;

                case EncodingForm_UTF32BE:
                case EncodingForm_UTF32:
                    eInFormEngine = EncodingForm_UTF32;
                    m_eStringDataTypeIn = eUCS4;
                    break;
            };

            switch(eOutEncodingForm)
            {
                case EncodingForm_LegacyBytes:
                case EncodingForm_LegacyString:
                    eOutFormEngine = EncodingForm_LegacyBytes;
                    m_eStringDataTypeOut = eBytes;
                    break;

                case EncodingForm_UTF8Bytes:
                case EncodingForm_UTF8String:
                case EncodingForm_UTF16BE:
                case EncodingForm_UTF16:
                    eOutFormEngine = EncodingForm_UTF16;
                    m_eStringDataTypeOut = eUCS2;
                    break;

                case EncodingForm_UTF32BE:
                case EncodingForm_UTF32:
                    eOutFormEngine = EncodingForm_UTF32;
                    m_eStringDataTypeOut = eUCS4;
                    break;
            };

            // do the load at this point.
            hr = Load();
        }
        return hr;
    }

    //extern char * EnumerateDictionary(PyObject* pDict);

    int ErrorOccurred()
    {
        if( PyErr_Occurred() )
        {
#ifdef VERBOSE_DEBUGGING
            bool printedErr = false;
            DebugOutput(sprintf(sprintfBuffer, "While executing the function, '%s', in the python script, '%s', the following error occurred:\n",
											 m_strFuncName, m_strScriptName));
            PyObject *err_type, *err_value, *err_traceback;
            PyErr_Fetch(&err_type, &err_value, &err_traceback);
            
            if( MyPyClass_Check(err_type) )
            {
                PyObject *pName = ((PyClassObject*)err_type)->cl_name;
                if( pName != 0 )
                {
                    char * name = PyString_AsString(pName);
                    DebugOutput(sprintf(sprintfBuffer, "%s; ", name));
                }
                printedErr = true;
            }

            if( MyPyInstance_Check(err_value) )
            {
                DebugOutput("Instance error; ");
                printedErr = true;
            }
            else if( MyPyString_Check(err_value) )
            {
                char * value = PyString_AsString(err_value);
                DebugOutput(sprintf(sprintfBuffer, "%s; ", value));
                printedErr = true;
            }

            if( MyPyTraceBack_Check(err_traceback) )
            {
                PyTracebackObject* pTb = (PyTracebackObject*)err_traceback;
                // if( PyTraceBack_Print(err_traceback,pTb) )
                char * trace = PyString_AsString(err_traceback);
                DebugOutput(sprintf(sprintfBuffer, "%s; ", trace));
                printedErr = true;
            }
            if (!printedErr)
                DebugOutput("\n\n\tPyScript: No data return from Python! Perhaps there's a syntax error in the Python function\n");
#endif
            PyErr_Clear();
        }

        // reset python before we go
        ResetPython();

		// most likely a compilation failure
        return /* CompilationFailed = */ -9;
    }

    int DoConvert
    (
        char *  lpInBuffer,
        int     nInLen,
        char *  lpOutBuffer,
        int&    rnOutLen
    )
    {
        DebugOutput("PyScript.CppDoConvert() BEGIN\n");

		PyObject* pValue = 0;
        switch(m_eStringDataTypeIn)
        {
            case eBytes:
                pValue = PyString_FromStringAndSize((const char *)lpInBuffer,nInLen);
                break;
            case eUCS2:
                pValue = PyUnicode_FromUnicode((const Py_UNICODE*)(const char *)lpInBuffer, nInLen / 2);
                break;
    /*      // apparently, UTF32 isn't available concurrently with UTF16... so for now... comment it out
            case eUCS4:
                pValue = PyUnicodeUCS4_FromUnicode(lpInBuffer,nInLen / 4);
                break;
    */
        }

        if( pValue == 0 )
        {
            ResetPython();

			DebugOutput(sprintf(sprintfBuffer, "PyScript: Can't convert input data '%s' to a form that Python can read!?\n", 
											 lpInBuffer));

			return /* IncompleteChar = */ -8;
        }

        DebugOutput(sprintf(sprintfBuffer, "nInLen = %d, rnOutLen = %d\n", nInLen, rnOutLen));

		// put the value to convert into the last argument slot
        PyTuple_SetItem(m_pArgs, m_nArgCount - 1, pValue);

        // do the call
        DebugOutput("Calling...");

		pValue = PyObject_CallObject(m_pFunc, m_pArgs);
        DebugOutput("finished.\n");


        Py_ssize_t nOut;

        if( pValue == 0 )
        {
            return ErrorOccurred();
        }

        int hr = 0;
        void * lpOutValue = 0;
        switch(m_eStringDataTypeOut)
        {
            case eBytes:
                DebugOutput("getting eBytes of pValue\n");

				PyString_AsStringAndSize(pValue,(char **)&lpOutValue,&nOut);
                if( nOut < 0 )
                {
                    // at least, this is what happens if the python function returns a string, but the ConvType is
                    //  incorrectly configured as returning Unicode. ErrStatus
                    DebugOutput("PyScript: Are you sure that the Python function returns non-Unicode-encoded (bytes) data?");

					hr = /* OutEncFormNotSupported = */ -13;
                }
                break;
            case eUCS2:
                nOut = (int)PyUnicode_GetSize(pValue) * sizeof(char) * 2;	// EC is expecting the number of bytes
                if( nOut < 0 )
                {
                    // at least, this is what happens if the python function returns a string, but the ConvType is
                    //  incorrectly configured as returning Unicode. ErrStatus
                    DebugOutput("PyScript: Are you sure that the Python function returns Unicode-encoded (wide) data?");

					hr = /* OutEncFormNotSupported = */ -13;
                }
                lpOutValue = PyUnicode_AsUnicode(pValue);   // PyUnicodeUCS2_AsUnicode(pValue);
                break;
    /*
            case eUCS4:
                lpOutValue = PyUnicodeUCS4_AsUnicode(pValue);
                break;
    */
        }

        // don't let it be longer than out output buffer (I had a problem with the UnicodeName function
        //  and a *very long* clipboard sequence!). 
        if(hr == 0)
        {
            if( nOut > (int)rnOutLen )
            {
                DebugOutput(sprintf(sprintfBuffer, "Too long: '%s'\n", (char *)lpOutValue));

				hr = /* NotEnoughBuffer = */ -17;
            }
            else
            {
                rnOutLen = nOut;
                DebugOutput(sprintf(sprintfBuffer, "copying to length %d\n", (int)nOut));

				if( nOut > 0 )
                    memcpy(lpOutBuffer,lpOutValue,nOut);
                DebugOutput(sprintf(sprintfBuffer, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer)));

				lpOutBuffer[nOut] = '\0';   // end the string here
                    DebugOutput(sprintf(sprintfBuffer, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer)));

			}
        }

        Py_DecRef(pValue);

        return hr;
    }

    /*
    int main() {
        DebugOutput("main() BEGIN\n");
        char scriptDir[]  = "/media/winD/Jim/computing/SEC_on_linux/testing/";
        char scriptName[] = "testcnv.py";
        int err = Initialize(scriptName, scriptDir);
        if (err != 0) {
            return err;
        }

        char strIn[] = "abcde";
        int szOut    = 1000;
        char strOut[szOut];
        err = DoConvert(strIn, strlen(strIn), strOut, szOut);
        if (err != 0) {
            return err;
        }
        DebugOutput("Got %s\n", strOut);

        DebugOutput("main() END\n");
        return 0;
    }
    */
}

//******************************************************************************
// Global wrappers to call the functions inside the namespace.
//******************************************************************************

int PyScriptEC_Initialize(char * strScript, char * strDir)
{
    return PyScriptEC::Initialize(strScript, strDir);
}
int PyScriptEC_PreConvert (int eInEncodingForm, int& eInFormEngine,
    int eOutEncodingForm, int& eOutFormEngine,
    int& eNormalizeOutput, bool bForward,
    int nInactivityWarningTimeOut)
{
    return PyScriptEC::PreConvert (eInEncodingForm, eInFormEngine,
                        eOutEncodingForm, eOutFormEngine,
                        eNormalizeOutput, bForward,
                        nInactivityWarningTimeOut);
}
int PyScriptEC_DoConvert (char * lpInBuffer, int nInLen, char * lpOutBuffer,
    int& rnOutLen)
{
    return PyScriptEC::DoConvert (lpInBuffer, nInLen, lpOutBuffer, rnOutLen);
}
