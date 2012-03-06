using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// Managed Code Page EncConverter
    /// </summary>
    //[GuidAttribute("F91EBC49-1019-4ff3-B143-A84E6081A472")]
    // normally these subclasses are treated as the base class (i.e. the 
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if 
    //  we add 'ComVisible = false', then it doesn't get the registry 
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.TecEncConverter' which is the basis of 
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)] 
    public class CpEncConverter : EncConverter
    {
        #region Member Variable Definitions
        private const int   CP_UTF8  = 65001;
        private const int   CP_UTF16 = 1200;

        private int     m_nCodePage;    // the code page to convert with
        private bool    m_bToWide;      // we have to keep track of the direction since it might be different than m_bForward
        
        public const string strDisplayName = "Code Page Converter";
        public const string strHtmlFilename = "Code Page Converter Plug-in About box.mht";
        #endregion Member Variable Definitions

        #region Initialization
        public CpEncConverter() : base(typeof(CpEncConverter).FullName,EncConverters.strTypeSILcp)
        {
        }

        public override void Initialize(string converterName, string converterSpec, 
            ref string lhsEncodingID, ref string rhsEncodingID, ref ConvType conversionType, 
            ref Int32 processTypeFlags, Int32 codePageInput, Int32 codePageOutput, bool bAdding)
        {
            //Console.WriteLine("Cp EC Initialize BEGIN");
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );
            m_nCodePage = System.Convert.ToInt32(ConverterIdentifier);
            
            // the UTF16 code page is a special case: ConvType must be Uni <> Uni
            if( m_nCodePage == CP_UTF16 )
            {
                if( String.IsNullOrEmpty(LeftEncodingID) )
                    lhsEncodingID = m_strLhsEncodingID = "utf-16";
                if( String.IsNullOrEmpty(RightEncodingID) )
                    rhsEncodingID = m_strRhsEncodingID = EncConverters.strDefUnicodeEncoding;
                
                // This only works if we consider UTF16 to be unicode; not legacy
                conversionType = m_eConversionType = ConvType.Unicode_to_from_Unicode;
            }

            // the rest are all "UnicodeEncodingConversion"s (by definition). Also, use the word 
            // "UNICODE" concatenated to the legacy encoding name
            else 
            {
                processTypeFlags = m_lProcessType |= (int)ProcessTypeFlags.UnicodeEncodingConversion;
                if( String.IsNullOrEmpty(RightEncodingID) )
                {
                    rhsEncodingID = m_strRhsEncodingID = EncConverters.strDefUnicodeEncoding;
                    if( !String.IsNullOrEmpty(lhsEncodingID) )
                        rhsEncodingID += " " + lhsEncodingID;
                }
            }

            // if it wasn't set above, then it's Legacy <> Unicode
            if( conversionType == ConvType.Unknown )
                conversionType = m_eConversionType = ConvType.Legacy_to_from_Unicode;

            // finally, it is also a "Code Page" conversion process type
            processTypeFlags = m_lProcessType |= (int)ProcessTypeFlags.CodePageConversion;
            //Console.WriteLine("Cp EC Initialize END");
        }

        protected override EncodingForm DefaultUnicodeEncForm(bool bForward, bool bLHS)
        { 
            // C# wants UTF-16, generally speaking.
            return EncodingForm.UTF16; 
        }
        #endregion Initialization

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

            // we have to know what the forward flag state is (and we can't use m_bForward because
            //  that might be different (e.g. if this was called from ConvertEx).
            m_bToWide = bForward;
            if (!IsLegacyFormat(eInEncodingForm) && IsLegacyFormat(eOutEncodingForm))
                m_bToWide = !bForward;

            // check if this is the special UTF8 code page, and if so, request that the engine
            //  form be UTF8Bytes (this is the one code page converter where both sides are 
            //  Unicode.
            if( m_bToWide )
            {
                // going "to wide" means the output form required by the engine is UTF16.
                eOutFormEngine = EncodingForm.UTF16;

                if (m_nCodePage == CP_UTF8 && !IsLegacyFormat(eInEncodingForm))
                    eInFormEngine = EncodingForm.UTF8Bytes;
                else
                    eInFormEngine = EncodingForm.LegacyBytes;
            }
            else
            {
                // going "from wide" means the input form required by the engine is UTF16.
                eInFormEngine = EncodingForm.UTF16;

                if (m_nCodePage == CP_UTF8 && !IsLegacyFormat(eOutEncodingForm))
                    eOutFormEngine = EncodingForm.UTF8Bytes;
                else if (IsLegacyFormat(eOutEncodingForm))
                    eOutFormEngine = EncodingForm.LegacyBytes;
            }
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
			byte[] baIn = new byte[nInLen];
			ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);
			System.Diagnostics.Debug.WriteLine(String.Format("Starting with {0} bytes.", baIn.Length));
			byte[] baOut;
			if (m_bToWide)
			{
				// Perform the conversion from one encoding to the other.
				Encoding encFrom = Encoding.GetEncoding(m_nCodePage);
				Encoding encTo   = Encoding.Unicode;
				baOut = Encoding.Convert(encFrom, encTo, baIn);
			}
			else
			{
				Encoding encFrom = Encoding.Unicode;
				Encoding encTo   = Encoding.GetEncoding(m_nCodePage);
				baOut = Encoding.Convert(encFrom, encTo, baIn);
			}
			System.Diagnostics.Debug.WriteLine(String.Format("Converted to {0} bytes.", baOut.Length));
			if (baOut.Length > 0)
				rnOutLen = Marshal.SizeOf(baOut[0]) * baOut.Length;
			else
				rnOutLen = 0;
			Marshal.Copy(baOut, 0, (IntPtr)lpOutBuffer, baOut.Length);
			Marshal.WriteByte((IntPtr)lpOutBuffer, rnOutLen, 0); // nul terminate
        }

        protected override string   GetConfigTypeName
        {
            get { return typeof(CpEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides

		#region Additional public methods to access the C++ DLL.
		static Dictionary<string, EncodingInfo> m_mapNameEncoding = new Dictionary<string, EncodingInfo>();

		/// <summary>
		/// Gets the available CodePage converter specifications.
		/// </summary>
		public static List<string> GetAvailableConverterSpecs()
		{
			var encodings = Encoding.GetEncodings();
			m_mapNameEncoding.Clear();
			var count = encodings.Length;
			var specs = new List<string>(count);
			for (var i = 0; i < count; ++i)
			{
				var name = encodings[i].CodePage.ToString();
				specs.Add(name);
				m_mapNameEncoding.Add(name, encodings[i]);
				//Console.WriteLine("Converter[{0}]: name = '{1}', displayname = '{2}', codepage = {3}",
				//	i, encodings[i].Name, encodings[i].DisplayName, encodings[i].CodePage);
			}
			return specs;
		}
		
		/// <summary>
		/// Gets the display name of the given CodePage converter.
		/// </summary>
		public static string GetDisplayName(string spec)
		{
			EncodingInfo encoding;
			if (m_mapNameEncoding.TryGetValue(spec, out encoding))
				return encoding.DisplayName;
			return null;
		}
		#endregion
	}
}
