//
// Note: To check the contents of the ICU libraries, you can do this:
// readelf -Ws libicuuc.so
//
#ifndef __ICUREGEXEC_H__
#define __ICUREGEXEC_H__

#pragma once

#ifdef __cplusplus
//
// Build as C-style so that the symbols can be found without C++ name-mangling.
// Methods can still be implemented in the C++ language.
//
extern "C" {
#endif

    int IcuRegexEC_Initialize(char * strConverterID);
    int IcuRegexEC_PreConvert (int eInEncodingForm, int& eInFormEngine,
                        int eOutEncodingForm, int& eOutFormEngine,
                        int& eNormalizeOutput, bool bForward,
                        int nInactivityWarningTimeOut);
    int IcuRegexEC_DoConvert (char * lpInBuffer, int nInLen,
                        char * lpOutBuffer, int& rnOutLen);

#ifdef __cplusplus
}   // Close the extern C.
#endif
#endif // ICUREGEXEC_H

