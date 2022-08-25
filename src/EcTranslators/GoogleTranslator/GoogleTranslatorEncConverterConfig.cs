using System;
using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40.EcTranslators.GoogleTranslator
{
	public class GoogleTranslatorEncConverterConfig : EncConverterConfig
    {
        public GoogleTranslatorEncConverterConfig()
            : base
            (
            typeof(GoogleTranslatorEncConverter).FullName,
            GoogleTranslatorEncConverter.CstrDisplayName,
            GoogleTranslatorEncConverter.strHtmlFilename,
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
            Console.WriteLine("GoogleTranslatorEncConverterConfig(3).Configure BEGIN");
#endif
			GoogleTranslatorAutoConfigDialog form = new GoogleTranslatorAutoConfigDialog(aECs, m_strDisplayName, m_strFriendlyName,
                m_strConverterID, m_eConversionType, m_strLhsEncodingID, m_strRhsEncodingID,
                m_lProcessType, m_bIsInRepository);

#if VERBOSE_DEBUGGING
            Console.WriteLine("GoogleTranslatorEncConverterConfig.Configure END");
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
            Console.Error.WriteLine("GoogleTranslatorEncConverterConfig.DisplayTestPage() BEGIN");
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

            GoogleTranslatorAutoConfigDialog form = new GoogleTranslatorAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
