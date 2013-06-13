//
// Note: To check the contents of the .so, you can do this:
// nm -g libPyScriptEncConverter.so.1.0
//
#ifndef __PYSCRIPTENCCONVERTER_H__
#define __PYSCRIPTENCCONVERTER_H__

#pragma once

namespace PyScriptEC
{
// keep track of how we're supposed to interpret the data (it comes into "DoConvert"
//  as a 'byte' pointer, but we need to create it as a Python object which is 
//  different depending on the expected type.
typedef enum PyStringDataType
{
    eBytes,
    eUCS2,
    eUCS4       // not supported yet by this code
};

// make my own version of certain Python macros (since they aren't doing type checking or null pointer testing!)
#define MyPyTuple_Check(op)     ((!op) ? false : PyObject_TypeCheck(op, &PyTuple_Type))
#define MyPyFunction_Check(op)  ((!op) ? false : PyObject_TypeCheck(op, &PyFunction_Type))
#define MyPyCallable_Check(op)  ((!op) ? false : PyCallable_Check(op))
#define MyPyClass_Check(op)     ((!op) ? false : PyObject_TypeCheck(op, &PyClass_Type))
#define MyPyInstance_Check(op)  ((!op) ? false : PyObject_TypeCheck(op, &PyInstance_Type))
#define MyPyString_Check(op)    ((!op) ? false : PyObject_TypeCheck(op, &PyString_Type))
#define MyPyTraceBack_Check(op) ((!op) ? false : PyObject_TypeCheck(op, &PyTraceBack_Type))
#define MyPyType_Check(op)      ((!op) ? false : PyObject_TypeCheck(op, &PyType_Type))
#define MyPyDict_Check(op)      ((!op) ? false : PyObject_TypeCheck(op, &PyDict_Type))
#define MyPyList_Check(op)      ((!op) ? false : PyObject_TypeCheck(op, &PyList_Type))

/////////////////////////////////////////////////////////////////////////////
// CPyScriptEncConverter
#define PythonInactivityWarningTimeOut 60000   // 60 seconds of inactivity means clean up resources
}

#ifdef __cplusplus
//
// Build as C-style so that the symbols can be found without C++ name-mangling.
// Methods can still be implemented in the C++ language.
//
extern "C" {
#endif

#ifdef _MSC_VER
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT
#endif

    DLLEXPORT int PyScriptEC_Initialize(char * strScript, char * strDir);
    DLLEXPORT int PyScriptEC_PreConvert (int eInEncodingForm, int& eInFormEngine,
                        int eOutEncodingForm, int& eOutFormEngine,
                        int& eNormalizeOutput, bool bForward,
                        int nInactivityWarningTimeOut);
    DLLEXPORT int PyScriptEC_DoConvert (char * lpInBuffer, int nInLen, char * lpOutBuffer,
                        int& rnOutLen);

#ifdef __cplusplus
}   // Close the extern C.
#endif
#endif // PYSCRIPTENCCONVERTER_H
