// This DLL interface can be used to get access to EncConverter's convert capability.
//  It can be used by C and C++ clients, and also other languages such as python.
//
// With C and C++ clients, there are two ways to use this: dynamic and shared linking.
// The library libecdriver.so must be available at runtime with either approach.
// For shared, you have to compile the client app with this .h file, and link with the
//  libecdriver.so library.
// For dynamic, ECDriver does not need to be present when building the client app.
// However dynamic code is somewhat more complicated.
//
// Contact silconverters_support@sil.org if you need other things in this interface.

#ifndef __ECDRIVER_H__
#define __ECDRIVER_H__

#pragma once

#ifdef __cplusplus
//
// Build as C-style so that the symbols can be found without C++ name-mangling.
// Methods can still be implemented in the C++ language.
//
extern "C" {
#endif

    // this function returns whether a workable version of EncConverters is installed 
    //  (it might be from FW, SA, PA, or SILConverters)
    bool IsEcInstalled();

    // this function can be used to acquire a converter name and other run-time parameters
    //  using the EC auto-select interfaces (i.e. each transducer provides it's own UI, so you
    //  don't have to). If it returns S_OK, then the name to use in the subsequent calls, the
    //  direction flag, and the normalize output flag will be filled in automatically the
    //  parameters. (see snippet below)
    int EncConverterSelectConverter(
        char * lpszConverterName, bool & bDirectionForward, int& eNormOutputForm);

    // this function can be used to initialize a converter by its name and set its run-time 
    //  parameters if you already have configuration information (e.g. from a previous call
    //  to *SelectConverter, but on a subsequent invocation). You must do either this or 
    //  *SelectConverter before calling either of the *ConvertString calls below.
    int EncConverterInitializeConverter(
        const char * lpszConverterName, bool bDirectionForward, int eNormOutputForm);

    // this function can be used to convert a string of narrow bytes using the named converter
    //  this string of narrow bytes ought to be UTF8 if the input to the conversion is Unicode.
    //  You should pass the length of the buffer for the converted result.
    int EncConverterConvertString(
        const char * lpszConverterName, const char * lpszInput, char * lpszOutput, int nOutputLen);

    // this function can be used to get a description from the given converter name
    int EncConverterConverterDescription(
        const char * lpszConverterName, char * lpszDescription, int nDescriptionLen);

    // This function frees memory that is used by embedding Mono.
    void Cleanup();

#ifdef __cplusplus
}   // Close the extern C.

//  Here are some snippets for using the above:

/*
// this code is to ask the user to choose a converter
if (IsEcInstalled())
{
    char szConverterName[1000];
    bool bDirectionForward = TRUE;
    int eNormFormOutput = 0;
    if (EncConverterSelectConverter(szConverterName, bDirectionForward, eNormFormOutput) == 0)
    {
        // input data is UTF-8
        const char * lpszInput = "किताब";
        char szOutput[1000];
        EncConverterConvertString(szConverterName, lpszInput, szOutput, 1000);
        // szOutput contains the UTF-8 result
    }
}

// this code is to initialize a converter when you've already got the initialization information.
if (IsEcInstalled())
{
    char szConverterName[1000]; 
    strcpy(szConverterName, L"हिन्‍दी to Latin"); // note it could contain Unicode chars
    bool bDirectionForward = true;
    int eNormFormOutput = 0;

    // initialize the converter based on these stored configuration properties (but beware,
    //  it may no longer be installed! So do something in the 'else' case to inform user)
    if (EncConverterInitializeConverter(szConverterName, bDirectionForward, eNormFormOutput) == 0)
    {
        // input data is wide chars (UTF-16 or 'hacked-UTF16' if Legacy)
        const char * lpszInput = "किताब";
        char szOutput[1000];
        EncConverterConvertString(szConverterName, lpszInput, szOutput, 1000);
        // szOutput contains the UTF-8 result
    }
}
*/
#endif
#endif // ECDRIVER_H

