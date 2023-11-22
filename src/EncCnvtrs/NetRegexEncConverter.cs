// Created by Jim Kornelsen on Dec 6 2011
// 28-Nov-11 JDK  Wrap Perl expression and write in temp file, rather than requiring input file.
//
using System;
using System.Diagnostics;
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

		public bool TransliteratorInitialized { get; set; }

		// the regular express definining the Converter identifier:
		//	{<findWhat>}->{<replaceWith>};<RegexOptions as an int>
		// e.g.
		//  {[aeiou]}->{V};1
		// (meaing: for convert latin vowels to 'V' and make it case insensitive. That is, 1 = RegexOption.IgnoreCase)
		private static readonly Regex _reParseConverterIdentifier = new("{(.+)}->{(.*)};?(.*)");
		private Regex _reConverter;
		private string _replaceWith;

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
								out string strFindWhat, out string strReplaceWith, out RegexOptions options)
		{
			var match = _reParseConverterIdentifier.Match(strConverterSpec);
			if (match.Success)
			{
				var strOptions = match.Groups[3].Value;
				if (String.IsNullOrEmpty(strOptions))
				{
					options = RegexOptions.None;
				}
				else
				{
					options = (RegexOptions)int.Parse(strOptions);
				}

				strFindWhat = match.Groups[1].Value;
				strReplaceWith = match.Groups[2].Value;

				return true;
			}
			else
			{
				options = RegexOptions.None;
				strFindWhat = strReplaceWith = null;
				return false;
			}
		}

		protected unsafe void Load(string converterIdentifier)
		{
			Util.DebugWriteLine(this, "BEGIN");

			if (TransliteratorInitialized)
				return;

			if (!ParseConverterIdentifier(converterIdentifier, out string findWhat, out string replaceWith, out RegexOptions options))
			{
				throw new ApplicationException($"{DisplayName} not properly configured! converterName: {converterIdentifier} (should be in the format {{<findWhat>}}->{{<replaceWith>}};<RegexOptions as Int>)");
			}

			_replaceWith = replaceWith;
			_reConverter = new Regex(findWhat, options | RegexOptions.Compiled);

			TransliteratorInitialized = true;  // so we don't to do this with each call to Convert

			Util.DebugWriteLine(this, "END");
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

			var strOutput = _reConverter.Replace(strInput, _replaceWith);

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
