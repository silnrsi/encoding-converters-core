#define DisableBilling

using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using static SilEncConverters40.EcTranslators.NllbTranslator.NllbTranslatorEncConverter;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SilEncConverters40.EcTranslators.NllbTranslator;
using System.Net;
using Newtonsoft.Json;

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
	public partial class NllbTranslatorAutoConfigDialog : AutoConfigDialog
	{
		private readonly ComboBoxItem SourceLanguageNameMustBeConfigured = new ComboBoxItem { Display = "Select Source Language" };
		private readonly ComboBoxItem TargetLanguageNameMustBeConfigured = new ComboBoxItem { Display = "Select Target Language" };

		// protected List<ComboBoxItem> LanguagesSupported;

		public NllbTranslatorAutoConfigDialog
            (
            IEncConverters aECs,
            string strDisplayName,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strLhsEncodingId,
            string strRhsEncodingId,
            int lProcessTypeFlags,
            bool bIsInRepository
            )
        {
            Util.DebugWriteLine(this, "(1) BEGIN");
            InitializeComponent();
            Util.DebugWriteLine(this, "initialized component");
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			base.Initialize
            (
            aECs,
            strHtmlFilename,
            strDisplayName,
            strFriendlyName,
            strConverterIdentifier,
            eConversionType,
            strLhsEncodingId,
            strRhsEncodingId,
            lProcessTypeFlags,
            bIsInRepository
            );
            Util.DebugWriteLine(this, "called base.Initalize");

			var langMap = GetCapabilities().GetAwaiter().GetResult();
			if (langMap == null)
				return;

			var languagesSupported = langMap.Select(kvp => new ComboBoxItem { Code = kvp.Key, Display = kvp.Value })
											.OrderBy(c => c.Display)
											.ToList();
			InitializeSourceAndTargetLanguages(languagesSupported);

			// if we're editing converter, then set the Converter Spec and say it's unmodified
			if (m_bEditMode)
			{
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(ConverterIdentifier));

				ParseConverterIdentifier(ConverterIdentifier, out string fromLanguage, out string toLanguage);

				var selectedItem = comboBoxSourceLanguages.Items.Cast<ComboBoxItem>().FirstOrDefault(l => l.Code == fromLanguage);
				comboBoxSourceLanguages.SelectedItem = selectedItem;
				comboBoxTargetLanguages.SelectedItem = languagesSupported.FirstOrDefault(l => l.Code == toLanguage);
				IsModified = false;
			}
			else
			{
				comboBoxSourceLanguages.SelectedItem = SourceLanguageNameMustBeConfigured;
				comboBoxTargetLanguages.SelectedItem = TargetLanguageNameMustBeConfigured;
			}

			m_bInitialized = true;

			helpProvider.SetHelpString(comboBoxSourceLanguages, Properties.Resources.HelpForNllbTranslatorSourceLanguagesComboBox);
			helpProvider.SetHelpString(comboBoxTargetLanguages, Properties.Resources.HelpForNllbTranslatorTargetLanguagesComboBox);
			helpProvider.SetHelpString(buttonSetNllbTranslateApiKey, Properties.Resources.HelpForNllbTranslatorAddYourOwnApiKey);

			Util.DebugWriteLine(this, "END");
        }

		public NllbTranslatorAutoConfigDialog
            (
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData
            )
        {
            Util.DebugWriteLine(this, "(2) BEGIN");
            InitializeComponent();

            base.Initialize
            (
            aECs,
            strFriendlyName,
            strConverterIdentifier,
            eConversionType,
            strTestData
            );
            Util.DebugWriteLine(this, "END");
        }

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
			var selectedToLanguage = (ComboBoxItem)comboBoxTargetLanguages.SelectedItem;
			if ((selectedToLanguage == null) || (TargetLanguageNameMustBeConfigured == selectedToLanguage))
			{
				MessageBox.Show(this, "The Target Language must be selected!", EncConverters.cstrCaption);
				return false;
			}

			var selectedFromLanguage = (ComboBoxItem)comboBoxSourceLanguages.SelectedItem;
			if ((selectedFromLanguage == null) || (SourceLanguageNameMustBeConfigured == selectedFromLanguage))
			{
				MessageBox.Show(this, "The Source Language must be selected!", EncConverters.cstrCaption);
				return false;
			}

			// for this converter, use the source and target language codes (e.g. hin_Deva) as the converter identifier
			ConverterIdentifier = String.Format("{0};{1}",
				selectedFromLanguage.Code,
				selectedToLanguage.Code);

            return base.OnApply();
        }

		protected override string ProgID
        {
            get
            {
                return typeof(NllbTranslatorEncConverter).FullName; 
            }
        }

        protected override string ImplType
        {
            get
            {
                return EncConverters.strTypeSILNllbTranslator;
            }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get
			{
				var selectedSourceLanguage = (ComboBoxItem)comboBoxSourceLanguages.SelectedItem;
				var selectedTargetLanguage = (ComboBoxItem)comboBoxTargetLanguages.SelectedItem;
				return $"Nllb Translate {selectedSourceLanguage} to {selectedTargetLanguage}";
			}
        }

		/// <summary>
		/// Initialize the source and possibly target language combo boxes with the translation languages possible.
		/// For TranslateWithTransliteration, we only need the source language, so the 'initializeTargetLanguageAlso' parameter should be false
		/// </summary>
		/// <param name="initializeTargetLanguageAlso">true to initialize the target language combo box also</param>
		private void InitializeSourceAndTargetLanguages(List<ComboBoxItem> languagesSupported)
		{
			var items = languagesSupported.ToArray();
			comboBoxSourceLanguages.Items.Clear();
			comboBoxSourceLanguages.Items.Add(SourceLanguageNameMustBeConfigured);
			comboBoxSourceLanguages.Items.AddRange(items);

			comboBoxTargetLanguages.Items.Clear();
			comboBoxTargetLanguages.Items.Add(TargetLanguageNameMustBeConfigured);
			comboBoxTargetLanguages.Items.AddRange(items);
		}

		public class ComboBoxItem
		{
			public string Display { get; set; }
			public string Code { get; set; }
			public override string ToString()
			{
				return Display;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;
				var objAsComboBoxItem = obj as ComboBoxItem;
				return (objAsComboBoxItem?.Display == Display) || (objAsComboBoxItem?.Code == Code);
			}

			public override int GetHashCode()
			{
				int hashCode = 1075847657;
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Code);
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Display);
				return hashCode;
			}
		}

		private void ButtonSetNllbTranslateApiKey_Click(object sender, EventArgs e)
		{
#if false
#if encryptingNewCredentials
			var nllbTranslatorKeyHide = Properties.Settings.Default.NllbTranslatorCredentials;
			var credentials = EncryptionClass.Encrypt(nllbTranslatorKeyHide);
#endif
			// only send the key if it's already the override key (so we don't expose ours)
			var translatorCredentialsOverride = Properties.Settings.Default.NllbTranslatorKeyOverride;
			if (!String.IsNullOrEmpty(translatorCredentialsOverride))
				translatorCredentialsOverride = EncryptionClass.Decrypt(translatorCredentialsOverride);

			using var dlg = new QueryForNllbCredentials(translatorCredentialsOverride);
			if (dlg.ShowDialog() == DialogResult.Yes)
			{
				NllbTranslatorApiKey = dlg.TranslatorKey;
				Properties.Settings.Default.Save();
			}
#endif
		}

		private void ComboBoxSourceLanguages_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			IsModified = true;
		}

		private void ComboBoxTargetLanguages_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			IsModified = true;
		}
	}
}

