// Created by Steve McConnel Feb 2, 2012 by copying and editing IcuTranslitEC.cpp,
// with reference to the old IcuEncConverter.cpp.

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>

// These headers are from the ICU library. If you don't have it, you may have to
// download it or get it from a package such as FieldWorks.
#include "unicode/ucnv.h"
#include "unicode/uloc.h"
#include "unicode/unistr.h"

#include "CEncConverter.h"
#include "IcuConvEC.h"

// Uncomment the following line if you want verbose debugging output
//#define VERBOSE_DEBUGGING

#ifndef _MSC_VER
#define _strdup   strdup
#define strncpy_s strncpy
#define strcpy_s  strcpy
#define strcat_s  strcat
#endif

#define MAXNAMESIZE 300

// Keep this in a namespace so that it doesn't get confused with functions that
// have the same name in other converters, for example Load().
namespace IcuConvEC
{
    char* m_strConverterSpec = NULL;
    char* m_strStandardName = NULL;

    bool        m_bToUtf16;
    UConverter* m_pConverter  = 0;

    int     m_cConvNames = -1;  // number of converters
    int     m_iConvName = 0;    // which converter will be next

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    printUnicodeString
    // DESCRIPTION
    //    This routine is intended for debugging.
    // RETURN VALUE
    //    none
    //
#ifdef VERBOSE_DEBUGGING
    static void printUnicodeString(const char *announce, const UnicodeString &s)
    {
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
        fprintf(stderr, "%s'%s' {", announce, out);

        // output the code units (not code points)
        length=s.length();
        for(i=0; i<length; ++i) {
            fprintf(stderr, " %04x", s.charAt(i));
        }
        fprintf(stderr, " }\n");
    }
#endif

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    UniStr_to_CharStar
    // DESCRIPTION
    //    Allocate a UTF-8 "char *" string from a UnicodeString.
    //    After calling this function, be sure to free the result.
    // RETURN VALUE
    //    pointer to a dynamically allocated UTF-8 character string
    //
    char * UniStr_to_CharStar(UnicodeString uniStr, int & len)
    {
        // "preflight" to get size
        len = uniStr.extract(0, uniStr.length(), (char *)NULL);
        int size = len + 1;
        char * newStr = (char *)malloc(size * sizeof(char));
        len = uniStr.extract(0, uniStr.length(), newStr);
        return newStr;
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    UniStr_to_CharStar
    // DESCRIPTION
    //    Allocate a UTF-8 "char *" string from a UnicodeString.
    //    After calling this function, be sure to free the result.
    //    Use this wrapper if you don't care about the string's length.
    // RETURN VALUE
    //    pointer to a dynamically allocated UTF-8 character string
    //
    char * UniStr_to_CharStar(UnicodeString uniStr)
    {
        int len;
        return UniStr_to_CharStar(uniStr, len);
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    IsFileLoaded
    // DESCRIPTION
    //    Test whether the converter has been initialized.
    // RETURN VALUE
    //    true iff the converter has been initialized
    //
    bool IsFileLoaded()
    {
        return (m_pConverter != 0);
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    FinalRelease
    // DESCRIPTION
    //    Dispose of all the memory allocated and close the converter.
    // RETURN VALUE
    //    none
    //
    void FinalRelease()
    {
        if (m_pConverter != 0)
        {
            ucnv_close(m_pConverter);
            m_pConverter = 0;
        }
        if (m_strConverterSpec != 0)
        {
            free(m_strConverterSpec);
            m_strConverterSpec = NULL;
        }
        if (m_strStandardName != 0)
        {
            free(m_strStandardName);
            m_strStandardName = NULL;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    Load
    // DESCRIPTION
    //    Initialize the converter, opening the internal ICU converter.
    // RETURN VALUE
    //    0 if successful, nonzero if an error occurs
    //
    int Load(void)
    {
        if (IsFileLoaded())
            return 0;       // don't bother if it's already loaded.

#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::Load() BEGIN\n");
        fprintf(stderr, "m_strConverterSpec = '%s'\n", m_strConverterSpec);
#endif

        UErrorCode status = U_ZERO_ERROR;

        // the syntax we use for custom converters is: "filename.dat:<custom converter>"
        //  (e.g. "./testdata.dat:customConverter"). If we get this syntax, then use the
        //  package interface.
        char* cnvNameInPkg = strrchr(m_strConverterSpec, ':');
        if (cnvNameInPkg != NULL)
        {
            size_t nIndex = m_strConverterSpec - cnvNameInPkg;
            char* pkgFile = (char *)malloc(nIndex + 1);
#if _MSC_VER
            strncpy_s(pkgFile, nIndex, m_strConverterSpec, nIndex);
#else
            strncpy(pkgFile, m_strConverterSpec, nIndex);
#endif
            pkgFile[nIndex] = '\0';     // NUL-terminate the string since strncpy doesn't.
            ++cnvNameInPkg;         // advance past the ':'
            m_pConverter = ucnv_openPackage(pkgFile, cnvNameInPkg, &status);
            free(pkgFile);
        }
        else
        {
            m_pConverter = ucnv_open(m_strConverterSpec, &status);
        }
        if (m_pConverter)
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Successfully created ICU converter '%s'.\n", m_strConverterSpec);
#endif
        }
        else
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "Failed to create ICU converter '%s'\n", m_strConverterSpec);
#endif
            return -1;
        }
        if( U_FAILURE(status) )
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "... but failure status %d: %s\n", status, u_errorName(status));
#endif
            return status;
        }
        else
        {
            // get a version of the 'standard name' so we can use it in the
            // DefaultUnicodeEncForm (just in case the user used one of the
            // aliases for the converter identifier)
            const char* pszName = ucnv_getName(m_pConverter, &status);
            m_strStandardName = _strdup(pszName);
        }
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::Load() END\n");
#endif
        return 0;
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    InactivityWarning
    // DESCRIPTION
    //    call to clean up resources when we've been inactive for some time.
    // RETURN VALUE
    //    none
    //
    void InactivityWarning()
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::InactivityWarning\n");
#endif
        FinalRelease();
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    ConverterNameList_start
    // DESCRIPTION
    //    Call this method once, and then keep calling ConverterNameList_next()
    //    until the end of the list.
    //    This method determines the number of converters, and initializes the
    //    index to retrieve the first converter.
    // RETURN VALUE
    //    the number of converters available
    int ConverterNameList_start()
    {
        m_cConvNames = ucnv_countAvailable();
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "%d converter IDs available.\n", m_cConvNames);
#endif
        m_iConvName = 0;
        return m_cConvNames;
    }

    const char * ReturnableString(const char* name)
    {
        static char chbuf[MAXNAMESIZE];
        size_t len = strlen(name);
        if (len >= MAXNAMESIZE)
            len = MAXNAMESIZE - 1;
        strncpy_s(chbuf, name, len);
        chbuf[len] = 0;
        return (const char *)chbuf;
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    ConverterNameList_next
    // DESCRIPTION
    //    Return the next converter name, or NULL if there are no more.
    // RETURN VALUE
    //    dynamically allocated copy of the converter's name, or NULL if no more
    //
    const char * ConverterNameList_next(void)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "+");
#endif
        if (m_iConvName >= m_cConvNames)
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "\n");
#endif
            return NULL;
        }

        const char *convName = ucnv_getAvailableName(m_iConvName++);

#ifndef _MSC_VER
        return strdup(convName);
#endif

        // The following code crashed on Linux during testing.
        char buf[MAXNAMESIZE];
        strcpy_s(buf, convName);

        UErrorCode status = U_ZERO_ERROR;
        int nSizeAliases = ucnv_countAliases(convName, &status);

        if( nSizeAliases > 1 )
            strcat_s(buf, " (aliases: ");

        for( int j = 1; U_SUCCESS(status) && (j < nSizeAliases); j++ )
        {
            const char * lpszAlias = ucnv_getAlias(convName, (uint16_t)j, &status);
            if( U_SUCCESS(status) )
            {
                strcat_s(buf, lpszAlias);
                if (j < (nSizeAliases - 1))
                    strcat_s(buf, " OR ");
            }
        }

        if( nSizeAliases > 1 )
        {
            strcat_s(buf, ")");
        }

        return ReturnableString(buf);
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    GetDisplayName
    // DESCRIPTION
    //    The ID is a string like "Any-Latin", and the display name is
    //    "Any to Latin".  Be sure to free the result when finished (C#
    //    marshalling will free it).
    // RETURN VALUE
    //    dynamically allocated copy of the display name of the given converter
    //
    const char * GetDisplayName(char * strID)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::GetDisplayName(\"%s\") BEGIN\n", strID);
#endif
        UErrorCode err = U_ZERO_ERROR;
        const char * locale = uloc_getDefault();
        UConverter * conv = m_pConverter;
        if (m_pConverter == NULL || ucnv_compareNames(strID, m_strConverterSpec) != 0)
            conv = ucnv_open(strID, &err);
        UChar buffer[MAXNAMESIZE];
        int32_t length = ucnv_getDisplayName(conv, locale, buffer, MAXNAMESIZE, &err);
        if (length >= MAXNAMESIZE)
            length = MAXNAMESIZE - 1;
        buffer[length] = 0;
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::GetDisplayName() => \"%s\" END\n", buffer);
#endif
        UnicodeString strDisplayName(buffer, length);
        const char * name = UniStr_to_CharStar(strDisplayName);
        if (conv != m_pConverter)
            ucnv_close(conv);
#ifdef _MSC_VER
        const char* str = ReturnableString(name);
        free((void *)name);
        return str;

        /*
        static char chbuf[MAXNAMESIZE];
        size_t len = strlen(name);
        if (len >= MAXNAMESIZE)
            len = MAXNAMESIZE - 1;
        strncpy_s(chbuf, name, len);
        chbuf[len] = 0;
        free((void *)name);
        return (const char *)chbuf;
        */
#else
        return name;
#endif
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    Initialize
    // DESCRIPTION
    //    Initialize the converter, saving the spec for future reference.
    // RETURN VALUE
    //    0 if successful, nonzero if an error occurs
    //
    int Initialize(char * strConverterSpec)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::Initialize() BEGIN\n");
        fprintf(stderr, "strConverterSpec '%s'\n", strConverterSpec);
#endif

        if (IsFileLoaded())
            FinalRelease();

        m_strConverterSpec = _strdup(strConverterSpec);

        // do the load at this point; not that we need it, but for checking
        // that everything is okay.
        int hr = Load();
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::Initialize() END\n");
#endif
        return hr;
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    PreConvert
    // DESCRIPTION
    //    Do the final setup to prepare for converting text.
    // RETURN VALUE
    //    0 if successful, nonzero if an error occurs
    //
    int PreConvert(
        int  eInEncodingForm,
        int& eInFormEngine,
        int  eOutEncodingForm,
        int& eOutFormEngine,
        int& eNormalizeOutput,
        bool bForward,
        int  nInactivityWarningTimeOut)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::PreConvert(): BEGIN\n");
#endif
        int hr = 0;
        // load the converter if it isn't already
        if (!IsFileLoaded())
            hr = Load();
        if (hr == 0)
        {
            // we need to know whether to go *to* wide or *from* wide for "DoConvert"
            m_bToUtf16 = (eOutEncodingForm == EncodingForm_UTF16);
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "IcuConvEC::PreConvert(): m_bToUtf16=%d\n", (int)m_bToUtf16);
#endif
            if (m_bToUtf16)
            {
                // going "to wide" means the output form required by the engine is UTF16.
                eOutFormEngine = EncodingForm_UTF16;

                // TODO: (TECHNICALLY, we should be doing the following for the other Unicode 
                //  formats as well--i.e. if it is the UTF-32 converter, then... what??? 
                //  is the input UTF16 or is it UTF8? I'm not sure what to do for the other
                //  cases. Maybe we should add additional enums to the 'ConversionType' 
                //  enum saying "ConvTypeUTF8_to_from_UTF16" and so on for the 32, etc
                //  flavors... Then we could set the e???FormEngine values correctly.
                if (ucnv_compareNames(m_strConverterSpec, "utf-8") == 0)
                    eInFormEngine = EncodingForm_UTF8Bytes;
            }
            else
            {
                // going "from wide" means the input form required by the engine is UTF16.
                eInFormEngine = EncodingForm_UTF16;

                if (ucnv_compareNames(m_strConverterSpec, "utf-8") == 0)
                    eOutFormEngine = EncodingForm_UTF8Bytes;
            }
        }
        else
        {
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "IcuConvEC::DoConvert(): Load() failed!\n");
#endif
        }
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::PreConvert(): END\n");
#endif
        return hr;
    }

    ///////////////////////////////////////////////////////////////////////////
    // NAME
    //    DoConvert
    // DESCRIPTION
    //    Convert the characters in the input buffer from one encoding to
    //    another, filling the output buffer with the result and setting the
    //    length of the output.
    // RETURN VALUE
    //    0 if successful, nonzero if an error occurs
    //
    int DoConvert(
        char* lpInBuffer,
        int   nInLen,
        char* lpOutBuffer,
        int&  rnOutLen)
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::DoConvert() BEGIN\n");
#endif
        int hr = 0;
        UErrorCode status = U_ZERO_ERROR;
        int32_t len;

#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConEC::DoConvert(): m_bToUtf16=%d, nInLen=%d, lpInBuffer[0]=%d\n",
            (int)m_bToUtf16, nInLen, (int)lpInBuffer[0]);
#endif
        if (m_bToUtf16)
        {
            // Convert from ??? to Unicode
            len = ucnv_toUChars(m_pConverter, (UChar*)lpOutBuffer, rnOutLen, 
                (const char*)lpInBuffer, nInLen, &status);
            // calling function expecting # of bytes, but ICU returns # of items
            rnOutLen = len * sizeof(UChar);
        }
        else
        {
            // Convert from Unicode to ???
            // ICU is expecting the # of items
            nInLen /= sizeof(UChar);
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "IcuConvEC::DoConvert(): adjusted nInLen=%d, rnOutLen=%d\n",
                nInLen, rnOutLen);
#endif
            len = ucnv_fromUChars(m_pConverter, (char*)lpOutBuffer, rnOutLen, 
                (const UChar*)lpInBuffer, nInLen, &status);
            rnOutLen = len;
#ifdef VERBOSE_DEBUGGING
            fprintf(stderr, "IcuConvEC::DoConvert(): after conversion, rnOutLen=%d\n",
                rnOutLen);
#endif
        }
        if (U_FAILURE(status))
            hr = status;
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "IcuConvEC::DoConvert() END\n");
#endif
        return hr;
    }
}

//*****************************************************************************
// Global wrappers to call the functions inside the namespace.
//*****************************************************************************
int IcuConvEC_ConverterNameList_start(void)
{
    return IcuConvEC::ConverterNameList_start();
}
//*****************************************************************************
const char * IcuConvEC_ConverterNameList_next(void)
{
    return IcuConvEC::ConverterNameList_next();
}
//*****************************************************************************
const char * IcuConvEC_GetDisplayName(char * strID)
{
    return IcuConvEC::GetDisplayName(strID);
}
//*****************************************************************************
int IcuConvEC_Initialize(char * strConverterSpec)
{
    return IcuConvEC::Initialize(strConverterSpec);
}
//*****************************************************************************
int IcuConvEC_PreConvert(int eInEncodingForm, int& eInFormEngine,
    int eOutEncodingForm, int& eOutFormEngine,
    int& eNormalizeOutput, bool bForward, int nInactivityWarningTimeOut)
{
    return IcuConvEC::PreConvert(eInEncodingForm, eInFormEngine,
        eOutEncodingForm, eOutFormEngine,
        eNormalizeOutput, bForward, nInactivityWarningTimeOut);
}
//*****************************************************************************
int IcuConvEC_DoConvert (char* lpInBuffer, int nInLen, char* lpOutBuffer, int& rnOutLen)
{
    return IcuConvEC::DoConvert (lpInBuffer, nInLen, lpOutBuffer, rnOutLen);
}
