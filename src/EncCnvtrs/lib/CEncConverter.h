// ECEncConverter.h
//  This file contains a base class that implements a great deal of the code needed to
//  support an Encoding Converter's plug-in. You can derive a class from this class and 
//  implement a few methods to get started developing a plug-in. Some instructions for 
//  doing this are in the "Making a new Plug-in ReadMe.txt" in the Developers folder of the
//  Encoding Converters install set.
#pragma once

/*
char *        m_strProgramID;     // indicates the Program ID from the registry (e.g. "SilEncConverters40.TecEncConverter")
char *        m_strImplementType; // eg. "SIL.tec" rather than the program ID
char *        m_strPersistKey;    // Key to the persistance for this instance (e.g. Reg key)
char *        m_strLhsEncodingID; // something unique/unchangable (at least from TK) (e.g. SIL-UNICODE_DEVANAGRI-2002<>SIL-UNICODE_IPA-2002)
char *        m_strRhsEncodingID; // something unique/unchangable (at least from TK) (e.g. SIL-UNICODE_DEVANAGRI-2002<>SIL-UNICODE_IPA-2002)
char *        m_strConverterID;   // file spec to the map (or some plug-in specific identifier, such as "Devanagari-Latin" (for ICU) or "65001" (for code page UTF8)
long          m_lProcessType;     // process type (see .idl)
int           m_eConversionType;  // conversion type (see .idl)
bool          m_bForward;         // default direction of conversion (for bidirectional conversions; e.g. *not* CC)
int           m_eEncodingInput;   // encoding form of input (see .idl)
int           m_eEncodingOutput;  // encoding form of output (see .idl)
int           m_eNormalizeOutput; // should we normalize the output?
bool          m_bDebugDisplayMode;// should we display debug information?
int           m_nCodePageInput;
int           m_nCodePageOutput;

bool          m_bInitialized;
bool          m_bIsInRepository;  // indicates whether this converter is in the static repository (true) or not (false)
*/

// From ECInterfaces.cs
const int ConvType_Unknown = 0;
const int ConvType_Legacy_to_from_Unicode = 1;
const int ConvType_Legacy_to_from_Legacy = 2;
const int ConvType_Unicode_to_from_Legacy = 3;
const int ConvType_Unicode_to_from_Unicode = 4;
const int ConvType_Legacy_to_Unicode = 5;
const int ConvType_Legacy_to_Legacy = 6;
const int ConvType_Unicode_to_Legacy = 7;
const int ConvType_Unicode_to_Unicode = 8;
const int NormConversionType_eLegacy = 0;
const int NormConversionType_eUnicode = 1;
const int EncodingForm_Unspecified = 0;
const int EncodingForm_LegacyString = 1;
const int EncodingForm_UTF8String = 2;
const int EncodingForm_UTF16BE = 3;
const int EncodingForm_UTF16 = 4;
const int EncodingForm_UTF32BE = 5;
const int EncodingForm_UTF32 = 6;
const int EncodingForm_LegacyBytes = 20;	// something far enough away that TEC won't use it's value
const int EncodingForm_UTF8Bytes = 21;
const int NormalizeFlags_None = 0;
const int NormalizeFlags_FullyComposed = 0x0100;
const int NormalizeFlags_FullyDecomposed = 0x0200;

const char * strConvType(int eType)
{
    switch(eType)
    {
    case ConvType_Legacy_to_from_Unicode:
        return "Legacy_to_from_Unicode";
        break;
    case ConvType_Legacy_to_from_Legacy:
        return "Legacy_to_from_Legacy";
        break;
    case ConvType_Unicode_to_from_Legacy:
        return "Unicode_to_from_Legacy";
        break;
    case ConvType_Unicode_to_from_Unicode:
        return "Unicode_to_from_Unicode";
        break;
    case ConvType_Legacy_to_Unicode:
        return "Legacy_to_Unicode";
        break;
    case ConvType_Legacy_to_Legacy:
        return "Legacy_to_Legacy";
        break;
    case ConvType_Unicode_to_Legacy:
        return "Unicode_to_Legacy";
        break;
    case ConvType_Unicode_to_Unicode:
        return "Unicode_to_Unicode";
        break;

    case ConvType_Unknown:
    default:
        return "Unknown";
        break;
    }
}

const char * strNormalizeOutputType(int eNormalFlags)
{
    switch(eNormalFlags)
    {
    case NormalizeFlags_FullyDecomposed:
        return "FullyDecomposed";
        break;
    case NormalizeFlags_FullyComposed:
        return "FullyComposed";
        break;
    default:
    case NormalizeFlags_None:
        return "None";
        break;
    }
}
