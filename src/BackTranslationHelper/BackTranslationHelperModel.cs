using System;
using System.Collections.Generic;

namespace BackTranslationHelper
{
	[Serializable]
	public class BackTranslationHelperModel
    {
		/// <summary>
		/// The data from the original source that is to be translated from
		/// </summary>
        public string SourceData { get; set; }

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
