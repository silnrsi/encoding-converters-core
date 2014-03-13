// Created by Jim Kornelsen on Dec 6 2011
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;   // for the class attributes
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    public class IcuRegexEncConverterConfig : EncConverterConfig
    {
        public IcuRegexEncConverterConfig()
            : base
            (
            typeof(IcuRegexEncConverter).FullName,
            IcuRegexEncConverter.strDisplayName,
            IcuRegexEncConverter.strHtmlFilename,
            ProcessTypeFlags.ICURegularExpression
            )
            {
            }

        public override bool Configure
        (
        IEncConverters aECs,
        string strFriendlyName,
        ConvType eConversionType,
        string strLhsEncodingID,
        string strRhsEncodingID
        )
        {
            var form = new IcuRegexAutoConfigDialog (
                aECs, m_strDisplayName, m_strFriendlyName,
                m_strConverterID, m_eConversionType,
                BestGuessEncoding(m_strLhsEncodingID, strLhsEncodingID, EncConverters.strDefUnicodeEncoding),
                BestGuessEncoding(m_strRhsEncodingID, strRhsEncodingID, EncConverters.strDefUnicodeEncoding), 
                m_lProcessType, m_bIsInRepository);

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

            IcuRegexAutoConfigDialog form = new IcuRegexAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
