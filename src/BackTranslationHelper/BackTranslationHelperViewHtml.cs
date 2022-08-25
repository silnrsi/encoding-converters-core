using System;
using System.Drawing;

namespace BackTranslationHelper
{
    public static class BackTranslationHelperViewHtml
    {
        public const string StyleClassNameLabels = "LabelStyle";
        public const string StyleClassNameSourceData = "SourceDataStyle";
        public const string StyleClassNameTargetData = "TargetDataStyle";
        public const string TargetTranslationTextAreaId = "TargetText";

        public const string CstrButtonPrefixFill = "btnFill";

        public static string Html(this BackTranslationHelperModel model, Font sourceLanguageFont, Font targetLanguageFont)
        {
            var html = String.Format(Properties.Resources.HTML_TableRow, FormatLanguageColumn(StyleClassNameSourceData, model.SourceData));
                
            if (model.TargetDataExistingEnabled)
                html += String.Format(Properties.Resources.HTML_TableRow, FormatLanguageColumn(StyleClassNameTargetData, model.TargetDataExisting));

            foreach (var targetPossible in model.TargetsPossible)
                html += targetPossible.Html();

            html += String.Format(Properties.Resources.HTML_TableRow, FormatLanguageColumnHtml(model.TargetDataEditable));

            html = String.Format(Properties.Resources.HTML_Table, html);

            return AddHtmlDocOutside(html, sourceLanguageFont, targetLanguageFont);
        }

        public static string AddHtmlDocOutside(string innerHtml, Font sourceLanguageFont, Font targetLanguageFont)
        {
            return String.Format(Properties.Resources.BackTranslationHelperHtmlPage,
                                 null, // Properties.Resources.jquery_3_6_0_min,
                                 StylePrefix(sourceLanguageFont, targetLanguageFont),
                                 innerHtml);
        }

        public static string StylePrefix(Font sourceLanguageFont, Font targetLanguageFont)
        {
            string strLangStyles = null;
            strLangStyles += HtmlStyle(StyleClassNameSourceData, sourceLanguageFont.Name, sourceLanguageFont.SizeInPoints, Color.Blue, false);
            strLangStyles += HtmlStyle(StyleClassNameTargetData, targetLanguageFont.Name, targetLanguageFont.SizeInPoints, Color.Green, false);

            return String.Format(Properties.Resources.HTML_StyleDefinition,
                                 targetLanguageFont.SizeInPoints,
                                 strLangStyles);
        }

        public static string HtmlStyle(string fieldType, string fontName, float fontPointSize, Color fontColor, bool isRtl)
        {
            string strHtmlStyle = String.Format(Properties.Resources.HTML_LangStyle,
                                                fieldType,
                                                fontName,
                                                fontPointSize,
                                                HtmlColor(fontColor),
                                                (isRtl) ? "rtl" : "ltr",
                                                (isRtl) ? "right" : "left");

            return strHtmlStyle;
        }

        public static string HtmlColor(Color clrRow)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}",
                                 clrRow.R, clrRow.G, clrRow.B);
        }

        public static string FormatLanguageColumnHtml(string value)
        {
            string htmlElement = String.Format(Properties.Resources.HTML_Textarea,
                                               TargetTranslationTextAreaId,
                                               StyleClassNameTargetData,
                                               value);

            return String.Format(Properties.Resources.HTML_TableCell,
                                 htmlElement);
        }

        public static string FormatLanguageColumn(string styleClassName, string value)
        {
            return String.Format(Properties.Resources.HTML_TableCell,
                                 String.Format(Properties.Resources.HTML_ParagraphText,
                                               styleClassName,
                                               value));
        }

        public static string ButtonId(int nTranslatorIndex)
        {
            return String.Format("{0}_{1}", CstrButtonPrefixFill, nTranslatorIndex);
        }

        public static string GetFillButtonHtml(int nTranslatorIndex, string translatorName)
        {
            return String.Format(Properties.Resources.HTML_ButtonImageToolTip,
                                 ButtonId(nTranslatorIndex),
                                 translatorName,
                                 "return OnFillTargetEditBox(this);",
                                 "Click to copy this option to the edit box below",
                                 "FillDownHS.png");
        }

        public static string Html(this TargetPossible model)
        {
            if (!model.FillButtonEnabled)
                return null;

            var row = FormatLanguageColumn(StyleClassNameLabels, model.TranslatorName);
            row += FormatLanguageColumn(StyleClassNameTargetData, model.TargetData);
            row += GetFillButtonHtml(model.PossibleIndex, model.TranslatorName);
            return String.Format(Properties.Resources.HTML_TableRow, row);
        }
    }
}
