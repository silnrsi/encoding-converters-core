// PyScriptEncConverter.cpp
//
// Rewritten by Jim Kornelsen for Linux on Nov 15 2011.
//
// 05-Dec-11 JDK  Put into a namespace.
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
    char *     m_strScriptName      = NULL;
    char *     m_strFuncName        = NULL;

    PyStringDataType    m_eStringDataTypeIn;
    PyStringDataType    m_eStringDataTypeOut;

    const char * clpszPyScriptDefFuncName = ("Convert");

    bool IsModuleLoaded() { return (m_pModule != 0); };
    void ResetPython()
    {
        fprintf(stderr, "ResetPython() BEGIN\n");
        //m_strScriptName = NULL;
        //m_strFuncName   = NULL;

        if( m_pArgs != 0 )
        {
            Py_DecRef(m_pArgs);
            m_pArgs = 0;
        }

        if( IsModuleLoaded() )
        {
            // this means we *were* doing something, so release everything (not the 
            //  func itself, but the module and then Finalize)
            fprintf(stderr, "releasing what we were doing...\n");
            Py_DecRef(m_pModule);
            m_pModule = 0;

            // reset the function pointer as well (just good practice)
            m_pFunc = 0;

            if( PyErr_Occurred() )
                PyErr_Clear();  // without this, the Finalize normally throws a fatal exception.
            
            Py_Finalize();
        }
        fprintf(stderr, "ResetPython() END\n");
    }

    int Load(void)
    {
        fprintf(stderr, "PyScript.CppLoad() BEGIN\n");
        int hr = 0;

        struct stat attrib;
        fprintf(stderr, "Checking for file '%s'\n", m_strFileSpec);
        if(stat(m_strFileSpec, &attrib) != 0 || !S_ISREG(attrib.st_mode)) {
            fprintf(stderr, "PyScript: Invalid script path");
            return -1;
        }
        fprintf(stderr, "Script file exists.\n");
            
        // see if the file has been changed (and reload if so)
        if (m_timeLastModified != 0)
        {
            if (attrib.st_mtime > m_timeLastModified)
            {
                // unload it and then we'll reload it later
                ResetPython();
            }
        }
        fprintf(stderr, "Setting time last modified.\n");
        m_timeLastModified = attrib.st_mtime;

        // if we've already initialized Python, then we're done.
        if( IsModuleLoaded() )
            return hr;

        // put the rest of the arguments into an array for later processing
        char * astrArgs[0]; 
        int nAstrArgs = 0;

        // hook up to the Python DLL
        fprintf(stderr, "Initializing python...\n");
        Py_Initialize();
        fprintf(stderr, "Initialized.\n");

        // next add the path to the sys.path
        if (m_strScriptDir != NULL)
        {
            char strCmd[1000];
            sprintf(strCmd, "import sys\nsys.path.append('%s')", m_strScriptDir);
            fprintf(stderr, "Running this python command:\n%s\n", strCmd);
            PyRun_SimpleString(strCmd);
            fprintf(stderr, "Finished python command.\n");
        }

        // turn the filename into a Python object (Python import doesn't like .py extension)
        fprintf(stderr, "ScriptName '%s'\n", m_strScriptName);
        if(!strcmp(m_strScriptName + strlen(m_strScriptName) - 3, ".py")) {
            m_strScriptName = strndup(m_strScriptName, strlen(m_strScriptName) - 3);
        }
        m_strScriptName = "testcnv";

        // get the module point by the name
        fprintf(stderr, "ScriptName '%s'\n", m_strScriptName);
        m_pModule = PyImport_ImportModule(m_strScriptName);
        if( m_pModule == 0 )
        {
            // gracefully disconnect from Python
            if( PyErr_Occurred() )
                PyErr_Clear();  // without this, the Finalize normally throws a fatal exception.
            Py_Finalize();

            fprintf(stderr, "PyScript: Unable to import script module '%s'! Is it locked? Does it have a syntax error? Is a Python distribution installed?", m_strScriptName);
            return -1;
        }

        PyObject* pDict = PyModule_GetDict(m_pModule);

        // if the user didn't give us a function name, then use the
        //  default function name, 'Convert'
        m_strFuncName = strdup(clpszPyScriptDefFuncName);
        if(nAstrArgs > 1)
            m_strFuncName = strdup(astrArgs[1]);

        m_pFunc = PyDict_GetItemString(pDict, m_strFuncName);
        
        if(!MyPyCallable_Check(m_pFunc))
        {
            // gracefully disconnect from Python
            fprintf(stderr, "PyScript: no callable function named '%s' in script module '%s'!",
                    m_strFuncName, m_strScriptName);
            ResetPython();
            return -1;
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
                    fprintf(stderr, "PyScript: Can't convert optional fixed parameter '%s' to a Python unicode string", strArg);
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
        fprintf(stderr, "Load() END\n");
        return hr;
    }

    int Initialize(char * strScript, char * strDir)
    {
        fprintf(stderr, "PyScript.CppInitialize() BEGIN\n");
        fprintf(stderr, "strScript '%s'\n", strScript);
        fprintf(stderr, "strDir '%s'\n", strDir);
        if (initialized) return 0;

        m_timeLastModified = time(NULL);
        m_eStringDataTypeIn = m_eStringDataTypeOut = eBytes;

        m_strScriptName = strdup(strScript);
        m_strScriptDir  = strdup(strDir);
        int stringSize = sizeof(char) * (strlen(strDir) + strlen(strScript) + 1);
        m_strFileSpec = (char *)malloc(stringSize);
        snprintf(m_strFileSpec, stringSize, "%s%s", strDir, strScript);

        // do the load at this point; not that we need it, but for checking that everything's okay.
        initialized = true;
        return Load();
    }

    // call to clean up resources when we've been inactive for some time.
    void InactivityWarning()
    {
        fprintf(stderr, "CPyScriptEncConverter::InactivityWarning\n");
        ResetPython();
    }

    int PreConvert
    (
        int     eInEncodingForm,
        int&	eInFormEngine,
        int     eOutEncodingForm,
        int&	eOutFormEngine,
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
            bool printedErr = false;
            fprintf(stderr, "While executing the function, '%s', in the python script, '%s', the following error occurred:",
                    m_strFuncName, m_strScriptName );
            PyObject *err_type, *err_value, *err_traceback;
            PyErr_Fetch(&err_type, &err_value, &err_traceback);
            
            if( MyPyClass_Check(err_type) )
            {
                PyObject *pName = ((PyClassObject*)err_type)->cl_name;
                if( pName != 0 )
                    fprintf(stderr, "%s; ", PyString_AsString(pName));
                printedErr = true;
            }

            if( MyPyInstance_Check(err_value) )
            {
                //fprintf(stderr, "%s; ", EnumerateDictionary(((PyInstanceObject*)err_value)->in_dict));
                fprintf(stderr, "Instance error; ");
                printedErr = true;
            }
            else if( MyPyString_Check(err_value) )
            {
                fprintf(stderr, "%s; ", PyString_AsString(err_value));
                printedErr = true;
            }

            if( MyPyTraceBack_Check(err_traceback) )
            {
                PyTracebackObject* pTb = (PyTracebackObject*)err_traceback;
                // if( PyTraceBack_Print(err_traceback,pTb) )
                fprintf(stderr, "%s; ", PyString_AsString(err_traceback));
                printedErr = true;
            }
            if (!printedErr)
                fprintf(stderr, "\n\n\tPyScript: No data return from Python! Perhaps there's a syntax error in the Python function");

            PyErr_Clear();

        }

        // reset python before we go
        ResetPython();

        return -1;
    }

    int DoConvert
    (
        char *  lpInBuffer,
        int     nInLen,
        char *  lpOutBuffer,
        int&    rnOutLen
    )
    {
        fprintf(stderr, "PyScript.CppDoConvert() BEGIN\n");
        PyObject* pValue = 0;
        switch(m_eStringDataTypeIn)
        {
            case eBytes:
                pValue = PyString_FromStringAndSize((const char *)lpInBuffer,nInLen);
                break;
            case eUCS2:
                pValue = PyUnicode_FromUnicode((const Py_UNICODE*)(const char *)lpInBuffer,nInLen / 2);
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
            fprintf(stderr, "PyScript: Can't convert input data '%s' to a form that Python can read!?", 
                    lpInBuffer);
            return -1;
        }
        
        // put the value to convert into the last argument slot
        PyTuple_SetItem(m_pArgs, m_nArgCount - 1, pValue);

        // do the call
        fprintf(stderr, "Calling...");
        pValue = PyObject_CallObject(m_pFunc, m_pArgs);
        fprintf(stderr, "finished.\n");

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
                fprintf(stderr, "getting eBytes of pValue\n");
                PyString_AsStringAndSize(pValue,(char **)&lpOutValue,&nOut);
                if( nOut < 0 )
                {
                    // at least, this is what happens if the python function returns a string, but the ConvType is
                    //  incorrectly configured as returning Unicode. ErrStatus
                    fprintf(stderr, "PyScript: Are you sure that the Python function returns non-Unicode-encoded (bytes) data?");
                    hr = -1;
                }
                break;
            case eUCS2:
                nOut = (int)PyUnicode_GetSize(pValue) * sizeof(char);
                if( nOut < 0 )
                {
                    // at least, this is what happens if the python function returns a string, but the ConvType is
                    //  incorrectly configured as returning Unicode. ErrStatus
                    fprintf(stderr, "PyScript: Are you sure that the Python function returns Unicode-encoded (wide) data?");
                    hr = -1;
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
                fprintf(stderr, "Too long: '%s'\n", (char *)lpOutValue);
                hr = -1;
            }
            else
            {
                rnOutLen = nOut;
                fprintf(stderr, "copying to length %d\n", (int)nOut);
                if( nOut > 0 )
                    memcpy(lpOutBuffer,lpOutValue,nOut);
				fprintf(stderr, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer));
                    lpOutBuffer[nOut] = '\0';   // end the string here
                    fprintf(stderr, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer));
                }
        }

        Py_DecRef(pValue);

        return hr;
    }

    /*
    int main() {
        fprintf(stderr, "main() BEGIN\n");
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
        fprintf(stderr, "Got %s\n", strOut);

        fprintf(stderr, "main() END\n");
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

