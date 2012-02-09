//
// IcuTranslitEC.cpp
//
// Rewritten by Jim Kornelsen for Linux on Dec 2 2011.
//
// 05-Dec-11 JDK  Put into a namespace.
// 06-Dec-11 JDK  Write return value directly using extract.
//
// When building, link with the ICU library and header files.
//
// g++ IcuTranslitEC.cpp -o IcuTranslitEC.exe -I/home/jkornelsen/p4repo/Calgary/FW_7.0/Lib/src/icu/installi686/include/ -L/usr/lib/fieldworks -l:libicuuc.so -l:libicui18n.so
// g++ IcuTranslitEC.cpp -o IcuTranslitEC.exe -I/usr/local/include -L/usr/local/lib -licuuc -licui18n

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// This file is from the ICU library. If you don't have it, you may have to
// download it or get it from a package such as FieldWorks.
#include "unicode/translit.h"

#include "CEncConverter.h"
#include "IcuTranslitEC.h"

// Uncomment the following line if you want verbose debugging output
//#define VERBOSE_DEBUGGING

// Keep this in a namespace so that it doesn't get confused with functions that
// have the same name in other converters, for example Load().
namespace IcuTranslitEC
{
    char * m_strConverterSpec = NULL;
    char   m_strFriendlyName[1000];

    bool            m_bLTR        = true; // if true, then use m_pTForwards
    Transliterator* m_pTForwards  = 0;
    Transliterator* m_pTBackwards = 0;

    char ** m_convNameList;
    int     m_iConvNameCount = -1;
    int     m_iConvNameIndex = 0; // which element will be next    

    // This routine is intended for debugging.
    static void
    printUnicodeString(const char *announce, const UnicodeString &s)
    {
#ifdef VERBOSE_DEBUGGING
        static char out[200];
        int32_t i, length;

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


    // Allocates a utf-8 (on Linux) char * from a UnicodeString.
    // After calling this function, be sure to free the result.
    char * UniStr_to_CharStar(UnicodeString uniStr, int & len)
    {
        len = uniStr.extract(0, uniStr.length(), (char *)NULL);   // "preflight" to get size
        int size = len + 1;
        char * newStr = (char *)malloc(size * sizeof(char));
        len = uniStr.extract(0, uniStr.length(), newStr);
        //newStr[len] = '\0'; // null terminate in case extract didn't do it
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

    bool IsFileLoaded()
    {
        return (m_pTForwards != 0);
    }

    void FinalRelease() 
    {
        if( m_pTForwards != 0 )
        {
            delete m_pTForwards;
            m_pTForwards = 0;
        }

        if( m_pTBackwards != 0 )
        {
            delete m_pTBackwards;
            m_pTBackwards = 0;
        }
        if (m_strConverterSpec != 0)
        {
            free(m_strConverterSpec);
            m_strConverterSpec = NULL;
        }
    }

    // Warning: If this function is just named Load(), and there is a Load() in
    // another library such as for IcuRegex, then the two will get mixed
    // up when calling from C#.  So either name them differently or else keep
    // them inside a C++ class or namespace.
    int Load(void)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuTranslitEC.CppLoad() BEGIN\n");
        fprintf(stderr, "m_strConverterSpec '%s'\n", m_strConverterSpec);
#endif
        int hr = 0;

        // but not if it's already loaded.
        if( IsFileLoaded() )
            return hr;

        UErrorCode status = U_ZERO_ERROR;
        UParseError parseError = { -1, 0, {0}, {0}};

        // it's not so clear what is a "rule-based" converter and what isn't. So just try to create it from 
        //  the normal approach first and if that fails, then try the createFromRules
        m_pTForwards = Transliterator::createInstance (
                       m_strConverterSpec, UTRANS_FORWARD, parseError, status);

        if( !m_pTForwards ) // IsRuleBased(strConverterSpec) )
        {
            UErrorCode statusFromRules = U_ZERO_ERROR;
            m_pTForwards = Transliterator::createFromRules (
                            m_strFriendlyName,
                            m_strConverterSpec, UTRANS_FORWARD, parseError,
                            statusFromRules);
        }

        // if the forward direction worked, see if there's a reversable one.
        if( m_pTForwards )
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Successfully created forward direction.\n");
#endif
            // use a different status variable since we don't *really* care if the reverse is possible 
            //  (i.e. even if it fails, we wouldn't want to return an error).
            UErrorCode statusRev = U_ZERO_ERROR;
            m_pTBackwards = m_pTForwards->createInverse(statusRev);

            if( m_pTBackwards )
            {
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "Successfully created backward direction.\n");
#endif
                //m_eConversionType = ConvType_Unicode_to_from_Unicode;
            }
        }
        else
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Failed to create ICU converter spec '%s'\n",
                    m_strConverterSpec);
#endif
            return -1;
        }

        if( U_FAILURE(status) )
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Failure status %d: %s\n",
                    status, u_errorName(status));
#endif
            hr = status;
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

    // Call this method once, and then keep calling ConverterNameList_next() until the end of
    // the list.
    // This method will create the list and store it in memory.
    // Returns the number of items in the list.
    //
    int ConverterNameList_start()
    {
        if (m_iConvNameCount >= 0)
        {
            // The list has already been created
            return m_iConvNameCount;
        }
        m_iConvNameCount = Transliterator::countAvailableIDs();
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "%d converter IDs available.\n", m_iConvNameCount);
#endif
        m_convNameList   = (char**)malloc(m_iConvNameCount * sizeof (char *));

        for(int i = 0; i < m_iConvNameCount; i++ )
        {
            UnicodeString strID = Transliterator::getAvailableID(i);
            printUnicodeString("Got ", strID);
            m_convNameList[i] = UniStr_to_CharStar(strID);
        }
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "Finished making list.\n");
#endif
        return m_iConvNameCount;
    }

    // This method will free the list from memory after the last element is
    // retrieved.
    char * ConverterNameList_next(void)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "+");
#endif
        if (m_iConvNameIndex >= m_iConvNameCount)
        {
            return NULL;
        }
        if (m_iConvNameIndex == m_iConvNameCount - 1)
        {
            // This is the last element in the list
            char * strLastItem = strdup(m_convNameList[m_iConvNameIndex]);
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Freeing list.\n");
#endif
            for (int i = 0; i < m_iConvNameCount; i++)
                free(m_convNameList[i]);
            free(m_convNameList);
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Freed list.\n");
#endif
            m_iConvNameCount = -1;
            m_iConvNameIndex = 0;
            return strLastItem;
        }
        return strdup(m_convNameList[m_iConvNameIndex++]);
    }

    // The ID is a string like "Any-Latin", and the display name is "Any to Latin".
    // Be sure to free the result when finished (C# marshalling will free it).
    char * GetDisplayName(char * strID)
    {
        UnicodeString strDisplayName;
        Transliterator::getDisplayName(strID, strDisplayName);
        return UniStr_to_CharStar(strDisplayName);
    }

    int Initialize(char * strConverterSpec)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuTranslitEC.CppInitialize() BEGIN\n");
        fprintf(stderr, "strConverterSpec '%s'\n", strConverterSpec);
#endif

        if( IsFileLoaded() )
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

        // keep track of which direction (can't use m_bForward as this may have come in from
        //	ConvertEx and be different than that).
        m_bLTR = bForward;

        // load the transliterator if it isn't already
        if( !IsFileLoaded() )
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
        fprintf(stderr, "IcuTranslitEC.CppDoConvert() BEGIN\n");
#endif
        int hr = 0;
        UnicodeString sInOut;
        //sInOut.setTo(lpInBuffer, nInLen / 2); // needed for Windows?
        sInOut.setTo(lpInBuffer);
        printUnicodeString("Will transliterate: ", sInOut);

        if( m_bLTR )
        {
            if( m_pTForwards )
            {
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "Doing forwards transliteration...\n");
#endif
                m_pTForwards->transliterate(sInOut);
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "Did forwards transliteration.\n");
#endif
            }
            else
            {
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "There is no forward transliterator.\n");
#endif
                hr = -1;
            }
        }
        else	// !m_bLTR
        {
            if( m_pTBackwards )
            {
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "Doing backwards transliteration...\n");
#endif
                m_pTBackwards->transliterate(sInOut);
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "Did backwards transliteration.\n");
#endif
            }
            else
            {
#ifdef VERBOSE_DEBUGGING
                fprintf(stderr, "There is no backwards transliterator.\n");
#endif
                hr = -1;
            }
        }

        if (hr == 0)
        {
            printUnicodeString("Result of transliteration: ", sInOut);
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "sInOut.length %d\n", sInOut.length());
#endif
            //int nLen = sInOut.length() * 2;   // for Windows?
            int nLen = sInOut.extract(0, sInOut.length(), (char *)NULL);   // "preflight" to get size
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
                //memcpy(lpOutBuffer,sInOut.getBuffer(),rnOutLen);  // works on Windows???
                nLen = sInOut.extract(0, sInOut.length(), lpOutBuffer);
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

int IcuTranslitEC_ConverterNameList_start(void)
{
    return IcuTranslitEC::ConverterNameList_start();
}
char * IcuTranslitEC_ConverterNameList_next(void)
{
    return IcuTranslitEC::ConverterNameList_next();
}
char * IcuTranslitEC_GetDisplayName(char * strID)
{
    return IcuTranslitEC::GetDisplayName(strID);
}
int IcuTranslitEC_Initialize(char * strConverterSpec)
{
    return IcuTranslitEC::Initialize(strConverterSpec);
}
int IcuTranslitEC_PreConvert (int eInEncodingForm, int& eInFormEngine,
    int eOutEncodingForm, int& eOutFormEngine,
    int& eNormalizeOutput, bool bForward,
    int nInactivityWarningTimeOut)
{
    return IcuTranslitEC::PreConvert (eInEncodingForm, eInFormEngine,
                    eOutEncodingForm, eOutFormEngine,
                    eNormalizeOutput, bForward,
                    nInactivityWarningTimeOut);
}
int IcuTranslitEC_DoConvert (char * lpInBuffer, int nInLen,
    char * lpOutBuffer, int& rnOutLen)
{
    return IcuTranslitEC::DoConvert (lpInBuffer, nInLen,
                    lpOutBuffer, rnOutLen);
}

//
// Example driver for testing.
//
/*
int main()
{
    fprintf(stderr, "main() BEGIN\n");

    int count = get_ConverterNameList_start();
    fprintf(stderr, "Got count %d\n", count);
    for (int i = 0; i < count; i++)
    {
        char * name = get_ConverterNameList_next();
        fprintf(stderr, "Got name '%s'\n", name);
        free(name);
    }

    //char strName[100] = "Any-Latin";
    //char strName[100] = "Any-Remove";
    char strName[100] = "Any-Upper";
    char * displayName = getDisplayName(strName);
    fprintf(stderr, "Display name '%s'\n", displayName);
    free(displayName);

    Initialize(strName);
    
    char strIn[] = "abcde";
    int szOut    = 1000;
    char strOut[szOut];
    int err = DoConvert(strIn, strlen(strIn), strOut, szOut);
    if (err != 0) {
        return err;
    }
    fprintf(stderr, "Got %s\n", strOut);

    fprintf(stderr, "main() END\n");
    return 0;
}
*/
