using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

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

        private int m_nCodePage;    // the code page to convert with
        private bool m_bToWide;      // we have to keep track of the direction since it might be different than m_bForward

        public const string strDisplayName = "Code Page Converter";
        public const string strHtmlFilename = "Code_Page_Converter_Plug-in_About_box.mht";
        #endregion Member Variable Definitions

        #region Initialization
        public CpEncConverter() : base(typeof(CpEncConverter).FullName, EncConverters.strTypeSILcp)
        {
        }

        public override void Initialize(string converterName, string converterSpec,
            ref string lhsEncodingID, ref string rhsEncodingID, ref ConvType conversionType,
            ref Int32 processTypeFlags, Int32 codePageInput, Int32 codePageOutput, bool bAdding)
        {
#if VERBOSE_DEBUGGING
            Console.WriteLine("Cp EC Initialize BEGIN");
#endif
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding);
            m_nCodePage = System.Convert.ToInt32(ConverterIdentifier);

            /* BE: this doesn't make any sense... why would you bother creating a UTF16-UTF16 converter?
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
            */
            if (m_nCodePage == CP_UTF8)
            {
                if (String.IsNullOrEmpty(LeftEncodingID))
                    lhsEncodingID = m_strLhsEncodingID = "utf-8";
                if (String.IsNullOrEmpty(RightEncodingID))
                    rhsEncodingID = m_strRhsEncodingID = EncConverters.strDefUnicodeEncoding;

                // This only works if we consider UTF8 to be unicode; not legacy
                conversionType = m_eConversionType = ConvType.Unicode_to_from_Unicode;
            }

            // the rest are all "UnicodeEncodingConversion"s (by definition). Also, use the word
            // "UNICODE" concatenated to the legacy encoding name
            else
            {
                processTypeFlags = m_lProcessType |= (int)ProcessTypeFlags.UnicodeEncodingConversion;
                if (String.IsNullOrEmpty(RightEncodingID))
                {
                    rhsEncodingID = m_strRhsEncodingID = EncConverters.strDefUnicodeEncoding;
                    if (!String.IsNullOrEmpty(lhsEncodingID))
                        rhsEncodingID += " " + lhsEncodingID;
                }
            }

            // if it wasn't set above, then it's Legacy <> Unicode
            if (conversionType == ConvType.Unknown)
                conversionType = m_eConversionType = ConvType.Legacy_to_from_Unicode;

            // finally, it is also a "Code Page" conversion process type
            processTypeFlags = m_lProcessType |= (int)ProcessTypeFlags.CodePageConversion;
#if VERBOSE_DEBUGGING
            Console.WriteLine("Cp EC Initialize END");
#endif
        }

        protected override EncodingForm DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // the only left-hand-side for forwarding conversion unicode possible is UTF8.
            if (bLHS)
                return bForward ? EncodingForm.UTF8String : EncodingForm.UTF16;

            // wrt. rhs
            return bForward ? EncodingForm.UTF16 : EncodingForm.UTF8String;
        }

        #endregion Initialization

        #region DLLImport Statements
        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe int MultiByteToWideChar(
            int CodePage,         // code page
            uint dwFlags,         // character-type options
            byte* lpMultiByteStr, // string to map
            int cbMultiByte,       // number of bytes in string
            char* lpWideCharStr,  // wide-character buffer
            int cchWideChar        // size of buffer
            );

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe int WideCharToMultiByte(
            int CodePage,               // code page
            uint dwFlags,               // performance and mapping flags
            char* lpWideCharStr,        // wide-character string
            int cchWideChar,            // number of chars in string
            byte* lpMultiByteStr,       // buffer for new string
            int cbMultiByte,            // size of buffer
            int lpDefaultChar,       // default for unmappable chars
            int lpUsedDefaultChar    // set when default char used
            );
        #endregion DLLImport Statements

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
            base.PreConvert(eInEncodingForm, ref eInFormEngine,
                eOutEncodingForm, ref eOutFormEngine,
                ref eNormalizeOutput, bForward);

            // we have to know what the forward flag state is (and we can't use m_bForward because
            //  that might be different (e.g. if this was called from ConvertEx).
            m_bToWide = bForward;

            // check if this is the special UTF8 code page, and if so, request that the engine
            //  form be UTF8Bytes (this is the one code page converter where both sides are
            //  Unicode.
            if (m_bToWide)
            {
                // going "to wide" means the output form required by the engine is UTF16.
                eOutFormEngine = EncodingForm.UTF16;

                if (m_nCodePage == CP_UTF8)
                    eInFormEngine = EncodingForm.UTF8Bytes;
            }
            else
            {
                // going "from wide" means the input form required by the engine is UTF16.
                eInFormEngine = EncodingForm.UTF16;

                if (m_nCodePage == CP_UTF8)
                    eOutFormEngine = EncodingForm.UTF8Bytes;
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
            if (Util.IsUnix)
            {
                byte[] baIn = new byte[nInLen];
                ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);
                Util.DebugWriteLine(this, String.Format("Starting with {0} bytes.", baIn.Length));
                byte[] baOut;
                if (m_bToWide)
                {
                    // Perform the conversion from one encoding to the other.
                    Encoding encFrom = Encoding.GetEncoding(m_nCodePage);
                    Encoding encTo = Encoding.Unicode;
                    baOut = Encoding.Convert(encFrom, encTo, baIn);
                }
                else
                {

                    Encoding encFrom = Encoding.Unicode;
                    Encoding encTo = Encoding.GetEncoding(m_nCodePage);
                    baOut = Encoding.Convert(encFrom, encTo, baIn);
                }

                Util.DebugWriteLine(this, String.Format("Converted to {0} bytes.", baOut.Length));
                if (baOut.Length > 0)
                    rnOutLen = Marshal.SizeOf(baOut[0]) * baOut.Length;
                else
                    rnOutLen = 0;
                Marshal.Copy(baOut, 0, (IntPtr)lpOutBuffer, baOut.Length);
                Marshal.WriteByte((IntPtr)lpOutBuffer, rnOutLen, 0); // nul terminate
            }
            else
            {
                if (m_bToWide)
                {
                    rnOutLen = MultiByteToWideChar(m_nCodePage, 0, lpInBuffer, nInLen, (char*)lpOutBuffer, rnOutLen / 2);
                    rnOutLen *= 2;  // sizeof(WCHAR);	// size in bytes
                }
                else
                {
                    rnOutLen = WideCharToMultiByte(m_nCodePage, 0, (char*)lpInBuffer, nInLen / 2, lpOutBuffer, rnOutLen, 0, 0);
                }
            }
        }

        protected override string GetConfigTypeName
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
#if VERBOSE_DEBUGGING
				Console.WriteLine("Converter[{0}]: name = '{1}', displayname = '{2}', codepage = {3}",
					i, encodings[i].Name, encodings[i].DisplayName, encodings[i].CodePage);
#endif
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
