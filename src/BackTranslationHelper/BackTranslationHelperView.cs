using ECInterfaces;
using SilEncConverters40;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BackTranslationHelper
{
    /// <summary>
    /// This is an attempt to create a user control that uses the SIL.Windows.Forms.HtmlBrowser.XWebBrowser to 
    /// display the BT data (cf. BackTranslationHelperCtrl.cs as the .Net/WinForms implementation). It's currently
    /// showing the data, but I really don't like the way it looks (css would help, no doubt), but if you want to
    /// use it on an OS that doesn't support normal WinForm controls, it's a starting point).
    /// </summary>
    public partial class BackTranslationHelperView : UserControl
    {
        #region Member variables
        public IBackTranslationHelperDataSource BackTranslationHelperDataSource;
        public List<IEncConverter> TheTranslators = new List<IEncConverter>();
        public bool _displayExistingTargetTranslation;
        public BackTranslationHelperModel _model;
        private string _tempPath;
        #endregion

        public BackTranslationHelperView()
        {
            InitializeComponent();
        }


        public void Initialize(bool displayExistingTargetTranslation)
        {
            _displayExistingTargetTranslation = displayExistingTargetTranslation;

            BackTranslationHelperDataSource.SetDataUpdateProc(UpdateData);

            // see how many converters are configured (if none, then query for one)
            if (!TheTranslators.Any())
            {
                var aTranslator = QueryTranslator();
                TheTranslators.Add(aTranslator.GetEncConverter);
            }
        }


        public static DirectableEncConverter QueryTranslator()
        {
            DirectableEncConverter theEc;
            try
            {
                var aEc = DirectableEncConverter.EncConverters.AutoSelectWithTitle(ConvType.Unicode_to_Unicode,
                                                                                   "Select the Encoding Converter to do the Translation (e.g. a Bing or DeepL translator)");
                if (aEc == null)
                    throw new ApplicationException("Unable to proceed if you don't select a Translator/EncConverter resource");

                theEc = new DirectableEncConverter(aEc);
            }
            catch (Exception)
            {
                throw;
            }

            return theEc;
        }

        public List<string> NewTargetTexts
        {
            get
            {
                // always choose the one in the text box
                var newTargetTexts = new List<string> { _model.TargetDataEditable };
                return newTargetTexts;
            }

            set
            {
                if (TheTranslators.Count == 1)
                {
                    System.Diagnostics.Debug.Assert(value.Count == 1);
                    _model.TargetDataEditable = value.FirstOrDefault();
                }
                else
                {
                    var possibleTargetTranslations = _model.TargetsPossible;
                    System.Diagnostics.Debug.Assert(value.Count == TheTranslators.Count);
                    System.Diagnostics.Debug.Assert(possibleTargetTranslations.Take(value.Count).ToList().All(pt => pt.FillButtonEnabled));

                    for (var i = 0; i < TheTranslators.Count; i++)
                    {
                        var label = possibleTargetTranslations[i];
                        label.TargetData = value[i];
                    }
                }
            }
        }

        #region Event handlers
        public void GetNewData(ref BackTranslationHelperModel model)
        {
            if (model == null)
                _model = BackTranslationHelperDataSource.Model;

            else if (_model != model)
                _model = model;

            for (var i = _model.TargetsPossible.Count; i < TheTranslators.Count; i++)
            {
                var theTranslator = TheTranslators[i];
                var translatedText = theTranslator.Convert(_model.SourceData);
                _model.TargetsPossible.Add(new TargetPossible { TargetData = translatedText, FillButtonEnabled = true, PossibleIndex = i, TranslatorName = theTranslator.Name });
            }

            model = _model;
        }

        public void UpdateData(BackTranslationHelperModel model)
        {
            // some clients (i.e. Word) only pass 1 translated target text (bkz it only knows about 1 EncConverter/Translator)
            //  if we have fewer than the number of possible target translations (i.e. we added 1 or more addl Translators),
            //  then convert the missing ones to add to this collection
            var displayPossibleTargetTranslations = TheTranslators.Count > 1;
            var numOfTranslators = TheTranslators.Count;
            for (var i = model.TargetsPossible.Count; i < numOfTranslators; i++)
            {
                var theTranslator = TheTranslators[i];
                var translatedText = theTranslator.Convert(model.SourceData);
                if (displayPossibleTargetTranslations)
                {
                    var targetPossible = new TargetPossible
                    {
                        TranslatorName = theTranslator.Name,
                        TargetData = translatedText,
                        FillButtonEnabled = true
                    };
                    model.TargetsPossible.Add(targetPossible);
                }
            }

            var html = _model.Html(BackTranslationHelperDataSource.SourceLanguageFont, BackTranslationHelperDataSource.TargetLanguageFont);
            if (_tempPath == null)
                _tempPath = SIL.IO.TempFile.WithExtension("htm").Path;
            System.IO.File.WriteAllText(_tempPath, html);
            xWebBrowser.Url = new Uri(_tempPath);
        }

        #endregion

        private void buttonWriteTextToTarget_Click(object sender, System.EventArgs e)
        {
            /*todo
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.WriteToTarget);
            BackTranslationHelperDataSource.Log($"change target text from '{labelTargetTextExisting.Text}' to '{textBoxTargetBackTranslation.Text}'");
            BackTranslationHelperDataSource.WriteToTarget(textBoxTargetBackTranslation.Text);
            */
        }

        private void buttonCopyToClipboard_Click(object sender, System.EventArgs e)
        {
            /*todo
            Clipboard.SetDataObject(textBoxTargetBackTranslation.Text);
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.Copy);
            */
        }

        private void buttonNextSection_Click(object sender, System.EventArgs e)
        {
            /*todo
            if (BackTranslationHelperDataSource == null)
            {
                // see if we can manually trigger the change of verse number in the host
                return;
            }
            BackTranslationHelperDataSource.ButtonPressed(ButtonPressed.MoveToNext);
            var existingTargetText = labelTargetTextExisting.Text;
            var newTargetText = textBoxTargetBackTranslation.Text;
            var modified = existingTargetText != newTargetText;

            if (!autoSaveToolStripMenuItem.Checked)
            {
                if (modified)
                {
                    var res = MessageBox.Show($"Do you want to save/write out the translated text before moving to the next portion?", BackTranslationHelperDataSource.ProjectName, MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Cancel)
                    {
                        BackTranslationHelperDataSource.Cancel();
                        return;
                    }
                    if (res == DialogResult.No)
                    {
                        BackTranslationHelperDataSource.MoveToNext();
                        return;
                    }
                }
            }

            if (modified)
            {
                BackTranslationHelperDataSource.Log($"change target text from '{existingTargetText}' to '{newTargetText}'");
                BackTranslationHelperDataSource.WriteToTarget(newTargetText);
            }

            BackTranslationHelperDataSource.MoveToNext();
            */
        }

        private void changeEncConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addEncConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newTranslator = QueryTranslator();
            TheTranslators.Add(newTranslator.GetEncConverter);
            Initialize(_model.TargetDataExistingEnabled);
            UpdateData(_model);
        }
    }
}
