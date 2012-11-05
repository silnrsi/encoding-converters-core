// Created by Jim Kornelsen on Dec 6 2011
// 28-Nov-11 JDK  Wrap Perl expression and write in temp file, rather than requiring input file.
//
using System;
using System.Runtime.InteropServices;

using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// Managed ICU Regex EncConverter
    /// </summary>
    //[GuidAttribute("54E0185D-3603-4113-B323-E0222FAD4CCE")]
    // normally these subclasses are treated as the base class (i.e. the 
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if 
    //  we add 'ComVisible = false', then it doesn't get the registry 
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.TecEncConverter' which is the basis of 
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)]
	public class IcuRegexEncConverter : EncConverter
    {
        #region DLLImport Statements
        // On Linux looks for libIcuRegexEC.so (adds lib- and -.so)
		[DllImport("IcuRegexEC", EntryPoint = "IcuRegexEC_Initialize", CallingConvention = CallingConvention.Cdecl)]
        static extern int CppInitialize (
            [MarshalAs(UnmanagedType.LPStr)] string strConverterSpec);

		[DllImport("IcuRegexEC", EntryPoint = "IcuRegexEC_DoConvert", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int CppDoConvert(
            byte* lpInputBuffer, int nInBufLen,
            byte* lpOutputBuffer, int *npOutBufLen);
        #endregion DLLImport Statements

        #region Member Variable Definitions
        public const string strDisplayName = "ICU Regular Expression";
        public const string strHtmlFilename = "ICU Regular Expression Plug-in About box.htm";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public IcuRegexEncConverter() : base (
            typeof(IcuRegexEncConverter).FullName,EncConverters.strTypeSILicuRegex)
        {
        }

        /// <summary>
        /// The class destructor. </summary>
        ~IcuRegexEncConverter()
        {
            Unload();
        }

        public override void Initialize(
            string converterName,
            string converterSpec,
            ref string lhsEncodingID,
            ref string rhsEncodingID,
            ref ConvType conversionType,
            ref Int32 processTypeFlags,
            Int32 codePageInput,
            Int32 codePageOutput,
            bool bAdding)
        {
            System.Diagnostics.Debug.WriteLine("IcuRegex EC Initialize BEGIN");
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

/*
            // the only thing we want to add (now that the convType can be less than accurate) 
            //  is to make sure it's unidirectional
            switch(conversionType)
            {
                case ConvType.Legacy_to_from_Legacy:
                    conversionType = ConvType.Legacy_to_Legacy;
                    break;
                case ConvType.Legacy_to_from_Unicode:
                    conversionType = ConvType.Legacy_to_Unicode;
                    break;
                case ConvType.Unicode_to_from_Legacy:
                    conversionType = ConvType.Unicode_to_Legacy;
                    break;
                //case ConvType.Unicode_to_from_Unicode:
                //    conversionType = ConvType.Unicode_to_Unicode;
                //    break;
                default:
                    break;
            }
*/

            System.Diagnostics.Debug.WriteLine("IcuRegex EC Initialize END");
        }
        #endregion Initialization

        #region Misc helpers
        protected bool IsFileLoaded()
        { 
            return false;
        }

        protected void Unload()
        { 
        }

        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16 on Windows.
            // Probably UTF8 for Linux?
            //return EncodingForm.UTF16;
            return EncodingForm.UTF8String;
        }

        protected unsafe void Load(string strExpression)
        {
            System.Diagnostics.Debug.WriteLine("IcuRegex Load BEGIN");
            //this.strFilepath = strExpression;

            if( IsFileLoaded() ) {
                //Unload();
                return;
            }

            if (!strExpression.Contains("->"))
            {
                throw new Exception (
                    "The ICU regular expression:\n\n'" + strExpression +
                    "'\n\ndoesn't contain -> ");
            }

            System.Diagnostics.Debug.WriteLine("Calling CppInitialize");
            int status = 0;
            try {
                status = CppInitialize(strExpression);
            } catch (DllNotFoundException exc) {
                throw new Exception("Failed to load .so file. Check path.");
            } catch (EntryPointNotFoundException exc) {
                throw new Exception("Failed to find function in .so file.");
            }
            if( status != 0 )  
            {
                throw new Exception("CppInitialize failed.");
            }
            System.Diagnostics.Debug.WriteLine("IcuRegex Load END");
        }
        #endregion Misc helpers

        #region Abstract Base Class Overrides
        protected override void PreConvert
            (
            EncodingForm        eInEncodingForm,
            ref EncodingForm    eInFormEngine,
            EncodingForm        eOutEncodingForm,
            ref EncodingForm    eOutFormEngine,
            ref NormalizeFlags  eNormalizeOutput,
            bool                bForward
            ) 
        {
	        // let the base class do it's thing first
            base.PreConvert( eInEncodingForm, ref eInFormEngine,
							eOutEncodingForm, ref eOutFormEngine,
							ref eNormalizeOutput, bForward);

	        // The CC DLL (conversion engine) usually works in UTF8 for Unicode. As a future 
	        //	enhancement, it might be possible to get a (marked) value from the repository
	        //	telling us what form to use (which would be UTF8Bytes by default and could be
	        //	something else if the user developed a UTF32 cc table--using the xYYYY syntax
	        //	rather than the uXXXX syntax). But for now, assume that all CC tables that 
	        //	use Unicode want UTF8.
            if( NormalizeLhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                // returning this value will cause the input Unicode data (of any form, UTF16, BE, etc.)
                //	to be converted to UTF8 narrow bytes before calling DoConvert.
                eInFormEngine = EncodingForm.UTF8Bytes;
            }
            else
            {
                // legacy
                eInFormEngine = EncodingForm.LegacyBytes;
            }

            if( NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                eOutFormEngine = EncodingForm.UTF8Bytes;
            }
            else
            {
                eOutFormEngine = EncodingForm.LegacyBytes;
            }

            // do the load at this point.
            Load(ConverterIdentifier);
        }

        [CLSCompliant(false)]
        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
            int status = 0;
            fixed(int* pnOut = &rnOutLen)
            {
                status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
            }
            if( status != 0 )  
            {
                EncConverters.ThrowError(ErrStatus.Exception, "CppDoConvert() failed.");
            }
        }

        protected override string   GetConfigTypeName
        {
            get { return typeof(IcuRegexEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides
    }
}
