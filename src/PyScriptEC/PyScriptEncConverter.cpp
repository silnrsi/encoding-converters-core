// PyScriptEncConverter.cpp
//
// Rewritten by Jim Kornelsen for Linux on Nov 15 2011.
//
// 05-Dec-11 JDK  Put into a namespace.
// 20-May-13 JDK  Linux requires strdup() instead of _strdup().
// 29-Jun-13 JDK  Implement choice of unicode string or bytes.
//
// Example steps to compile:
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

// Uncomment the following line if you want verbose debugging output
//#define VERBOSE_DEBUGGING

#ifdef VERBOSE_DEBUGGING
#ifdef _WIN32
#include "Windows.h"        // for OutputDebugString
#include "WinBase.h"
#endif
#endif

#ifndef _WIN32
#define _strdup strdup
#endif

// Keep this in a namespace so that it doesn't get confused with functions that
// have the same name in other converters.
namespace PyScriptEC
{
    bool initialized=false;  // true after Initialize() is called

    PyObject * m_pFunc         = 0;
    PyObject * m_pModule       = 0;
    char *     m_strScriptFile = NULL;
    char *     m_strFuncName   = NULL;

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
        fprintf(stderr, "%s", str);
        //printf("%s", str);
#endif
#endif
    }

    char sprintfBuffer[1000];   // buffer used below
    void DebugOutput(int dontcare)
    {
        DebugOutput((const char*)sprintfBuffer);
    }

    void ResetPython()
    {
        DebugOutput("CppResetPython() BEGIN\n");
        if( IsModuleLoaded() )
        {
            // this means we *were* doing something, so release everything (not the 
            //  func itself, but the module and then Finalize)
            DebugOutput("CppResetPython: releasing what we were doing...\n");

            Py_DecRef(m_pModule);
            m_pModule = 0;

            // reset the function pointer as well (just good practice)
            m_pFunc = 0;

            if( PyErr_Occurred() )
                PyErr_Clear();  // without this, the Finalize normally throws a fatal exception.
            
            Py_Finalize();
        }
        DebugOutput("CppResetPython() END\n");

    }

    //**************************************************************************
    /**
     * Set default string types and load the script.
     */
    int Initialize(char * strScriptFile, char * strScriptDir)
    {
        DebugOutput("CppInitialize() BEGIN\n");
#ifdef _MSC_BUILD
		DebugOutput(sprintf_s(sprintfBuffer, "strScriptFile '%s'\n", strScriptFile));
		DebugOutput(sprintf_s(sprintfBuffer, "strScriptDir '%s'\n", strScriptDir));
#else
		DebugOutput(sprintf(sprintfBuffer, "strScriptFile '%s'\n", strScriptFile));
        DebugOutput(sprintf(sprintfBuffer, "strScriptDir '%s'\n", strScriptDir));
#endif
        if (initialized)
        {
            ResetPython();
        }

#ifdef _WIN32
        // On Windows the caller should typically give us utf-16.
        m_eStringDataTypeIn = m_eStringDataTypeOut = eUCS2;
#else
        // On most Linux builds Python expects 4-byte data.
        m_eStringDataTypeIn = m_eStringDataTypeOut = eUCS4;
#endif

        // do the load at this point
        initialized = true;
        DebugOutput("CppInitialize: Loading\n");

        int hr = 0;

        // if we've already initialized Python, then we're done.
        if( IsModuleLoaded() )
            return hr;

        // hook up to the Python DLL
        DebugOutput("CppInitialize: Initializing python...\n");
        Py_Initialize();
        DebugOutput("CppInitialize: Initialized.\n");

        // next add the path to the sys.path
        if (strScriptDir != NULL)
        {
            char strCmd[1000];
#ifdef _MSC_BUILD
            _snprintf_s(strCmd, 1000, "import sys\nsys.path.append(r'%s')", strScriptDir);
#else
            snprintf(strCmd, 1000, "import sys\nsys.path.append(r'%s')", strScriptDir);
#endif
            strCmd[999] = 0;    // just in case...

#ifdef _MSC_BUILD
			DebugOutput(sprintf_s(sprintfBuffer, "Running this python command:\n%s\n", strCmd));
#else
			DebugOutput(sprintf(sprintfBuffer, "Running this python command:\n%s\n", strCmd));
#endif

            PyRun_SimpleString(strCmd);
            DebugOutput("Finished python command.\n");

        }

        // turn the filename into a Python object name (Python import doesn't like .py extension)
#ifdef _MSC_BUILD
		DebugOutput(sprintf_s(sprintfBuffer, "ScriptFile '%s'\n", strScriptFile));
#else
		DebugOutput(sprintf(sprintfBuffer, "ScriptFile '%s'\n", strScriptFile));
#endif
        m_strScriptFile = _strdup(strScriptFile);   // save name for error messages
        char * strScriptName = _strdup(strScriptFile);
        char * pszExtension = strstr(strScriptName, ".py");
        if (pszExtension)
            pszExtension[0] = '\0';  // truncate strScriptName at position of pszExtension

        // get the module point by the name
#ifdef _MSC_BUILD
		DebugOutput(sprintf_s(sprintfBuffer, "ScriptName '%s'\n", strScriptName));
#else
		DebugOutput(sprintf(sprintfBuffer, "ScriptName '%s'\n", strScriptName));
#endif

        m_pModule = PyImport_ImportModule(strScriptName);
        if( m_pModule == 0 )
        {
            // gracefully disconnect from Python
            // This calls PyErr_Clear() if needed, without which Py_Finalize()
            // throws a fatal exception.
            ErrorOccurred();
            Py_Finalize();

#ifdef _MSC_BUILD
			DebugOutput(sprintf_s(sprintfBuffer, "PyScript: Unable to import script module '%s'! Is it locked? Does it have a syntax error? Is a Python distribution installed?", strScriptFile));
#else
			DebugOutput(sprintf(sprintfBuffer, "PyScript: Unable to import script module '%s'! Is it locked? Does it have a syntax error? Is a Python distribution installed?", strScriptFile));
#endif

            return /* CantOpenReadMap = */ -11;
        }

        PyObject* pDict = PyModule_GetDict(m_pModule);

        // use the default function name, 'Convert'
        m_strFuncName = _strdup("Convert");
        m_pFunc = PyDict_GetItemString(pDict, m_strFuncName);
        
        if(!MyPyCallable_Check(m_pFunc))
        {
            // gracefully disconnect from Python
#ifdef _MSC_BUILD
			DebugOutput(sprintf_s(sprintfBuffer, "PyScript: no callable function named '%s' in script module '%s'!", m_strFuncName, strScriptFile));
#else
			DebugOutput(sprintf(sprintfBuffer, "PyScript: no callable function named '%s' in script module '%s'!", m_strFuncName, strScriptFile));
#endif

            ResetPython();
            return /* NameNotFound = */ -7;
        }

        DebugOutput("CppInitialize() END\n");
        return hr;
    }

    // call to clean up resources when we've been inactive for some time.
    void InactivityWarning()
    {
        DebugOutput("CppInactivityWarning BEGIN\n");

        ResetPython();
    }

    int PreConvert
    (
        int  eInFormEngine,
        int  eOutFormEngine,
        int  eNormalizeOutput,
        bool bForward
    )
    {
        DebugOutput("CppPreConvert BEGIN\n");
        int hr = 0;
        if(hr == 0)
        {
            // the Python converter doesn't currently do "bi-directional", so this code ignores the 
            //  the direction.
            switch(eInFormEngine)
            {
                case EncodingForm_LegacyBytes:
                case EncodingForm_LegacyString:
                    DebugOutput("CppPreConvert: eInFormEngine legacy bytes\n");
                    m_eStringDataTypeIn = eBytes;
                    break;

                case EncodingForm_UTF8Bytes:
                    DebugOutput("CppPreConvert: eInFormEngine UTF-8\n");
                    m_eStringDataTypeIn = eUTF8Bytes;
                    break;

                case EncodingForm_UTF8String:
                case EncodingForm_UTF16BE:
                case EncodingForm_UTF16:
                    DebugOutput("CppPreConvert: eInFormEngine 1- or 2-byte unicode\n");
                    m_eStringDataTypeIn = eUCS2;
                    break;

                case EncodingForm_UTF32BE:
                case EncodingForm_UTF32:
                    DebugOutput("CppPreConvert: eInFormEngine 4-byte unicode\n");
                    m_eStringDataTypeIn = eUCS4;
                    break;
            };

            switch(eOutFormEngine)
            {
                case EncodingForm_LegacyBytes:
                case EncodingForm_LegacyString:
                    DebugOutput("CppPreConvert: eOutFormEngine legacy bytes\n");
                    m_eStringDataTypeOut = eBytes;
                    break;

                case EncodingForm_UTF8Bytes:
                    DebugOutput("CppPreConvert: eOutFormEngine UTF-8\n");
                    m_eStringDataTypeOut = eUTF8Bytes;
                    break;

                case EncodingForm_UTF8String:
                case EncodingForm_UTF16BE:
                case EncodingForm_UTF16:
                    DebugOutput("CppPreConvert: eOutFormEngine 1- or 2-byte unicode\n");
                    m_eStringDataTypeOut = eUCS2;
                    break;

                case EncodingForm_UTF32BE:
                case EncodingForm_UTF32:
                    DebugOutput("CppPreConvert: eOutFormEngine 4-byte unicode\n");
                    m_eStringDataTypeOut = eUCS4;
                    break;
            };
        }
        DebugOutput("CppPreConvert END\n");
        return hr;
    }

    int ErrorOccurred()
    {
        if( PyErr_Occurred() )
        {
#ifdef _MSC_BUILD
			DebugOutput(sprintf_s(sprintfBuffer, "Error occurred while executing %s() in %s.\n", m_strFuncName, m_strScriptFile));
#else
			DebugOutput(sprintf(sprintfBuffer, "Error occurred while executing %s() in %s.\n", m_strFuncName, m_strScriptFile));
#endif
                                               
            PyErr_Print();
            PyErr_Clear();

            // reset python before we go
            ResetPython();

            // most likely a compilation failure
            return -9; // CompilationFailed
        }
        return 0;
    }

    int DoConvert
    (
        char *  lpInBuffer,
        int     nInLen,
        char *  lpOutBuffer,
        int&    rnOutLen
    )
    {
        DebugOutput("CppDoConvert() BEGIN\n");
#ifdef _MSC_BUILD
		DebugOutput(sprintf_s(sprintfBuffer, "nInLen = %d, rnOutLen = %d\n", nInLen, rnOutLen));
        DebugOutput(sprintf_s(sprintfBuffer, "sizeof(Py_UNICODE) = %d\n", sizeof(Py_UNICODE)));
#else
		DebugOutput(sprintf(sprintfBuffer, "nInLen = %d, rnOutLen = %d\n", nInLen, rnOutLen));
        DebugOutput(sprintf(sprintfBuffer, "sizeof(Py_UNICODE) = %d\n", sizeof(Py_UNICODE)));
#endif

        PyObject* pValue = 0;
        switch(m_eStringDataTypeIn)
        {
            case eBytes:
                DebugOutput("CppDoConvert: eBytes input\n");
                pValue = PyString_FromStringAndSize((const char *)lpInBuffer, nInLen);
                break;
            case eUTF8Bytes:
                DebugOutput("CppDoConvert: eUTF8Bytes input\n");
                pValue = PyUnicode_FromStringAndSize((const char *)lpInBuffer, nInLen);
                break;
            case eUCS2:
                DebugOutput("CppDoConvert: eUCS2 input\n");
                pValue = PyUnicode_FromUnicode((const Py_UNICODE*)(const char *)lpInBuffer, nInLen / 2);
                break;
          // UTF32 isn't available concurrently with UTF16... so for now... disable it on Windows
#ifndef _WIN32
            case eUCS4:
                DebugOutput("CppDoConvert: eUCS4 input\n");
                int lenRoundedUp = (nInLen + (4 - 1)) / 4;  // don't chop off trailing values of less than 4 bytes
                pValue = PyUnicodeUCS4_FromUnicode((const Py_UNICODE*)(const char *)lpInBuffer, lenRoundedUp);
                break;
#endif
        }

        if( pValue == 0 )
        {
            ResetPython();

#ifdef _MSC_BUILD
			DebugOutput(sprintf_s(sprintfBuffer, "PyScript: Can't convert input data '%s' to a form that Python can read!?\n", lpInBuffer));
#else
			DebugOutput(sprintf(sprintfBuffer, "PyScript: Can't convert input data '%s' to a form that Python can read!?\n", lpInBuffer));
#endif

            return /* IncompleteChar = */ -8;
        }

        // put the value to convert into the last argument slot
        // we need at least one for the data value to be passed
        int nArgCount = 1;
        PyObject * pArgs = PyTuple_New(nArgCount);
        PyTuple_SetItem(pArgs, nArgCount - 1, pValue);

        // do the call
        DebugOutput("CppDoConvert: Calling...");
        pValue = PyObject_CallObject(m_pFunc, pArgs);
        Py_DecRef(pArgs);
        DebugOutput("finished.\n");

        if( pValue == 0 )
        {
            return ErrorOccurred();
        }

        Py_ssize_t nOut;
        void *     lpOutValue = 0;
        int        hr         = 0;
        int        err        = 0;
        switch(m_eStringDataTypeOut)
        {
            case eBytes:
                DebugOutput("CppDoConvert: eBytes output\n");
                PyString_AsStringAndSize(pValue,(char **)&lpOutValue,&nOut);
                if( nOut < 0 )
                {
                    // at least, this is what happens if the python function returns a string, but the ConvType is
                    //  incorrectly configured as returning Unicode. ErrStatus
                    DebugOutput("CppDoConvert: The Python function may have returned illegal bytes.\n");

                    hr = /* OutEncFormNotSupported = */ -13;
                }
                break;
            case eUTF8Bytes:
                DebugOutput("CppDoConvert: eUTF8Bytes output\n");
#ifdef _WIN32
                // This might be unnecessary, but the documentation suggests that PyString_AsStringAndSize
                // might output something like cp1252 on Windows instead of UTF-8.
                pValue = PyUnicode_AsUTF8String(pValue);
                if (pValue == NULL)
                {
                    DebugOutput("CppDoConvert: Are you sure that the Python function returns Unicode-encoded (wide) data?\n");
                    hr = /* OutEncFormNotSupported = */ -13;
                    break;
                }
#endif
                err = PyString_AsStringAndSize(pValue, (char **)&lpOutValue, &nOut);
                if ( nOut < 0 || err < 0)
                {
                    DebugOutput("CppDoConvert: The Python function may have returned some illegal bytes.\n");
                    hr = /* OutEncFormNotSupported = */ -13;
                }
                break;
            case eUCS2:
                DebugOutput("CppDoConvert: eUCS2 output\n");
                nOut = (int)PyUnicode_GetSize(pValue) * sizeof(char) * 2;   // EC is expecting the number of bytes
                if( nOut < 0 )
                {
                    // at least, this is what happens if the python function returns a string, but the ConvType is
                    //  incorrectly configured as returning Unicode. ErrStatus
                    DebugOutput("CppDoConvert: Are you sure that the Python function returns Unicode-encoded (wide) data?\n");

                    hr = /* OutEncFormNotSupported = */ -13;
                }
                lpOutValue = PyUnicode_AsUnicode(pValue);   // PyUnicodeUCS2_AsUnicode(pValue);
                break;
#ifndef _WIN32
            case eUCS4:
                DebugOutput("CppDoConvert: eUCS4 output\n");
                nOut = (int)PyUnicode_GetSize(pValue) * sizeof(char) * 4;   // EC is expecting the number of bytes
                if( nOut < 0 )
                {
                    // at least, this is what happens if the python function returns a string, but the ConvType is
                    //  incorrectly configured as returning Unicode. ErrStatus
                    DebugOutput("CppDoConvert: Are you sure that the Python function returns Unicode-encoded (wide) data?\n");

                    hr = /* OutEncFormNotSupported = */ -13;
                }
                lpOutValue = PyUnicodeUCS4_AsUnicode(pValue);
                break;
#endif
        }

        // don't let it be longer than out output buffer (I had a problem with the UnicodeName function
        //  and a *very long* clipboard sequence!). 
        if(hr == 0)
        {
            if( nOut > (int)rnOutLen )
            {
#ifdef _MSC_BUILD
				DebugOutput(sprintf_s(sprintfBuffer, "Too long: '%s'\n", (char *)lpOutValue));
#else
				DebugOutput(sprintf(sprintfBuffer, "Too long: '%s'\n", (char *)lpOutValue));
#endif

                hr = /* NotEnoughBuffer = */ -17;
            }
            else
            {
                rnOutLen = nOut;
#ifdef _MSC_BUILD
				DebugOutput(sprintf_s(sprintfBuffer, "copying to length %d\n", (int)nOut));
#else
				DebugOutput(sprintf(sprintfBuffer, "copying to length %d\n", (int)nOut));
#endif

                if( nOut > 0 )
                    memcpy(lpOutBuffer,lpOutValue,nOut);

#ifdef _MSC_BUILD
				DebugOutput(sprintf_s(sprintfBuffer, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer)));
#else
				DebugOutput(sprintf(sprintfBuffer, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer)));
#endif

                lpOutBuffer[nOut] = '\0';   // end the string here
#ifdef _MSC_BUILD
				DebugOutput(sprintf_s(sprintfBuffer, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer)));
#else
				DebugOutput(sprintf(sprintfBuffer, "length of outBuffer = %u\n", (unsigned)strlen(lpOutBuffer)));
#endif
            }
        }

        Py_DecRef(pValue);

        DebugOutput("CppDoConvert() END\n");
        return hr;
    }
}

//******************************************************************************
// Global wrappers to call the functions inside the namespace.
//******************************************************************************

int PyScriptEC_Initialize(char * strScript, char * strDir)
{
    return PyScriptEC::Initialize(strScript, strDir);
}
int PyScriptEC_PreConvert (int eInFormEngine, int eOutFormEngine,
    int eNormalizeOutput, bool bForward)
{
    return PyScriptEC::PreConvert (eInFormEngine, eOutFormEngine,
                        eNormalizeOutput, bForward);
}
int PyScriptEC_DoConvert (char * lpInBuffer, int nInLen, char * lpOutBuffer,
    int& rnOutLen)
{
    return PyScriptEC::DoConvert (lpInBuffer, nInLen, lpOutBuffer, rnOutLen);
}
