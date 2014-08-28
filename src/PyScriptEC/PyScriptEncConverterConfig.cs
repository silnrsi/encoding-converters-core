// Created by Jim Kornelsen on Nov 14 2011
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;   // for the class attributes
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    public class PyScriptEncConverterConfig : EncConverterConfig
    {
        public PyScriptEncConverterConfig()
            : base
            (
            typeof(PyScriptEncConverter).FullName,
            PyScriptEncConverter.strDisplayName,
            PyScriptEncConverter.strHtmlFilename,
            ProcessTypeFlags.DontKnow
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
            var form = new PyScriptAutoConfigDialog (
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

            PyScriptAutoConfigDialog form = new PyScriptAutoConfigDialog(aECs, strFriendlyName,
                strConverterIdentifier, eConversionType, strTestData);

            base.DisplayTestPage(form);
        }
    }
}
