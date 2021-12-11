using System;
using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40.EcTranslators
{
	public class BingTranslatorEncConverterConfig : EncConverterConfig
    {
        public BingTranslatorEncConverterConfig()
            : base
            (
            typeof(BingTranslatorEncConverter).FullName,
            BingTranslatorEncConverter.CstrDisplayName,
            BingTranslatorEncConverter.strHtmlFilename,
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
            Console.WriteLine("BingTranslatorEncConverterConfig(3).Configure BEGIN");
#endif
			BingTranslatorAutoConfigDialog form = new BingTranslatorAutoConfigDialog(aECs, m_strDisplayName, m_strFriendlyName,
                m_strConverterID, m_eConversionType, m_strLhsEncodingID, m_strRhsEncodingID,
                m_lProcessType, m_bIsInRepository);

#if VERBOSE_DEBUGGING
            Console.WriteLine("BingTranslatorEncConverterConfig.Configure END");
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
            Console.Error.WriteLine("BingTranslatorEncConverterConfig.DisplayTestPage() BEGIN");
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

            BingTranslatorAutoConfigDialog form = new BingTranslatorAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
