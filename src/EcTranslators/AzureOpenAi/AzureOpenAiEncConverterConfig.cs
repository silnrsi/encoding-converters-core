using System;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
    public class AzureOpenAiEncConverterConfig : EncConverterConfig
    {
        public AzureOpenAiEncConverterConfig()
            : base
            (
            typeof(AzureOpenAiEncConverter).FullName,
            AzureOpenAiEncConverter.strDisplayName,
            AzureOpenAiEncConverter.strHtmlFilename,
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
            Console.WriteLine("AzureOpenAiEncConverterConfig(3).Configure BEGIN");
#endif
            AzureOpenAiAutoConfigDialog form = new AzureOpenAiAutoConfigDialog(aECs, m_strDisplayName, m_strFriendlyName,
                m_strConverterID, m_eConversionType, m_strLhsEncodingID, m_strRhsEncodingID,
                m_lProcessType, m_bIsInRepository);

#if VERBOSE_DEBUGGING
            Console.WriteLine("AzureOpenAiEncConverterConfig.Configure END");
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
            Console.Error.WriteLine("AzureOpenAiEncConverterConfig.DisplayTestPage() BEGIN");
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

            AzureOpenAiAutoConfigDialog form = new AzureOpenAiAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
