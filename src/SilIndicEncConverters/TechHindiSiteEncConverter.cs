using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public const string CstrHtmlFilename = "Technical_Hindi_(Google_group)_Html_Converter_Plug-in_About_box.htm";

        protected string ConverterPageUri;
        protected string InputHtmlElementId;
        protected string OutputHtmlElementId;
        protected string ConvertFunctionName;
        protected string ConvertReverseFunctionName;
		protected WhichBrowser WebBrowserType;

        private WebBrowserAdaptor _webBrowser;

        private bool _bForward;
        #endregion Member Variable Definitions

        /// <summary>See EncConverter.RiskyNativeDependencies. This converter drives a live third-party
        /// page in-process via WebBrowserAdaptor/WebView2, whose WebView2Loader.dll loses to a host
        /// process's own already-loaded copy (confirmed root cause of the "Unable to cast to
        /// ICoreWebView2Environment" failure seen running inside Word -- see Handover.md, "Generic
        /// subprocess fallback for host-process conflicts"). Merged with (not replacing) the base
        /// class's config-sourced list, so the installed *.config file can still flag additional risky
        /// DLLs for this converter too, on top of the one already known in code.</summary>
        public override IEnumerable<string> RiskyNativeDependencies =>
            base.RiskyNativeDependencies.Concat(new[] { "WebView2Loader.dll" });

        #region Initialization
        public TechHindiSiteEncConverter()
            : base(typeof(TechHindiSiteEncConverter).FullName, CstrImplementationType)
        {
			// this was needed for the Bing Translator to hit the Azure Translator (https call) endpoint from Word. There's some permutation of
			//  this enc Converter to not work from w/in word (which are html based)... so I'm just wondering if this is needed here too
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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

        /// <summary>Mirrors WebBrowserAdaptor.CreateBrowser's own resolution of WhichBrowser.Undefined,
        /// without actually creating anything -- used by PreConvert below to decide, before Load() ever
        /// runs, whether this call would end up driving Edge/WebView2.</summary>
        private bool WillUseEdgeBrowser =>
            WebBrowserType == WhichBrowser.Edge ||
            (WebBrowserType == WhichBrowser.Undefined
                && WebBrowserEdgeInfo.ShouldUseBrowser
                && WebBrowserEdgeInfo.IsWebView2RuntimeInstalled);

        protected override void PreConvert(EncodingForm eInEncodingForm, ref EncodingForm eInFormEngine, EncodingForm eOutEncodingForm, ref EncodingForm eOutFormEngine, ref NormalizeFlags eNormalizeOutput, bool bForward)
        {
            _bForward = bForward;

            if (!IsLoaded)
            {
                // Edge/WebView2 specifically needs to be kept out of this (real host) process
                // altogether, not just caught-and-retried after the fact: a live dotnet-dump capture
                // proved ICoreWebView2Environment.CreateCoreWebView2Controller can hang with no way to
                // ever observe completion (see WebBrowserEdge's WebView2InitializationTimeoutSeconds doc
                // comment), and reproducing that timeout -- with or without an explicit Dispose() of the
                // abandoned control -- crashed the *whole* process moments later during native
                // window-class cleanup ("Failed to unregister class Chrome_WidgetWin_0"), even on a
                // clean run with no other WebView2 activity, and even though the timeout itself was
                // caught and a retried conversion via the subprocess fallback succeeded. In other words,
                // catching the failure isn't enough to protect the host process -- so when a subprocess
                // route actually exists, Edge is deferred there proactively, before ever touching
                // WebView2 in-process, rather than only reactively after a failed attempt (which is what
                // EncConverter.RiskyNativeDependencies/Convert()'s catch-and-retry still provide for
                // every other, well-behaved risky dependency, and remain the fallback here too if no
                // subprocess route is available -- see the IsSubprocessFallbackAvailable check below).
                // EngageSubprocessFallback() itself refuses to re-engage (returns false) when this code
                // is already running inside EncConverterHostExe.exe (see
                // EncConverters.IsRunningAsSubprocessHost), which is both how recursion is avoided and
                // why the check below skips straight to Load() in that one process -- the isolated place
                // it's actually safe to attempt this for real.
                if (WillUseEdgeBrowser
                    && !EncConverters.IsRunningAsSubprocessHost
                    && EncConverters.IsSubprocessFallbackAvailable
                    && EngageSubprocessFallback())
                {
                    // deliberately thrown, not just returned from: EncConverter.Convert()/ConvertEx()'s
                    // own catch clauses (see their doc comments) are what actually retry via
                    // _fallbackProxy -- this just needs to unwind out of PreConvert/InternalConvertEx
                    // before DoConvert ever runs against a _webBrowser that Load() never created.
                    throw new ApplicationException(
                        "Deferring WebView2 (Edge) initialization to the isolated EncConverterHostExe " +
                        "subprocess rather than risking it in this process directly.");
                }

                // this is what actually loads (and, per the RiskyNativeDependencies override above,
                // potentially collides on) WebView2Loader.dll -- deliberately called BEFORE
                // base.PreConvert() rather than after (the more common ordering elsewhere in this
                // codebase), specifically so that if this throws, the exception propagates out of THIS
                // method before base.PreConvert() ever runs its WasJustLoaded-gated conflict check:
                // there's nothing to usefully check yet at that point anyway, and
                // EncConverter.Convert()/ConvertEx() already catch exactly this exception one level up
                // (see their own doc comments) to engage the subprocess fallback and retry the very same
                // call. base.PreConvert() only needs to run afterward for the case where this succeeds
                // without throwing but still leaves a foreign copy of a declared-risky dependency
                // resident (see CheckForNativeDependencyConflictAfterLoad).
                Load();
            }

            base.PreConvert(eInEncodingForm, ref eInFormEngine, eOutEncodingForm, ref eOutFormEngine, ref eNormalizeOutput, bForward);
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
					Application.DoEvents();
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
