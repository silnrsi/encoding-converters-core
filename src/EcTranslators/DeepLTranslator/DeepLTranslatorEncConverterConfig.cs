using System;
using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40.EcTranslators.DeepLTranslator
{
	public class DeepLTranslatorEncConverterConfig : EncConverterConfig
    {
        public DeepLTranslatorEncConverterConfig()
            : base
            (
            typeof(DeepLTranslatorEncConverter).FullName,
            DeepLTranslatorEncConverter.CstrDisplayName,
            DeepLTranslatorEncConverter.strHtmlFilename,
            ProcessTypeFlags.Translation
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
            Console.WriteLine("DeepLTranslatorEncConverterConfig(3).Configure BEGIN");
#endif
			DeepLTranslatorAutoConfigDialog form = new DeepLTranslatorAutoConfigDialog(aECs, m_strDisplayName, m_strFriendlyName,
                m_strConverterID, m_eConversionType, m_strLhsEncodingID, m_strRhsEncodingID,
                m_lProcessType, m_bIsInRepository);

#if VERBOSE_DEBUGGING
            Console.WriteLine("DeepLTranslatorEncConverterConfig.Configure END");
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
            Console.Error.WriteLine("DeepLTranslatorEncConverterConfig.DisplayTestPage() BEGIN");
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

            DeepLTranslatorAutoConfigDialog form = new DeepLTranslatorAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
