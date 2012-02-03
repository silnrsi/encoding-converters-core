// Created by Steve McConnel Feb 2, 2012 by copying and editing IcuTranslitEC.cpp,
// with reference to the old IcuEncConverter.cpp.

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// This file is from the ICU library. If you don't have it, you may have to
// download it or get it from a package such as FieldWorks.
#include "unicode/ucnv.h"
#include "unicode/uloc.h"

#include "CEncConverter.h"
#include "IcuConvEC.h"

// Keep this in a namespace so that it doesn't get confused with functions that
// have the same name in other converters, for example Load().
namespace IcuConvEC
{
	char* m_strConverterSpec = NULL;
	char* m_strStandardName = NULL;

	bool        m_bToUtf16;
	UConverter* m_pConverter  = 0;

	int     m_cConvNames = -1;	// number of converters
	int     m_iConvName = 0;	// which converter will be next

#if 0
	///////////////////////////////////////////////////////////////////////////
	// NAME
	//    printUnicodeString
	// DESCRIPTION
	//    This routine is intended for debugging.
	// RETURN VALUE
	//    none
	//
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
		printf("%s'%s' {", announce, out);

		// output the code units (not code points)
		length=s.length();
		for(i=0; i<length; ++i) {
			printf(" %04x", s.charAt(i));
		}
		printf(" }\n");
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
			return 0;		// don't bother if it's already loaded.

		fprintf(stderr, "IcuConvEC::Load() BEGIN\n");
		fprintf(stderr, "m_strConverterSpec = '%s'\n", m_strConverterSpec);

		UErrorCode status = U_ZERO_ERROR;

		// the syntax we use for custom converters is: "filename.dat:<custom converter>"
		//  (e.g. "./testdata.dat:customConverter"). If we get this syntax, then use the
		//  package interface.
		char* cnvNameInPkg = strrchr(m_strConverterSpec, ':');
		if (cnvNameInPkg != NULL)
		{
			size_t nIndex = m_strConverterSpec - cnvNameInPkg;
			char* pkgFile = strncpy((char *)malloc(nIndex + 1), m_strConverterSpec, nIndex);
			pkgFile[nIndex] = '\0';		// NUL-terminate the string since strncpy doesn't.
			++cnvNameInPkg;			// advance past the ':'
			m_pConverter = ucnv_openPackage(pkgFile, cnvNameInPkg, &status);
			free(pkgFile);
		}
		else
		{
			m_pConverter = ucnv_open(m_strConverterSpec, &status);
		}
		if (m_pConverter)
		{
			fprintf(stderr, "Successfully created ICU converter '%s'.\n", m_strConverterSpec);
		}
		else
		{
			fprintf(stderr, "Failed to create ICU converter '%s'\n", m_strConverterSpec);
			return -1;
		}
		if( U_FAILURE(status) )
		{
			fprintf(stderr, "... but failure status %d: %s\n", status, u_errorName(status));
			return status;
		}
		else
		{
			// get a version of the 'standard name' so we can use it in the
			// DefaultUnicodeEncForm (just in case the user used one of the
			// aliases for the converter identifier)
			const char* pszName = ucnv_getName(m_pConverter, &status);
			m_strStandardName = strdup(pszName);
		}
		fprintf(stderr, "IcuConvEC::Load() END\n");
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
		fprintf(stderr, "IcuConvEC::InactivityWarning\n");
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
		fprintf(stderr, "%d converter IDs available.\n", m_cConvNames);
		m_iConvName = 0;
		return m_cConvNames;
	}

	///////////////////////////////////////////////////////////////////////////
	// NAME
	//    ConverterNameList_next
	// DESCRIPTION
	//    Return the next converter name, or NULL if there are no more.
	// RETURN VALUE
	//    dynamically allocated copy of the converter's name, or NULL if no more
	//
	char * ConverterNameList_next(void)
	{
		fprintf(stderr, "+");
		if (m_iConvName >= m_cConvNames)
			return NULL;
		const char *convName = ucnv_getAvailableName(m_iConvName++);
		return strdup(convName);
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
	char * GetDisplayName(char * strID)
	{
		UErrorCode err = U_ZERO_ERROR;
		const char * locale = uloc_getDefault();
		UConverter * conv = m_pConverter;
		if (ucnv_compareNames(strID, m_strConverterSpec) != 0)
			conv = ucnv_open(strID, &err);
		int32_t length = ucnv_getDisplayName(conv, locale, NULL, 0, &err);
		UChar * displayU = (UChar *)malloc((length + 1) * sizeof(UChar));
		length = ucnv_getDisplayName(conv, locale, displayU, length, &err);
		if (sizeof(UChar) == sizeof(char))
			return (char*)displayU;
		UnicodeString * strDisplayName = new UnicodeString(displayU, length);
		free(displayU);
		if (conv != m_pConverter)
			ucnv_close(conv);
		return UniStr_to_CharStar(*strDisplayName);
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
		fprintf(stderr, "IcuConvEC.CppInitialize() BEGIN\n");
		fprintf(stderr, "strConverterSpec '%s'\n", strConverterSpec);

		if (IsFileLoaded())
			FinalRelease();

		m_strConverterSpec = strdup(strConverterSpec);

		// do the load at this point; not that we need it, but for checking
		// that everything is okay.
		int hr = Load();
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
		int hr = 0;
		// load the converter if it isn't already
		if (!IsFileLoaded())
			hr = Load();
		if (hr == 0)
		{
            // we need to know whether to go *to* wide or *from* wide for "DoConvert"
			m_bToUtf16 = bForward;
			if (m_bToUtf16)
			{
				// going "to wide" means the output form required by the engine is UTF16.
				eOutFormEngine = EncodingForm_UTF16;

				// TODO: (TECHNICALLY, we should be doing the following for the other Unicode 
				//	formats as well--i.e. if it is the UTF-32 converter, then... what??? 
				//	is the input UTF16 or is it UTF8? I'm not sure what to do for the other
				//	cases. Maybe we should add additional enums to the 'ConversionType' 
				//	enum saying "ConvTypeUTF8_to_from_UTF16" and so on for the 32, etc
				//	flavors... Then we could set the e???FormEngine values correctly.
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
		fprintf(stderr, "IcuConvEC.CppDoConvert() BEGIN\n");
		int hr = 0;
		UErrorCode status = U_ZERO_ERROR;
		int32_t len;

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
			len = ucnv_fromUChars(m_pConverter, (char*)lpOutBuffer, rnOutLen, 
				(const UChar*)lpInBuffer, nInLen, &status);
			rnOutLen = len;
		}
		if (U_FAILURE(status))
			hr = status;
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
char * IcuConvEC_ConverterNameList_next(void)
{
	return IcuConvEC::ConverterNameList_next();
}
//*****************************************************************************
char * IcuConvEC_GetDisplayName(char * strID)
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

//
// Example driver for testing.
//
/*
int main()
{
	fprintf(stderr, "main() BEGIN\n");

	int count = IcuConvEC_ConverterNameList_start();
	fprintf(stderr, "Got count %d\n", count);
	for (int i = 0; i < count; i++)
	{
		char * name = IcuConvEC_ConverterNameList_next();
		fprintf(stderr, "Got name '%s'\n", name);
		free(name);
	}

	char strName[100] = "ISO-8859:1";
	char * displayName = IcuConvEC_getDisplayName(strName);
	fprintf(stderr, "Display name '%s'\n", displayName);
	free(displayName);

	IcuConvEC_Initialize(strName);

	char strIn[] = "abcde";
	int szOut    = 1000;
	char strOut[szOut];
	int err = IcuConvEC_DoConvert(strIn, strlen(strIn), strOut, szOut);
	if (err != 0) {
		return err;
	}
	fprintf(stderr, "Got %s\n", strOut);

	fprintf(stderr, "main() END\n");
	return 0;
}
*/
