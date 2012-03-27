using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ECInterfaces;                     // for IEncConverter
using System.Runtime.InteropServices;   // for DllImport

namespace SilEncConverters40
{
    //[CLSCompliantAttribute(false)]  // because of GeckoWebBrowser
    public partial class CpAutoConfigDialog : SilEncConverters40.AutoConfigDialog
    {
        //const string cstrDefaultCodePageToSelect = "65001"; // make it UTF-8 by default (the most likely choice)
        const string cstrDefaultCodePageToSelect = "1200"; // make it UTF-16 by default (the most likely choice)

        EncodingInfo[] m_encInfos = Encoding.GetEncodings();

        public CpAutoConfigDialog
            (
            IEncConverters aECs,
            string strDisplayName,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strLhsEncodingId,
            string strRhsEncodingId,
            int lProcessTypeFlags,
            bool bIsInRepository
            )
        {
            InitializeComponent();

            base.Initialize
            (
            aECs,
            CpEncConverter.strHtmlFilename,
            strDisplayName,
            strFriendlyName,
            strConverterIdentifier,
            eConversionType,
            strLhsEncodingId,
            strRhsEncodingId,
            lProcessTypeFlags,
            bIsInRepository
            );

            m_bQueryForConvType = false; // the converter determines this itself.

            if (m_encInfos != null)
                foreach (EncodingInfo encInfo in m_encInfos)
                {
                    // String str = String.Format("CodePage: {0}, DisplayName: {1}, Name: {2}", encInfo.CodePage, encInfo.DisplayName, encInfo.Name);
                    String str = Convert.ToString(encInfo.CodePage, 10);
                    comboBoxCodePageList.Items.Add(str);
                }

            // if we're editing ...
            if (m_bEditMode)
            {
                int nIndex = comboBoxCodePageList.Items.IndexOf(ConverterIdentifier);
                if (nIndex != -1)
                {
                    comboBoxCodePageList.SelectedIndex = nIndex;
                    IsModified = false;
                }
            }
            else
            {
                int nIndex = comboBoxCodePageList.Items.IndexOf(cstrDefaultCodePageToSelect);
                if (nIndex != -1)
                    comboBoxCodePageList.SelectedIndex = nIndex;
            }
        }

        public CpAutoConfigDialog
            (
            IEncConverters aECs,
            string strFriendlyName,
            string strConverterIdentifier,
            ConvType eConversionType,
            string strTestData
            )
        {
            InitializeComponent();

            base.Initialize
            (
            aECs,
            strFriendlyName,
            strConverterIdentifier,
            eConversionType,
            strTestData
            );
        }

        // this method is called either when the user clicks the "Apply" or "OK" buttons *OR* if she
        //  tries to switch to the Test or Advanced tab. This is the dialog's one opportunity
        //  to make sure that the user has correctly configured a legitimate converter.
        protected override bool OnApply()
        {
            // for CodePage, get the converter identifier from the selected Index of the combo box.
            ConverterIdentifier = (string)comboBoxCodePageList.SelectedItem;

            if (String.IsNullOrEmpty(ConverterIdentifier))
                return false;

            // if we're actually on the setup tab, then give the exact error.
            if (tabControl.SelectedTab == tabPageSetup)
            {
                // only do these message boxes if we're on the Setup tab itself, because if this OnApply
                //  is being called as a result of the user switching to the Test tab, that code will
                //  already put up an error message and we don't need two error messages.
                try
                {
                    if (String.IsNullOrEmpty(ConverterIdentifier))
                        throw new Exception("You must choose a code page!");

                    int nCodePage = Convert.ToInt32(ConverterIdentifier);
                }
                catch
                {
                    MessageBox.Show(this, "You must choose a code page!", EncConverters.cstrCaption);
                    return false;
                }
            }

            return base.OnApply();
        }

        protected override string ProgID
        {
            get { return typeof(CpEncConverter).FullName; }
        }

        protected override string ImplType
        {
            get { return EncConverters.strTypeSILcp; }
        }

        protected override string DefaultFriendlyName
        {
            // as the default, make it the same as the table name (w/o extension)
            get
            {
                return String.Format("CP_{0}", ConverterIdentifier);
            }
        }

        /*
            typedef struct _cpinfoexW {
                UINT    MaxCharSize;                    // max length (in bytes) of a char
                BYTE    DefaultChar[MAX_DEFAULTCHAR];   // default character (MB)
                BYTE    LeadByte[MAX_LEADBYTES];        // lead byte ranges
                WCHAR   UnicodeDefaultChar;             // default character (Unicode)
                UINT    CodePage;                       // code page id
                WCHAR   CodePageName[MAX_PATH];         // code page name (Unicode)
            } CPINFOEXW, *LPCPINFOEXW;
         * 
         *  BOOL
            WINAPI
            GetCPInfoExW(
                __in UINT          CodePage,
                __in DWORD         dwFlags,
                __out LPCPINFOEXW  lpCPInfoEx);
            #ifdef UNICODE
            #define GetCPInfoEx  GetCPInfoExW
        */
        public struct CPINFOEX
        {
            public Int32 MaxCharSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] DefaultChar;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] LeadByte;
            public short UnicodeDefaultChar;
            public Int32 CodePage;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
            public short[] CodePageName;
        }

        private void comboBoxCodePageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsModified = true;
            string strCodePage = (string)comboBoxCodePageList.SelectedItem;
            int nCodePage = 0;
            try
            {
                nCodePage = Convert.ToInt32(strCodePage);
            }
            catch
            {
                return;
            }

            System.Diagnostics.Debug.Assert((comboBoxCodePageList.SelectedIndex >= 0) && (comboBoxCodePageList.SelectedIndex < m_encInfos.Length));
            EncodingInfo encInfo = m_encInfos[comboBoxCodePageList.SelectedIndex];
            System.Diagnostics.Debug.Assert(nCodePage == encInfo.CodePage);

            string strInfo = String.Format("Code Page: {1}{0}Display Name: {2}{0}Registered Name: {3}",
                Environment.NewLine,
                encInfo.CodePage,
                encInfo.DisplayName,
                encInfo.Name);
            textBoxCodePageDetails.Text = strInfo;
        }

        protected string ShortArrayToString(short[] ash)
        {
            // count the number of shorts in the array which are non-zero
            int i = 0;
            while (ash[i] != 0)
                i++;   // noop

            char[] ach = new char[i];
            i = 0;
            while (ash[i] != 0)
            {
                ach[i] = (char)ash[i];
                i++;
            }

            return new string(ach);
        }

        protected int StringLengthFromByteArray(byte[] aby)
        {
            // count the number of bytes in the array which are non-zero
            int i = 0;
            while (aby[i] != 0)
                i++;

            return i;
        }
    }
}

