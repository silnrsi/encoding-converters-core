using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ECInterfaces;
using ICU4NET;

namespace SilEncConverters40
{
    /// <summary>
    /// IcuBreakIteratorEncConverter implements the EncConverter interface to provide a 
    /// wrapper for the break iterator project at: http://code.google.com/p/icu4net/
    /// which itself is a wrapper around ICU break iterator code at:
    /// http://userguide.icu-project.org/boundaryanalysis#TOC-BreakIterator-Boundary-Analysis-Exa
    /// </summary>
    [GuidAttribute("5265D30D-7402-4D69-A620-D1D1611CAD4A")]
    public sealed class IcuBreakIteratorEncConverter : EncConverter
    {
        #region Member Variable Definitions

        public const string CstrImplementationType = "ICU.BreakIterator";
        public const string CstrDisplayName = "ICU Boundary Analysis/Break Iterator";
        public const string CstrHtmlFilename = "ICU Boundary Analysis-Break Iterator Converter About box.mht";

		public const string DefaultSeparator = " ";

        private bool _bForward;
        private BreakIterator _breakIterator;
        #endregion Member Variable Definitions

        #region Initialization
        public IcuBreakIteratorEncConverter()
            : base(typeof(IcuBreakIteratorEncConverter).FullName, CstrImplementationType)
        {
            int nType = 0;
            string dontknow = null;
            var convType = ConvType.Unicode_to_from_Unicode;
            Initialize(CstrDisplayName, DefaultSeparator, ref dontknow, ref dontknow, ref convType, ref nType, 0, 0, false);
        }
        #endregion Initialization

        #region Abstract Base Class Overrides

        protected override string GetConfigTypeName
        {
            get
            {
                return typeof(IcuBreakIteratorConfig).AssemblyQualifiedName;
            }
        }

        protected override void PreConvert(EncodingForm eInEncodingForm, ref EncodingForm eInFormEngine, EncodingForm eOutEncodingForm, ref EncodingForm eOutFormEngine, ref NormalizeFlags eNormalizeOutput, bool bForward)
        {
            base.PreConvert(eInEncodingForm, ref eInFormEngine, eOutEncodingForm, ref eOutFormEngine, ref eNormalizeOutput, bForward);

            _bForward = bForward;

            if (!IsLoaded)
                Load();
        }

        private Regex _regexForMandarin = new Regex("[3200-D7AF]");

        protected override unsafe void DoConvert(byte* lpInBuffer, int nInLen, byte* lpOutBuffer, ref int rnOutLen)
        {
            // we need to put it *back* into a string for the lookup
            // [aside: I should probably override base.InternalConvertEx so I can avoid having the base 
            //  class version turn the input string into a byte* for this call just so we can turn around 
            //  and put it *back* into a string for our processing... but I like working with a known 
            //  quantity and no other EncConverter does it that way. Besides, I'm afraid I'll break smtg ;-]
            var baIn = new byte[nInLen];
            ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);
            var caIn = Encoding.Unicode.GetChars(baIn);

            // here's our input string
            var strInput = new string(caIn);

            string strOutput = null;
            if (_bForward)
            {
                var bySpace = strInput.Split(DefaultSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                _breakIterator.SetText(strInput);
                var words = _breakIterator.Enumerate().ToList();
                if (bySpace.Length == words.Count)
                {
                    // it didn't do anything!
                    // if it is mandarin, this is probably expected and we can do this
                    if (_regexForMandarin.IsMatch(strInput))
                    {
                        strOutput = bySpace
                            .SelectMany(word => word)
                            .Aggregate<char, string>(null, (current, ch) => current + (ch + ConverterIdentifier));
                    }
                    else
                    {
                        strOutput = strInput;
                    }
                }
                else
                {
                    int nNumWords = words.Count - 1;
                    for (var i = 0; i < nNumWords; i++)
                    {
                        var word = words[i];
                        if (!String.IsNullOrEmpty(word) && (word != ConverterIdentifier))
                            strOutput += words[i] + ConverterIdentifier;
                    }
                    strOutput += words.Last();
                }
            }
            else
            {
                strOutput = strInput.Replace(ConverterIdentifier, null);
            }

            if (String.IsNullOrEmpty(strOutput))
                return;

            var nLen = strOutput.Length * 2;
            if (nLen > rnOutLen)
                EncConverters.ThrowError(ErrStatus.OutputBufferFull);
            rnOutLen = nLen;
            ECNormalizeData.StringToByteStar(strOutput, lpOutBuffer, rnOutLen, false);
        }
		#endregion Abstract Base Class Overrides

		#region Misc Helpers

		private bool IsLoaded
        {
            get
            {
                return (_breakIterator != null);
            }
        }

        private void Load()
        {
            _breakIterator = BreakIterator.CreateWordInstance(Locale.GetUS());
        }
        #endregion Misc Helpers
    }

	public static class Extension
	{
		public static IEnumerable<string> Enumerate(this BreakIterator bi)
		{
			var sb = new StringBuilder();
			string text = bi.GetCLRText();
			int start = bi.First(), end = bi.Next();
			while (end != BreakIterator.DONE)
			{
				yield return text.Substring(start, end - start);
				start = end; end = bi.Next();
			}
		}
	}
}
