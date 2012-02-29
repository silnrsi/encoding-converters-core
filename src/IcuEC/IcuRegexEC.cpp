//
// IcuRegexEC.cpp
//
// Rewritten by Jim Kornelsen for Linux on Dec 6 2011.
//
// When building, link with the ICU library and header files.
//
// g++ IcuRegexEC.cpp -o IcuRegexEC.exe -I/usr/local/include -L/usr/local/lib -licuuc -licui18n

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// This file is from the ICU library. If you don't have it, you may have to
// download it or get it from a package such as FieldWorks.
#include "unicode/regex.h"

#include "CEncConverter.h"
#include "IcuRegexEC.h"

// Uncomment the following line if you want verbose debugging output
//#define VERBOSE_DEBUGGING

// Keep this in a namespace so that it doesn't get confused with functions that
// have the same name in other converters, for example Load().
namespace IcuRegexEC
{
    char *          m_strConverterSpec = NULL;
    UnicodeString   m_strFind;
    UnicodeString   m_strReplace;
    RegexMatcher*   m_pMatcher;

    // This routine is intended for debugging.
    static void
    printUnicodeString(const char *announce, const UnicodeString &s)
    {
#ifdef VERBOSE_DEBUGGING
        static char out[200];
        int32_t i, length;

        // output the string, converted to the platform encoding

        // Note for Windows: The "platform encoding" defaults to the "ANSI
        // codepage", which is different from the "OEM codepage" in the
        // console window. However, if you pipe the output into a file and
        // look at it with Notepad
        // or similar, then "ANSI" characters will show correctly.
        // Production code should be aware of what encoding is required,
        // and use a UConverter or at least a charset name explicitly.
        out[s.extract(0, 99, out)]=0;
        printf("%s'%s' {", announce, out);

        // output the code units (not code points)
        length=s.length();
        for(i=0; i<length; ++i) {
            printf(" %04x", s.charAt(i));
        }
        printf(" }\n");
#endif
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
		strncpy_s(dst, len + 1, src, len);
		return dst;
	}
#endif

	// Allocates a utf-8 (on Linux) char * from a UnicodeString.
    // After calling this function, be sure to free the result.
    char * UniStr_to_CharStar(UnicodeString uniStr, int & len)
    {
        len = uniStr.extract(0, uniStr.length(), (char *)NULL);   // "preflight" to get size
        int size = len + 1;
        char * newStr = (char *)malloc(size * sizeof(char));
        len = uniStr.extract(0, uniStr.length(), newStr);
        //newStr[len] = '\0'; // null terminate
        //m_convNameList[i] = strdup((char*)strID.getTerminatedBuffer());
        //LPCTSTR lpszValue = strDisplayName.getTerminatedBuffer();
        //if( _tcslen(lpszValue) == 0 )
        //    lpszValue = strID.getTerminatedBuffer();
        return newStr;
    }

    // Use this wrapper if you don't care about the resulting string's length.
    char * UniStr_to_CharStar(UnicodeString uniStr)
    {
        int len;
        return UniStr_to_CharStar(uniStr, len);
    }

    bool IsMatcherLoaded()
    {
        return (m_pMatcher != 0); 
    }

    void FinalRelease() 
    {
        if( m_pMatcher != 0 )
        {
            delete m_pMatcher;
            m_pMatcher = 0;
        }
        if (m_strConverterSpec != 0)
        {
            free(m_strConverterSpec);
            m_strConverterSpec = NULL;
        }
    }

    // the format of the converter spec for ICU regular expressions is:
    //  {Find string}->{Replace} ({flags})
    // e.g.
    //  "[aeiou]->V /i"
    const char clpszFindReplaceDelimiter[]   = "->";
    const char clpszCaseInsensitiveFlag[]    = " /i";
    //
    // Split up strConverterSpec into the Find part and the Replace part.
    // If successful (return value true), then be sure to free strFind and
    // strReplace when finished.
    // Sets bIgnoreCase if the flag is present.
    //
    bool ParseConverterSpec
    (
        char *    strConverterSpec,
        char *    &strFind,      // string passed by reference
        char *    &strReplace,   // string passed by reference
        bool      &bIgnoreCase
    )
    {
        // split up the converter spec into the pieces: 
        //  <Find>-><Replace> (<Flags>)*
        // BUT BEWARE, is there any reason that they couldn't have spaces
        // in there? So instead of using 
        // RegexMatcher::split, do it more carefully
        char * found = strstr(strConverterSpec, clpszFindReplaceDelimiter);
        if (found == NULL)
            return false;

        // set strFind to everything up to but not including the delimiter,
        // and strReplace to everything after the delimiter.
        strFind = strndup(strConverterSpec,
                     strlen(strConverterSpec) - strlen(found));
        strReplace = strdup(found + strlen(clpszFindReplaceDelimiter));

        found = strstr(strConverterSpec, clpszCaseInsensitiveFlag);
        if (found != NULL)
        {
            bIgnoreCase = true;
            // remove the flag from strReplace
            char * swap = strndup(strReplace,
                            strlen(strReplace) - strlen(found));
            free(strReplace);
            strReplace = swap;
        }
        return true;
    }

    // Warning: If this function is just named Load(), and there is a Load() in
    // another library such as for python scripts, then the two will get mixed
    // up when calling from C#.  So either name them differently or else put
    // them inside a C++ class or namespace.
    int Load(void)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuRegexEC.CppLoad() BEGIN\n");
#endif
        int hr = 0;

        // but not if it's already loaded.
        if( IsMatcherLoaded() )
            return hr;

        bool bIgnoreCase = false;
        char *strFind, *strReplace;
        if (ParseConverterSpec (
            m_strConverterSpec, strFind, strReplace, bIgnoreCase) )
        {
            uint32_t flags = 0; // by default
            if( bIgnoreCase )
                flags |= UREGEX_CASE_INSENSITIVE;

            m_strFind.setTo(strFind);
            m_strReplace.setTo(strReplace);
            free(strFind);
            free(strReplace);
            UErrorCode status = U_ZERO_ERROR;
            printUnicodeString("Creating matcher with: ", m_strFind);
            m_pMatcher = new RegexMatcher(m_strFind, flags, status);
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Created matcher.\n");
#endif

            if( U_FAILURE(status) )
            {
                // the base class does ReturnError and if we do it also, then it inverts the error
                //  code twice
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "Failure status %d: %s\n",
                        status, u_errorName(status));
#endif
                hr = status;
            }
        }
        else
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr,
                "IcuRegex: The converter identifier '%s' is invalid!\n\nUsage: <Find>-><Replace> (<Flags>)*\n\n\twhere <Flags> can be '/i'",
                m_strConverterSpec);
#endif
            return -1;
        }
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "Load() END\n");
#endif
        return hr;
    }

    // call to clean up resources when we've been inactive for some time.
    void InactivityWarning()
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuTransEC::InactivityWarning\n");
#endif
        FinalRelease();
    }

    int Initialize(char * strConverterSpec)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuRegexEC.CppInitialize() BEGIN\n");
        fprintf(stderr, "strConverterSpec '%s'\n", strConverterSpec);
#endif

        if( IsMatcherLoaded() )
            FinalRelease();

        m_strConverterSpec = strdup(strConverterSpec);

        // do the load at this point; not that we need it, but for checking that everything's okay.
        return Load();
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

        // Basically, all transliterators are UTF16.
        //eInFormEngine = eOutFormEngine = EncodingForm_UTF16;
        eInFormEngine = eOutFormEngine = EncodingForm_UTF8String; // Linux?

        // load the transliterator if it isn't already
        if( !IsMatcherLoaded() )
            hr = Load();

        return hr;
    }

    int DoConvert
    (
        char *  lpInBuffer,
        int     nInLen,
        char *  lpOutBuffer,
        int&    rnOutLen
    )
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuRegexEC.CppDoConvert() BEGIN\n");
#endif
        int hr = 0;
        UnicodeString sInput;
        //sInput.setTo(lpInBuffer, nInLen / 2); // needed for Windows?
        sInput.setTo(lpInBuffer);
        printUnicodeString("Will transliterate: ", sInput);
        m_pMatcher->reset(sInput);

        UErrorCode status = U_ZERO_ERROR;
        printUnicodeString("Calling replaceAll with: ", m_strReplace);
        UnicodeString sOutput = m_pMatcher->replaceAll(m_strReplace, status);
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "Called replaceAll.\n");
#endif

        if( U_FAILURE(status) )
        {
            // the base class does ReturnError and if we do it also, then it
            // inverts the error code twice
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Failure status %d: %s\n",
                    status, u_errorName(status));
#endif
            hr = status;
        }
        else
        {
            //int nLen = sOutput.length() * sizeof(char); // needed for Windows?
            printUnicodeString("Result of regex: ", sOutput);
            int nLen = sOutput.extract(0, sOutput.length(), (char *)NULL);   // "preflight" to get size
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "preflight nLen %d, rnOutLen %d\n", nLen, rnOutLen);
#endif
            if( nLen >= (int)rnOutLen )
            {
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "Length %d more than output buffer size %d\n",
                        nLen, rnOutLen);
#endif
                hr = -1;
            }
            else
            {
                //memcpy(lpOutBuffer,sOutput.getBuffer(),rnOutLen);  // works on Windows???
                nLen = sOutput.extract(0, sOutput.length(), lpOutBuffer);
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "actual nLen %d\n", nLen);
#endif
                rnOutLen = nLen;
                //lpOutBuffer[rnOutLen] = '\0';   // null terminate in case extract didn't do it
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "lpOutBuffer length = %u\n", (unsigned)strlen(lpOutBuffer));
                fprintf(stderr, "lpOutBuffer: '%s'\n", lpOutBuffer);
#endif
            }
        }
        return hr;
    }
}

//******************************************************************************
// Global wrappers to call the functions inside the namespace.
//******************************************************************************

int IcuRegexEC_Initialize(char * strConverterID)
{
    return IcuRegexEC::Initialize(strConverterID);
}
int IcuRegexEC_PreConvert (int eInEncodingForm, int& eInFormEngine,
    int eOutEncodingForm, int& eOutFormEngine,
    int& eNormalizeOutput, bool bForward,
    int nInactivityWarningTimeOut)
{
    return IcuRegexEC::PreConvert (eInEncodingForm, eInFormEngine,
                    eOutEncodingForm, eOutFormEngine,
                    eNormalizeOutput, bForward,
                    nInactivityWarningTimeOut);
}
int IcuRegexEC_DoConvert (char * lpInBuffer, int nInLen,
    char * lpOutBuffer, int& rnOutLen)
{
    return IcuRegexEC::DoConvert (lpInBuffer, nInLen,
                    lpOutBuffer, rnOutLen);
}

//
// Example driver for testing.
//
/*
int main()
{
    fprintf(stderr, "main() BEGIN\n");

    //char strRegex[] = "[aeiou] ->V /i";   // matches things like "a "
    char strRegex[] = "[aeiou]->V /i";
    IcuRegexEC::Initialize(strRegex);
    
    char strIn[] = "abcde";
    int szOut    = 1000;
    char strOut[szOut];
    int err = IcuRegexEC::DoConvert(strIn, strlen(strIn), strOut, szOut);
    if (err != 0) {
        return err;
    }
    fprintf(stderr, "Got %s\n", strOut);

    fprintf(stderr, "main() END\n");
    return 0;
}
*/
