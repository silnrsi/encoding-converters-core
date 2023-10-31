using System;
using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40.EcTranslators.NllbTranslator
{
	public class NllbTranslatorEncConverterConfig : EncConverterConfig
    {
        public NllbTranslatorEncConverterConfig()
            : base
            (
            typeof(NllbTranslatorEncConverter).FullName,
            NllbTranslatorEncConverter.CstrDisplayName,
            NllbTranslatorEncConverter.strHtmlFilename,
            ProcessTypeFlags.DontKnow
            )
            {
            }

        [STAThread]
        public override bool Configure
        (
        IEncConverters aECs,
        string strFriendlyName,
        ConvType eConversionType,
        string strLhsEncodingID,
        string strRhsEncodingID
        )
        {
#if VERBOSE_DEBUGGING
            Console.WriteLine("NllbTranslatorEncConverterConfig(3).Configure BEGIN");
#endif
			NllbTranslatorAutoConfigDialog form = new NllbTranslatorAutoConfigDialog(aECs, m_strDisplayName, m_strFriendlyName,
                m_strConverterID, m_eConversionType, m_strLhsEncodingID, m_strRhsEncodingID,
                m_lProcessType, m_bIsInRepository);

#if VERBOSE_DEBUGGING
            Console.WriteLine("NllbTranslatorEncConverterConfig.Configure END");
#endif
			return base.Configure(form);
        }

        public override void DisplayTestPage
            (
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData
            )
        {
            Console.Error.WriteLine("NllbTranslatorEncConverterConfig.DisplayTestPage() BEGIN");
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

            NllbTranslatorAutoConfigDialog form = new NllbTranslatorAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
