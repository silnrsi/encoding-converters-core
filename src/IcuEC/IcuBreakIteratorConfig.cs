using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
	public class IcuBreakIteratorConfig : EncConverterConfig
    {
        public IcuBreakIteratorConfig()
            : base
            (
            typeof(IcuBreakIteratorEncConverter).FullName,
            IcuBreakIteratorEncConverter.CstrDisplayName,
            IcuBreakIteratorEncConverter.CstrHtmlFilename,
            ProcessTypeFlags.DontKnow
            )
            {
            }

        public override bool Configure
        (
        IEncConverters theEcs,
        string strFriendlyName,
        ConvType eConversionType,
        string strLhsEncodingID,
        string strRhsEncodingID
        )
        {
			var strTestData = "พักหลังๆนี่เวลาแก๊นจะตัดสินใจซื้ออะไรซักอย่างที่มันมีราคา จะคิดแล้วคิดอีก อย่างน้อยก็ทิ้งเวลาไว้ตั้งแต่";
			var form = new IcuBreakIteratorAutoConfigDialog(
				theEcs, m_strDisplayName, m_strFriendlyName,
				m_strConverterID, m_eConversionType,
				EncConverters.strDefUnicodeEncoding,
				EncConverters.strDefUnicodeEncoding,
				m_lProcessType, m_bIsInRepository, strTestData);

			return Configure(form);
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
            InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
                ref eConversionType, ref strTestData);

			strTestData = "พักหลังๆนี่เวลาแก๊นจะตัดสินใจซื้ออะไรซักอย่างที่มันมีราคา จะคิดแล้วคิดอีก อย่างน้อยก็ทิ้งเวลาไว้ตั้งแต่";

            var form = new IcuBreakIteratorAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
