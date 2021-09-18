using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ECInterfaces;
using static SilEncConverters40.WebBrowserAdaptor;

namespace SilEncConverters40
{
    /// <summary>
    /// TechHindiSiteEncConverter implements the EncConverter interface to provide a 
    /// wrapper for the web-page based converter available at the Technical Hindi
    /// google group: http://groups.google.com/group/technical-hindi/files
    /// 
    /// These web pages have Java script code to convert legacy encodings to Unicode
    /// which I'm "borrowing" to do the conversion.
    /// 
    /// The identifier for this EncConverter is:
    ///     <uri to file>;
    ///     <id of input (legacy) textarea>;
    ///     <id of output (unicode) textarea>;
    ///     <name of function to do conversion>;
    ///     (<name of function to do reverse conversion>)
    /// </summary>
    [GuidAttribute("0F218E35-EA40-4d56-9FFF-322094B4F412")]
    public class TechHindiSiteEncConverter : EncConverter
    {
        #region Member Variable Definitions

        public const string CstrImplementationType = "SIL.TechHindiWebPage";
        public const string CstrDisplayName = "Technical Hindi (Google group) Html Converter";
        public const string CstrHtmlFilename = "Technical_Hindi_(Google_group)_Html_Converter_Plug-in_About_box.mht";

        protected string ConverterPageUri;
        protected string InputHtmlElementId;
        protected string OutputHtmlElementId;
        protected string ConvertFunctionName;
        protected string ConvertReverseFunctionName;
		protected WhichBrowser WebBrowserType;

        private WebBrowserAdaptor _webBrowser;

        private bool _bForward;
        #endregion Member Variable Definitions

        #region Initialization
        public TechHindiSiteEncConverter()
            : base(typeof(TechHindiSiteEncConverter).FullName, CstrImplementationType)
        {
        }

        public override void Initialize(string converterName, string converterSpec, ref string lhsEncodingID, ref string rhsEncodingID, ref ECInterfaces.ConvType conversionType, ref int processTypeFlags, int codePageInput, int codePageOutput, bool bAdding)
        {
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding);

            if (!ParseConverterIdentifier(converterSpec, out ConverterPageUri, out InputHtmlElementId,
                out OutputHtmlElementId, out ConvertFunctionName, out ConvertReverseFunctionName, out WebBrowserType))
            {
                throw new ApplicationException(String.Format("{0} not properly configured!", CstrDisplayName));
            }

			// if we don't have a reverse function, then don't allow it to be 'to_from'
			if (String.IsNullOrEmpty(ConvertReverseFunctionName))
			{
				m_eConversionType = conversionType = MakeUniDirectional(conversionType);
			}
		}

        internal static bool ParseConverterIdentifier(string converterSpec, out string strConverterPageUri, 
            out string strInputHtmlElementId, out string strOutputHtmlElementId,
            out string strConvertFunctionName, out string strConvertReverseFunctionName, out WhichBrowser browserType)
        {
            strConverterPageUri = strInputHtmlElementId = strOutputHtmlElementId =
                strConvertFunctionName = strConvertReverseFunctionName = null;
			browserType = WhichBrowser.Undefined;	// means figure it out from the registry and OS type

			string[] astrs = converterSpec.Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries);
            if (astrs.Length < 4)
                 return false;

            strConverterPageUri = astrs[0];
            strInputHtmlElementId = astrs[1];
            strOutputHtmlElementId = astrs[2];
            strConvertFunctionName = astrs[3];
			if (astrs.Length == 5)
			{
				if (!Enum.TryParse(astrs[4], out browserType))
					strConvertReverseFunctionName = astrs[4];
			}
			else if(astrs.Length == 6)
			{
				strConvertReverseFunctionName = astrs[4];
				Enum.TryParse(astrs[5], out browserType);
			}
			return true;
        }

		#endregion Initialization

		#region Abstract Base Class Overrides

		protected override string GetConfigTypeName
        {
            get { return typeof(TechHindiSiteConfig).AssemblyQualifiedName; }
        }

        protected override void PreConvert(EncodingForm eInEncodingForm, ref EncodingForm eInFormEngine, EncodingForm eOutEncodingForm, ref EncodingForm eOutFormEngine, ref NormalizeFlags eNormalizeOutput, bool bForward)
        {
            base.PreConvert(eInEncodingForm, ref eInFormEngine, eOutEncodingForm, ref eOutFormEngine, ref eNormalizeOutput, bForward);

            _bForward = bForward;

            if (!IsLoaded)
                Load();
        }

        protected override unsafe void DoConvert(byte* lpInBuffer, int nInLen, byte* lpOutBuffer, ref int rnOutLen)
        {
            // we need to put it *back* into a string for the lookup
            // [aside: I should probably override base.InternalConvertEx so I can avoid having the base 
            //  class version turn the input string into a byte* for this call just so we can turn around 
            //  and put it *back* into a string for our processing... but I like working with a known 
            //  quantity and no other EncConverter does it that way. Besides, I'm afraid I'll break smtg ;-]
            byte[] baIn = new byte[nInLen];
            ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);
            string strOutput = null;
            Encoding enc;
            bool bInputLegacy = ((_bForward &&
                                  (NormalizeLhsConversionType(ConversionType) == NormConversionType.eLegacy))
                                 ||
                                 (!_bForward &&
                                  (NormalizeRhsConversionType(ConversionType) == NormConversionType.eLegacy)));

            if (bInputLegacy)
            {
                try
                {
                    enc = Encoding.GetEncoding(CodePageInput);
                }
                catch
                {
                    enc = Encoding.GetEncoding(EncConverters.cnIso8859_1CodePage);
                }
            }
            else
                enc = Encoding.Unicode;

            char[] caIn = enc.GetChars(baIn);

            // here's our input string
            string strInput = new string(caIn);

			strOutput = ConvertAsync(strInput, strOutput).Result;

			StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
		}

		private async Task<string> ConvertAsync(string strInput, string strOutput)
		{
			if (_bForward)
			{
				try
				{
					await _webBrowser.SetInnerTextAsync(InputHtmlElementId, strInput);
#if DebugWithMessageBoxes
					MessageBox.Show($"strInput: {strInput}, innerText: {innerText}");
#endif // DebugWithMessageBoxes

					await _webBrowser.ExecuteScriptFunctionAsync(ConvertFunctionName);
					strOutput = await _webBrowser.GetInnerTextAsync(OutputHtmlElementId);
				}
				catch (Exception ex)
				{
					ShowExceptionMessage(ex);
				}
			}
			else
			{
				try
				{
					await _webBrowser.SetInnerTextAsync(OutputHtmlElementId, strInput);
					await _webBrowser.ExecuteScriptFunctionAsync(ConvertReverseFunctionName);
					strOutput = await _webBrowser.GetInnerTextAsync(InputHtmlElementId);
				}
				catch (Exception ex)
				{
					ShowExceptionMessage(ex);
				}
			}

			if (String.IsNullOrEmpty(strOutput))
				strOutput = "\ufffd";   // pass something back!

			return strOutput;
		}

		#endregion Abstract Base Class Overrides

		#region Misc Helpers
		protected bool IsLoaded
        {
            get
            {
                return
                    !( (_webBrowser == null)
                    || (String.IsNullOrEmpty(ConvertFunctionName)));
            }
        }

        protected void Load()
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterPageUri));
            if (_webBrowser == null)
            {
                _webBrowser = CreateBrowser(WebBrowserType);
				_webBrowser.Initialize();
            }

            _webBrowser.Navigate(ConverterPageUri);
        }

        protected unsafe void StringToProperByteStar(string strOutput, byte* lpOutBuffer, ref int rnOutLen)
        {
            // if the output is legacy, then we need to shrink it from wide to narrow
            if ((_bForward && NormalizeRhsConversionType(ConversionType) == NormConversionType.eLegacy)
                || (!_bForward && NormalizeLhsConversionType(ConversionType) == NormConversionType.eLegacy))
            {
                byte[] baOut = EncConverters.GetBytesFromEncoding(CodePageOutput, strOutput, true);

                if (baOut.Length > rnOutLen)
                    EncConverters.ThrowError(ErrStatus.OutputBufferFull);
                rnOutLen = baOut.Length;
                ECNormalizeData.ByteArrToByteStar(baOut, lpOutBuffer);
            }
            else
            {
                int nLen = strOutput.Length * 2;
                if (nLen > (int)rnOutLen)
                    EncConverters.ThrowError(ErrStatus.OutputBufferFull);
                rnOutLen = nLen;
                ECNormalizeData.StringToByteStar(strOutput, lpOutBuffer, rnOutLen, false);
            }
        }
		public static void ShowExceptionMessage(Exception ex)
		{
			string msg = "Could not call script: " + ex.Message;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
				msg += $"{Environment.NewLine}because: (InnerException): {ex.Message}";
			}

			MessageBox.Show(msg);
		}
		#endregion Misc Helpers
	}
}
