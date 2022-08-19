#ifndef __PYSCRIPTENCCONVERTER_H__
#define __PYSCRIPTENCCONVERTER_H__

#pragma once

namespace PyScriptEC
{
// keep track of how we're supposed to interpret the data (it comes into "DoConvert"
//  as a 'byte' pointer, but we need to create it as a Python object which is 
//  different depending on the expected type.
#ifdef _MSC_VER
typedef
#endif
enum PyStringDataType
{
    eBytes,     // non-unicode bytes (or encoded unicode data, but EncConverters doesn't recommend this)
    eUTF8Bytes, // UTF8 bytes in C#, unicode string in python script
    eUCS2,      // Windows; unicode string in python script
    eUCS4       // Linux;   unicode string in python script
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
    DLLEXPORT int PyScriptEC_PreConvert (int eInFormEngine, int eOutFormEngine,
                        int eNormalizeOutput, bool bForward);
    DLLEXPORT int PyScriptEC_DoConvert (char * lpInBuffer, int nInLen, char * lpOutBuffer,
                        int& rnOutLen);

#ifdef __cplusplus
}   // Close the extern C.
#endif
#endif // PYSCRIPTENCCONVERTER_H
