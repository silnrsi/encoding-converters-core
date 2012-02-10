// Created by Jim Kornelsen on Dec 3 2011
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;                  // for RegistryKey
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// Managed ICU transliterator EncConverter
    /// </summary>
    //[GuidAttribute("54E0185D-3603-4113-B323-E0222FAD4CCE")]
    // normally these subclasses are treated as the base class (i.e. the 
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if 
    //  we add 'ComVisible = false', then it doesn't get the registry 
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.TecEncConverter' which is the basis of 
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)] 
	public class IcuTranslitEncConverter : EncConverter
    {
        #region DLLImport Statements
        // On Linux looks for libIcuTranslitEC.so (adds lib- and -.so)
        [DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_Initialize")]
        static extern unsafe int CppInitialize (
            [MarshalAs(UnmanagedType.LPStr)] string strConverterID);

        [DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_DoConvert")]
        static extern unsafe int CppDoConvert(
            byte* lpInputBuffer, int nInBufLen,
            byte* lpOutputBuffer, int *npOutBufLen);

		[DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_ConverterNameList_start")]
		static extern unsafe int CppConverterNameList_start();

		[DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_ConverterNameList_next")]
		static extern unsafe string CppConverterNameList_next();

		[DllImport("IcuTranslitEC", EntryPoint="IcuTranslitEC_GetDisplayName")]
		static extern unsafe string CppGetDisplayName(string strID);
        #endregion DLLImport Statements

        #region Member Variable Definitions
        public const string strDisplayName = "ICU Transliterator";
        public const string strHtmlFilename = "ICU Transliterators Plug-in About box.htm";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public IcuTranslitEncConverter() : base (
            typeof(IcuTranslitEncConverter).FullName,EncConverters.strTypeSILicuTrans)
        {
        }

        /// <summary>
        /// The class destructor. </summary>
        ~IcuTranslitEncConverter()
        {
            //if( IsFileLoaded() )
            //    CCUnloadTable(m_hTable);
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
            System.Diagnostics.Debug.WriteLine("IcuTranslit EC Initialize BEGIN");
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

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
                case ConvType.Unicode_to_from_Unicode:
                    conversionType = ConvType.Unicode_to_Unicode;
                    break;
                default:
                    break;
            }
            System.Diagnostics.Debug.WriteLine("IcuTranslit EC Initialize END");
        }

        #endregion Initialization

        #region Misc helpers
        protected bool IsFileLoaded()
        { 
            //return (m_hTable != 0);
            return false;
        }

        protected void Unload()
        { 
            //System.Diagnostics.Debug.WriteLine("CcEncConverter.Unload");
            //if( IsFileLoaded() )
            //{
            //    CCUnloadTable(m_hTable);
            //    m_hTable = 0;
            //}
        }

        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16
            return EncodingForm.UTF16;
        }

        protected unsafe void Load(string strTranslitID)
        {
            System.Diagnostics.Debug.WriteLine("IcuTranslit Load BEGIN");

            if( IsFileLoaded() )
                Unload();

            System.Diagnostics.Debug.WriteLine("Calling CppInitialize");
            int status = 0;
            try {
                status = CppInitialize(strTranslitID);
            } catch (DllNotFoundException exc) {
                throw new Exception("Failed to load .so file. Check path.");
            } catch (EntryPointNotFoundException exc) {
                throw new Exception("Failed to find function in .so file.");
            }
            if( status != 0 )  
            {
                throw new Exception("CppInitialize failed.");
            }
            System.Diagnostics.Debug.WriteLine("IcuTranslit Load END");
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
	        // let the base class do its thing first
            base.PreConvert( eInEncodingForm, ref eInFormEngine,
							eOutEncodingForm, ref eOutFormEngine,
							ref eNormalizeOutput, bForward);

            if( NormalizeLhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                if (ECNormalizeData.IsUnix)
                {
                    // returning this value will cause the input Unicode data (of any form, UTF16, BE, etc.)
                    //	to be converted to UTF8 narrow bytes before calling DoConvert.
                    eInFormEngine = EncodingForm.UTF8Bytes;
                }
                else
                {
                    eInFormEngine = EncodingForm.UTF16;
                }
            }
            else
            {
                // legacy
                eInFormEngine = EncodingForm.LegacyBytes;
            }

            // Output will be stored in a typical C# string, so eOutFormEngine will be UTF16,
            // even though the Perl script is writing UTF8 bytes to output.
            if( NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                if (ECNormalizeData.IsUnix)
                {
                    eOutFormEngine = EncodingForm.UTF8Bytes;
                }
                else
                {
                    eOutFormEngine = EncodingForm.UTF16;
                }
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
            System.Diagnostics.Debug.WriteLine("IcuTranslitEC.DoConvert BEGIN()");
            int status = 0;
            fixed(int* pnOut = &rnOutLen)
            {
                status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
            }
            if( status != 0 )  
            {
                EncConverters.ThrowError(ErrStatus.Exception, "CppDoConvert() failed.");
            }
            System.Diagnostics.Debug.WriteLine("IcuTranslitEC.DoConvert END()");
        }

        protected override string   GetConfigTypeName
        {
            get { return typeof(IcuTranslitConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides
		
		#region Additional public methods to access the C++ DLL.
		/// <summary>
		/// Gets the available ICU transliterator specifications.
		/// </summary>
		public static unsafe List<string> GetAvailableConverterSpecs()
		{
			int count = CppConverterNameList_start();
			List<string> specs = new List<string>(count);
			for (int i = 0; i < count; ++i)
				specs.Add( CppConverterNameList_next() );
			return specs;
		}
		
		/// <summary>
		/// Gets the display name of the given ICU transliterator specification.
		/// </summary>
		public static unsafe string GetDisplayName(string spec)
		{
			return CppGetDisplayName(spec);
		}
		#endregion
    }
}
