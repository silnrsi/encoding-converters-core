using System;
using System.Collections.Generic;

namespace BackTranslationHelper
{
	[Serializable]
	public class BackTranslationHelperModel
    {
		/// <summary>
		/// The data from the original source that goes into the Source Data (readonly) text box.
		/// This version may be partial snippets of text (e.g. iso-formatted runs of text in Word
		/// or scripture text from Paratext that may be broken up by footnotes or other meta data)
		/// that doesn't necessarily make the best for translation. This string will be used for the
		/// Translator's Convert method only if SourceDataAlternate is null.
		/// </summary>
		public string SourceData { get; set; }

		/// <summary>
		/// The data from the original source that is to be translated from (unless it is null,
		/// in which case, the SourceData member will be translated). This generally represents
		/// complete sentences that are best for translation.
		/// </summary>
		public string SourceDataAlternate { get; set; }

		/// <summary>
		/// Call this to get the source text to translate using the Translator EncConverters
		/// </summary>
		public string SourceToTranslate
		{
			get { return SourceDataAlternate ?? SourceData; }
		}

		/// <summary>
		/// The one or more translations/conversions from the SourceData field value via the one or more EncConverter Translators
		/// </summary>
		public List<TargetPossible> TargetsPossible { get; set; }

		/// <summary>
		/// The converted/translated text that the user can edit and accept to replace the existing text in the source document
		/// (e.g. when used in Word), OR put into the target project document (e.g. in Paratext). In the latter case, this value
		/// may be the existing text from the target project (if it exists) or pre-filled in from the first of the TargetsPossible.
		/// </summary>
		public string TargetData { get; set; }

		/// <summary>
		/// This is set to true if there was originally some data already existing in the target project, which causes us
		/// to keep it in another field (as well as the editable field associated w/ the TargetData property) in case the
		/// user wants to revert to it.
		/// </summary>
        public string TargetDataPreExisting { get; set; }
    }

	[Serializable]
	public class TargetPossible
    {
		/// <summary>
		/// The 0-based index of the translator/EncConverter
		/// </summary>
        public int PossibleIndex { get; set; }

		/// <summary>
		/// The EncConverter/Translator's Name property
		/// </summary>
        public string TranslatorName { get; set; }

		/// <summary>
		/// The translation/conversion of the SourceData value thru the EncConverter with the TranslatorName Name property
		/// </summary>
        public string TargetData { get; set; }
    }
}
