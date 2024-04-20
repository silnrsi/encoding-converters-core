using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;   // for the class attributes
using System.Text;                      // for ASCIIEncoding
using System.Threading.Tasks;
using ECInterfaces;                     // for ConvType
using SilEncConverters40;
using Paratext.PluginInterfaces;
using System.Text.RegularExpressions;
using SilEncConverters40.EcTranslators;

namespace SilEncConverters40.PtxConverters
{
    /// <summary>
    /// Managed Paratext Project EncConverter.
    /// </summary>
#if X64
	[GuidAttribute("034F26A4-1827-4ADD-815B-593956C5EC8C")]
#else
    // Ptx doesn't have an x86 version, and this doesn't work except as a Ptx plugin, so... noop
#endif
    // normally these subclasses are treated as the base class (i.e. the 
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if 
    //  we add 'ComVisible = false', then it doesn't get the registry 
    //  'HKEY_CLASSES_ROOT\SIL.ParatextBackTranslationHelperPlugin.PtxProjectEncConverter' which is the basis of 
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)] 
    public class PtxProjectEncConverter : TranslatorConverter
	{
        #region Const Definitions

		public const string DisplayName = "Paratext Project Data";
        public const string ImplementTypePtxProjectData = "SIL.PtxProjectData";

		#endregion Const Definitions

		#region Member Variable Definitions

		private IProject _project;

		[CLSCompliant(false)]
		public IProject ParatextProject
		{
			get
			{
				if (_project == null)
				{
					EncConverters.ThrowError(ErrStatus.NameNotFound, projectName);
				}
				return _project;
			}

			set { _project = value; }
		}

		/// <summary>
		/// The name of the Paratext Project from which the data will be returned
		/// </summary>
		protected string projectName;

		#endregion Member Variable Definitions

		#region Initialization
		public PtxProjectEncConverter() : base(typeof(PtxProjectEncConverter).FullName, ImplementTypePtxProjectData)
        {
		}

		public override void Initialize(string converterName, string converterSpec,
			ref string lhsEncodingID, ref string rhsEncodingID, ref ConvType conversionType,
			ref Int32 processTypeFlags, Int32 codePageInput, Int32 codePageOutput, bool bAdding)
		{
			Util.DebugWriteLine(this, $"BEGIN: {converterName}, {converterSpec}");

            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

			if (!ParseConverterIdentifier(converterSpec, ref projectName))
			{
				throw new ApplicationException($"{DisplayName} not properly configured! converterName: {converterName}");
			}

			if (conversionType == ConvType.Unknown)
				conversionType = ConvType.Unicode_to_Unicode;

			// I'm assuming that we'd have to/want to set up a different one to go the other direction
			m_eConversionType = conversionType = MakeUniDirectional(conversionType);

			if (String.IsNullOrEmpty(lhsEncodingID))
				lhsEncodingID = m_strLhsEncodingID = EncConverters.strDefUnicodeEncoding;
			if (String.IsNullOrEmpty(rhsEncodingID))
				rhsEncodingID = m_strRhsEncodingID = EncConverters.strDefUnicodeEncoding;

			Util.DebugWriteLine(this, "END");
		}

		internal static bool ParseConverterIdentifier(string converterSpec,
			ref string projectName)
		{
            projectName = null;

			string[] astrs = converterSpec.Split(new[] { ';' });

            // gotta at least have the converterSpec
            if (astrs.Length != 1)
				return false;

			if (!String.IsNullOrEmpty(astrs[0]))
                projectName = astrs[0];

			return true;
		}

		#endregion Initialization

		#region Abstract Base Class Overrides

		// instead of text data to 'convert', this project type gets a verse reference (e.g. 28_001_001;<bool>)
		//	the 'transduction' is just to query the project for the tokens in that verse (possibly accumulating the inline markers
		//	to the end if the bool part comes in as 'true') and returns the verse text as it is in the configured project
		protected Regex RegExParseParatextVerseReference = new(@"(\d{2})_(\d{3})_(\d{3})-?(\d{3})?");

		[CLSCompliant(false)]
        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
            // we need to put it *back* into a string for the rest of the processing
            // [aside: I should probably override base.InternalConvertEx so I can avoid having the base 
            //  class version turn the input string into a byte* for this call just so we can turn around 
            //  and put it *back* into a string for our processing... but I like working with a known 
            //  quantity and no other EncConverter does it that way. Besides, I'm afraid I'll break smtg ;-]
            byte[] baIn = new byte[nInLen];
            ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);

            char[] caIn = Encoding.Unicode.GetChars(baIn);

            // here's our input string
            var strInput = new string(caIn);

			string strOutput;
			if (String.IsNullOrEmpty(strInput))
			{
				strOutput = strInput;
			}
			else
			{
				var astrs = strInput.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				strInput = astrs[0];

				// Boolean indicating whether (if true) we are returning the verse text first: full verse text by paragraphs
				// followed by inline markers (e.g. footnotes), so we don't interrupt the verse scripture text with footnotes
				// as they appear in the SFM document.
				var postponeInlineMarkers = (astrs.Length > 1) ? Boolean.Parse(astrs[1]) : false;

				strOutput = CallParatextForVerseData(strInput, postponeInlineMarkers);
			}

			StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
		}

		private string CallParatextForVerseData(string strInput, bool postponeInlineMarkers)
		{
			try
			{
                // for this converter, we have to get the data from Paratext, so let's expect a verse reference specifier
                //  as we get from GetBookChapterVerseRangeKey
                // $"{verseReference.BookNum:D2}_{verseReference.ChapterNum:D3}_{verseReference.VerseNum:D3}(-{verseReference.AllVerses.Last().VerseNum:D3})"
                var match = RegExParseParatextVerseReference.Match(strInput);
                var strOutput = strInput;
                if (!match.Success)
                    return strOutput;

                var bookNumber = int.Parse(match.Groups[1].Value);
                var chapterNumber = int.Parse(match.Groups[2].Value);
                var verseNumber = int.Parse(match.Groups[3].Value);

                var tokens = ParatextProject.GetUSFMTokens(bookNumber, chapterNumber, verseNumber)?.ToList();

                var data = tokens?.OfType<IUSFMTextToken>()
                                  .Where(t => PtxPluginHelpers.IsPublishableVernacular(t, tokens) /* && PtxPluginHelpers.IsMatchingVerse(t.VerseRef, _verseReference)*/)
                                  .ToDictionary(ta => ta, ta => ta.VerseRef);

                if ((data == null) || !data.Any())
                    return $"No data found! Is it possible the requested verse doesn't exist? {strOutput}";

				if (!postponeInlineMarkers)
				{
					var textValues = data.Select(t => t.Key.Text);
					strOutput = string.Join(Environment.NewLine, textValues);
				}
				else
				{
					// this is different from the implementation in the Paratext BackTranslationHelperForm.cs in SEC
					//	(so if you have to change it, change it in both places)
					strOutput = PtxPluginHelpers.GetSourceAlternate(tokens, data.Keys.ToList());
				}

                return strOutput;
            }
            catch (Exception ex)
			{
				return LogExceptionMessage(GetType().Name, ex);
			}
		}

		#endregion Abstract Base Class Overrides

		#region Misc helpers

		protected override string GetConfigTypeName
		{
			get { return typeof(PtxProjectEncConverterConfig).AssemblyQualifiedName; }
		}

		public override bool HasUserOverriddenCredentials => true;

		#endregion Misc helpers
	}
}
