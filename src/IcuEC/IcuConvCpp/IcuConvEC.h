// Created by Steve McConnel Feb 2, 2012 by copying and editing IcuTranslitEC.h
//
// Note: To check the contents of the ICU libraries, you can do this:
// readelf -Ws libicuuc.so
//
#ifndef __ICUCONVEC_H__
#define __ICUCONVEC_H__

#pragma once

#ifdef __cplusplus
// Build as C-style so that the symbols can be found without C++ name-mangling.
// Methods can still be implemented in the C++ language.
extern "C"
{
#endif

#ifdef _MSC_VER
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT
#endif

    DLLEXPORT int IcuConvEC_ConverterNameList_start(void);

#ifdef _MSC_VER
    DLLEXPORT BSTR IcuConvEC_ConverterNameList_next(void);
    DLLEXPORT BSTR IcuConvEC_GetDisplayName(char * strID);
#else
    DLLEXPORT const char * IcuConvEC_ConverterNameList_next(void);
    DLLEXPORT const char * IcuConvEC_GetDisplayName(char * strID);
#endif

    DLLEXPORT int IcuConvEC_Initialize(char * strConverterID);
    DLLEXPORT int IcuConvEC_PreConvert(int eInEncodingForm, int & eInFormEngine,
		int eOutEncodingForm, int & eOutFormEngine,
		int & eNormalizeOutput, bool bForward, int nInactivityWarningTimeOut);
    DLLEXPORT int IcuConvEC_DoConvert(char * lpInBuffer, int nInLen, char * lpOutBuffer, int & rnOutLen);

#ifdef __cplusplus
}   // Close the extern C.
#endif
#endif // ICUCONVEC_H
