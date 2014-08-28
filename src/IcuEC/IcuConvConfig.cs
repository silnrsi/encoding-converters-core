// Created by Steve McConnel Feb 2, 2012 by copying and editing IcuTranslitConfig.cs

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;   // for the class attributes
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
	public class IcuConvEncConverterConfig : EncConverterConfig
	{
		public IcuConvEncConverterConfig()
			: base(
				typeof(IcuConvEncConverter).FullName,
				IcuConvEncConverter.strDisplayName,
				IcuConvEncConverter.strHtmlFilename,
				ProcessTypeFlags.ICUConverter)
		{
		}

		public override bool Configure(
			IEncConverters aECs,
			string strFriendlyName,
			ConvType eConversionType,
			string strLhsEncodingID,
			string strRhsEncodingID)
		{
			IcuConvAutoConfigDialog form = new IcuConvAutoConfigDialog (
				aECs, m_strDisplayName, m_strFriendlyName,
				m_strConverterID, m_eConversionType, m_strLhsEncodingID, m_strRhsEncodingID,
				m_lProcessType, m_bIsInRepository);

			return base.Configure(form);
		}

		public override void DisplayTestPage(
			IEncConverters aECs,
			string strFriendlyName,
			string strConverterIdentifier,
			ConvType eConversionType,
			string strTestData)
		{
			InitializeFromThis(ref strFriendlyName, ref strConverterIdentifier,
				ref eConversionType, ref strTestData);

			IcuConvAutoConfigDialog form = new IcuConvAutoConfigDialog(aECs, strFriendlyName,
				strConverterIdentifier, eConversionType, strTestData);

			base.DisplayTestPage(form);
		}
	}
}
