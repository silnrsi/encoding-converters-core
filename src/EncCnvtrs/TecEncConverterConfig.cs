using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;   // for the class attributes
using ECInterfaces;                     // for IEncConverter

//uncomment the following line for verbose debugging output using Console.WriteLine
//#define VERBOSE_DEBUGGING

namespace SilEncConverters40
{
    public class TecEncConverterConfig : EncConverterConfig
    {
        public TecEncConverterConfig()
            : base
            (
            typeof(TecEncConverter).FullName,
            TecEncConverter.strDisplayName,
            TecEncConverter.strHtmlFilename,
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
            Console.WriteLine("TecEncConverterConfig(3).Configure BEGIN");
#endif
			TecAutoConfigDialog form = new TecAutoConfigDialog(aECs, m_strDisplayName, m_strFriendlyName,
                m_strConverterID, m_eConversionType, m_strLhsEncodingID, m_strRhsEncodingID,
                m_lProcessType, m_bIsInRepository);

#if VERBOSE_DEBUGGING
            Console.WriteLine("TecEncConverterConfig.Configure END");
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
            Console.Error.WriteLine("TecEncConverterConfig.DisplayTestPage() BEGIN");
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

            TecAutoConfigDialog form = new TecAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
