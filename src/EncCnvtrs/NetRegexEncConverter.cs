// Created by Jim Kornelsen on Dec 6 2011
// 28-Nov-11 JDK  Wrap Perl expression and write in temp file, rather than requiring input file.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ECInterfaces;                     // for IEncConverter
using SilEncConverters40.Properties;

namespace SilEncConverters40
{
    /// <summary>
    /// Managed Net Regex EncConverter
    /// </summary>
	public class NetRegexEncConverter : EncConverter
    {
        #region Const Definitions
        public const string DisplayName = ".Net Regular Expression";
        public const string strHtmlFilename = "Net_Regular_Expression_Plug-in_About_box.htm";

		public const string RegexDelimiter = "->";
		public const ushort DisabledBitFlag = 0x8000;
		public const string SeparatorRegularExpressions = "\u241E";

		public bool TransliteratorInitialized { get; set; }

		// the regular express definining the Converter identifier:
		//	{<findWhat>}->{<replaceWith>};<RegexOptions as an int>
		// e.g.
		//  {[aeiou]}->{V};1
		// (meaing: for convert latin vowels to 'V' and make it case insensitive. That is, 1 = RegexOption.IgnoreCase)
		private static readonly Regex _reParseConverterIdentifier = new("{(.+?)}->{(.*?)};?(.*)");
		private List<Regex> _reConverters = new();
		private List<string> _replaceWiths = new();

		#endregion Const Definitions

		#region Initialization
		/// <summary>
		/// The class constructor. </summary>
		public NetRegexEncConverter()
			: base (typeof(NetRegexEncConverter).FullName, EncConverters.strTypeSILNetRegex)
        {
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
			Util.DebugWriteLine(this, "BEGIN");
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

			// this is the only conversion type this converter supports
			m_eConversionType = conversionType = ConvType.Unicode_to_Unicode;

			// both sides are (supposed to be) Unicode
			if (String.IsNullOrEmpty(lhsEncodingID))
				lhsEncodingID = m_strLhsEncodingID = EncConverters.strDefUnicodeEncoding;
			if (String.IsNullOrEmpty(rhsEncodingID))
				rhsEncodingID = m_strRhsEncodingID = EncConverters.strDefUnicodeEncoding;

			// not really an "ICU" regular expression, but the UI now just
			//  shows this as 'Regular Expression' and it applies to this and the ICU one.
			processTypeFlags |= (int)ProcessTypeFlags.ICURegularExpression;

			TransliteratorInitialized = false;  // so it has to be reinitialized (But only once)

			Util.DebugWriteLine(this, "END");
        }
        #endregion Initialization

        #region Misc helpers
        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
			return EncodingForm.UTF16;
		}

		internal static bool ParseConverterIdentifier(string strConverterSpec,
			out List<string> findWhats, out List<string> replaceWiths, out List<RegexOptions> regexOptions, out List<bool> disabled)
		{
			var aregexs = strConverterSpec.Split([SeparatorRegularExpressions], StringSplitOptions.RemoveEmptyEntries);
			regexOptions = new List<RegexOptions>(aregexs.Length);
			findWhats = new List<string>(aregexs.Length);
			replaceWiths = new List<string>(aregexs.Length);
			disabled = new List<bool>(aregexs.Length);

			foreach (var aregex in aregexs)
			{
				var match = _reParseConverterIdentifier.Match(aregex);
				if (match.Success)
				{
					var disabledFlag = false;
					var options = RegexOptions.None;
					var strOptions = match.Groups[3].Value;
					if (!String.IsNullOrEmpty(strOptions))
					{
						var numberStyle = NumberStyles.Integer;
						if (strOptions.StartsWith("0x"))
						{
							numberStyle = NumberStyles.HexNumber;
							strOptions = strOptions.Substring(2);	// strip off the '0x'
						}
						ushort nOptions = ushort.Parse(strOptions, numberStyle); // the high bit, if set, indicates that it's disabled
						disabledFlag = (nOptions & DisabledBitFlag) == DisabledBitFlag;
						unchecked
						{
							nOptions &= (ushort)~DisabledBitFlag;
						}
						options = (RegexOptions)nOptions;
					}
					disabled.Add(disabledFlag);
					regexOptions.Add(options);

					findWhats.Add(match.Groups[1].Value);
					replaceWiths.Add(match.Groups[2].Value);
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		protected unsafe void Load(string converterIdentifier)
		{
			Util.DebugWriteLine(this, "BEGIN");

			if (TransliteratorInitialized)
				return;

			if (!ParseConverterIdentifier(converterIdentifier,
				out List<string> findWhats, out List<string> replaceWiths, out List<RegexOptions> options, out List<bool> disable))
			{
				throw new ApplicationException($"{DisplayName} not properly configured! converterName: {converterIdentifier} (should be in the format {{<findWhat>}}->{{<replaceWith>}};<RegexOptions as Int>)");
			}

			_replaceWiths.Clear();
			_reConverters.Clear();

			for (int i = 0; i < findWhats.Count; i++)
			{
				if (disable[i])
					continue;

				var findWhat = SafeUnescape(findWhats[i]);
				_replaceWiths.Add(SafeUnescape(replaceWiths[i]));

				var reConverter = new Regex(findWhat, options[i] | RegexOptions.Compiled);
				_reConverters.Add(reConverter);
			}

			TransliteratorInitialized = true;  // so we don't to do this with each call to Convert

			Util.DebugWriteLine(this, "END");

			static unsafe string SafeUnescape(string regexString)
			{
				try
				{
					// it'll throw an exception w/ something like \s+, but not \n
					regexString = Regex.Unescape(regexString);
				}
				catch { }

				return regexString;
			}
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

            // do the load at this point.
            Load(ConverterIdentifier);
        }

        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
			// we need to put it *back* into a string for the conversion
			byte[] baIn = new byte[nInLen];
			ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);
			char[] caIn = Encoding.Unicode.GetChars(baIn);

			// here's our input string
			var strInput = new string(caIn);
			var strOutput = (!_reConverters.Any()) ? strInput : String.Empty;
			for (int i = 0; i < _reConverters.Count; i++)
			{
				var reConverter = _reConverters[i];
				var replaceWith = _replaceWiths[i];
				strInput = strOutput = reConverter.Replace(strInput, replaceWith);
			}

			StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
		}

		protected override string   GetConfigTypeName
        {
            get { return typeof(NetRegexEncConverterConfig).AssemblyQualifiedName; }
        }

		[CLSCompliant(false)]
		private static unsafe void StringToProperByteStar(string strOutput, byte* lpOutBuffer, ref int rnOutLen)
		{
			int nLen = strOutput.Length * 2;
			if (nLen > (int)rnOutLen)
				EncConverters.ThrowError(ErrStatus.OutputBufferFull);
			rnOutLen = nLen;
			ECNormalizeData.StringToByteStar(strOutput, lpOutBuffer, rnOutLen, false);
		}
        #endregion Abstract Base Class Overrides
    }
}
