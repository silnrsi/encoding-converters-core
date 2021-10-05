// Created by Jim Kornelsen on Dec 3 2011
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;                  // for RegistryKey
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// Managed ICU transliterator EncConverter
    /// </summary>
    //[GuidAttribute("54E0185D-3603-4113-B323-E0222FAD4CCE")]
    // normally these subclasses are treated as the base class (i.e. the
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if
    //  we add 'ComVisible = false', then it doesn't get the registry
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.TecEncConverter' which is the basis of
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)]
    public class IcuTranslitEncConverter : EncConverter
    {
        #region DLLImport Statements
        // On Linux looks for libIcuTranslitEC.so (adds lib- and -.so)
        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_Initialize", CallingConvention = CallingConvention.Cdecl)]
        static extern int CppInitialize(
            [MarshalAs(UnmanagedType.LPStr)] string strConverterID);

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_DoConvert", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int CppDoConvert(
            byte* lpInputBuffer, int nInBufLen,
            byte* lpOutputBuffer, int* npOutBufLen);

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_ConverterNameList_start", CallingConvention = CallingConvention.Cdecl)]
        static extern int CppConverterNameList_start();

        static string CppConverterNameList_next()
        {
            if (Util.IsUnix)
            {
                return CppConverterNameList_next_Linux();
            }
            else
            {
                return CppConverterNameList_next_Windows();
            }
        }

        static string CppGetDisplayName(string strID)
        {
            if (Util.IsUnix)
            {
                return CppGetDisplayName_Linux(strID);
            }
            else
            {
                return CppGetDisplayName_Windows(strID);
            }
        }

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        static extern string CppConverterNameList_next_Linux();

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        static extern string CppGetDisplayName_Linux(
            [MarshalAs(UnmanagedType.LPStr)] string strID);

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppConverterNameList_next_Windows();

        [DllImport("IcuTranslitEC.dll", EntryPoint = "IcuTranslitEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppGetDisplayName_Windows([MarshalAs(UnmanagedType.LPStr)] string strID);

        #endregion DLLImport Statements

        #region Member Variable Definitions
        public const string strDisplayName = "ICU Transliterator";
        public const string strHtmlFilename = "ICU_Transliterators_Plug-in_About_box.htm";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public IcuTranslitEncConverter() : base(
            typeof(IcuTranslitEncConverter).FullName, EncConverters.strTypeSILicuTrans)
        {
        }

        /// <summary>
        /// The class destructor. </summary>
        ~IcuTranslitEncConverter()
        {
            //if( IsFileLoaded() )
            //    CCUnloadTable(m_hTable);
        }

        public override void Initialize(
            string converterName,
            string converterSpec,
            ref string lhsEncodingID,
            ref string rhsEncodingID,
            ref ConvType conversionType,
            ref Int32 processTypeFlags,
            Int32 codePageInput,
            Int32 codePageOutput,
            bool bAdding)
        {
            Util.DebugWriteLine(this, "BEGIN");
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID,
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding);

            // the only thing we want to add (now that the convType can be less than accurate)
            //  is to make sure it's unidirectional
            switch (conversionType)
            {
                case ConvType.Legacy_to_from_Legacy:
                    conversionType = ConvType.Legacy_to_Legacy;
                    break;
                case ConvType.Legacy_to_from_Unicode:
                    conversionType = ConvType.Legacy_to_Unicode;
                    break;
                case ConvType.Unicode_to_from_Legacy:
                    conversionType = ConvType.Unicode_to_Legacy;
                    break;
                case ConvType.Unicode_to_from_Unicode:
                    conversionType = ConvType.Unicode_to_Unicode;
                    break;
                default:
                    break;
            }
            Util.DebugWriteLine(this, "END");
        }

        #endregion Initialization

        #region Misc helpers
        protected bool IsFileLoaded()
        {
            //return (m_hTable != 0);
            return false;
        }

        protected void Unload()
        {
            //Util.DebugWriteLine(this, "BEGIN");
            //if( IsFileLoaded() )
            //{
            //    CCUnloadTable(m_hTable);
            //    m_hTable = 0;
            //}
        }

        protected override EncodingForm DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16
            return EncodingForm.UTF16;
        }

        protected unsafe void Load(string strTranslitID)
        {
            Util.DebugWriteLine(this, "BEGIN");

            if (IsFileLoaded())
                Unload();

            Util.DebugWriteLine(this, "Calling CppInitialize");
            int status = 0;

            status = CppInitialize(strTranslitID);
            if (status != 0)
            {
                throw new Exception("CppInitialize failed.");
            }
            Util.DebugWriteLine(this, "END");
        }
        #endregion Misc helpers

        #region Abstract Base Class Overrides
        protected override void PreConvert
            (
            EncodingForm        eInEncodingForm,
            ref EncodingForm    eInFormEngine,
            EncodingForm        eOutEncodingForm,
            ref EncodingForm    eOutFormEngine,
            ref NormalizeFlags  eNormalizeOutput,
            bool                bForward
            )
        {
            // let the base class do its thing first
            base.PreConvert(eInEncodingForm, ref eInFormEngine,
                            eOutEncodingForm, ref eOutFormEngine,
                            ref eNormalizeOutput, bForward);

            if (NormalizeLhsConversionType(ConversionType) == NormConversionType.eUnicode)
            {
                if (Util.IsUnix)
                {
                    // returning this value will cause the input Unicode data (of any form, UTF16, BE, etc.)
                    //	to be converted to UTF8 narrow bytes before calling DoConvert.
                    eInFormEngine = EncodingForm.UTF8Bytes;
                }
                else
                {
                    eInFormEngine = EncodingForm.UTF16;
                }
            }
            else
            {
                // legacy
                eInFormEngine = EncodingForm.LegacyBytes;
            }

            // Output will be stored in a typical C# string, so eOutFormEngine will be UTF16,
            // even though the Perl script is writing UTF8 bytes to output.
            if (NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode)
            {
                if (Util.IsUnix)
                {
                    eOutFormEngine = EncodingForm.UTF8Bytes;
                }
                else
                {
                    eOutFormEngine = EncodingForm.UTF16;
                }
            }
            else
            {
                eOutFormEngine = EncodingForm.LegacyBytes;
            }

            // do the load at this point.
            Load(ConverterIdentifier);
        }

        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
            Util.DebugWriteLine(this, "BEGIN");
            int status = 0;
            fixed (int* pnOut = &rnOutLen)
            {
                status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
            }
            if (status != 0)
            {
                EncConverters.ThrowError(ErrStatus.Exception, "CppDoConvert() failed.");
            }
            Util.DebugWriteLine(this, "END");
        }

        protected override string GetConfigTypeName
        {
            get { return typeof(IcuTranslitConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides

        #region Additional public methods to access the C++ DLL.
        /// <summary>
        /// Gets the available ICU transliterator specifications.
        /// </summary>
        public static List<string> GetAvailableConverterSpecs()
        {
            int count = CppConverterNameList_start();
            List<string> specs = new List<string>(count);
            for (int i = 0; i < count; ++i)
                specs.Add(CppConverterNameList_next());
            return specs;
        }

        /// <summary>
        /// Gets the display name of the given ICU transliterator specification.
        /// </summary>
        public static string GetDisplayName(string spec)
        {
            return CppGetDisplayName(spec);
        }
        #endregion
    }
}
