// Created by Jim Kornelsen on Nov 14 2011
//
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;                  // for RegistryKey
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// Managed Python script EncConverter
    /// </summary>
    //[GuidAttribute("54E0185D-3603-4113-B323-E0222FAD4CCE")]
    // normally these subclasses are treated as the base class (i.e. the
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if
    //  we add 'ComVisible = false', then it doesn't get the registry
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.TecEncConverter' which is the basis of
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)]
    public class PyScriptEncConverter : EncConverter
    {
        #region DLLImport Statements
        // On Linux looks for libPyScriptEncConverter.so (adds lib- and -.so)
        [DllImport("PyScriptEncConverter", EntryPoint = "PyScriptEC_Initialize", CallingConvention = CallingConvention.Cdecl)]
        static extern int CppInitialize(
            [MarshalAs(UnmanagedType.LPStr)] string strScript,
            [MarshalAs(UnmanagedType.LPStr)] string strDir);

        [DllImport("PyScriptEncConverter", EntryPoint = "PyScriptEC_PreConvert", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int CppPreConvert(
            int eInFormEngine, int eOutFormEngine,
            int eNormalizeOutput, bool bForward);

        [DllImport("PyScriptEncConverter", EntryPoint = "PyScriptEC_DoConvert", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int CppDoConvert(
            byte* lpInputBuffer, int nInBufLen,
            byte* lpOutputBuffer, int* npOutBufLen);
        #endregion DLLImport Statements

        #region Member Variable Definitions
        private DateTime m_timeModified = DateTime.MinValue;

        public const string strDisplayName = "Python Script";
        public const string strHtmlFilename = "Python_Script_Plug-in_About_box.htm";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public PyScriptEncConverter() : base(
            typeof(PyScriptEncConverter).FullName, EncConverters.strTypeSILPyScript)
        {
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
			m_eConversionType = conversionType = MakeUniDirectional(conversionType);

            // if we're supposedly adding this one, then clobber our copy of its last modified
            // (there was a problem with us instantiating lots of these things in a row and
            //  not detecting the change because the modified date was within a second of each
            //  other)
            if (bAdding)
            {
                Util.DebugWriteLine(this, "Adding");
                m_timeModified = DateTime.MinValue;

                // do the load at this point; not that we need it, but for checking that everything's okay.
                Load();
            }
            Util.DebugWriteLine(this, "END");
        }

        #endregion Initialization

        #region Abstract Base Class Overrides
        protected override void PreConvert
            (
            EncodingForm eInEncodingForm,
            ref EncodingForm eInFormEngine,
            EncodingForm eOutEncodingForm,
            ref EncodingForm eOutFormEngine,
            ref NormalizeFlags eNormalizeOutput,
            bool bForward
            )
        {
            // let the base class do its thing first
            base.PreConvert(eInEncodingForm, ref eInFormEngine,
                            eOutEncodingForm, ref eOutFormEngine,
                            ref eNormalizeOutput, bForward);

            if (NormalizeLhsConversionType(ConversionType) == NormConversionType.eUnicode)
            {
                // We could use UTF-8 here, but wide data works just fine.
                // the Windows version definitely needs UTF16.
                if (Util.IsUnix)
                {
                    Util.DebugWriteLine(this, "eInFormEngine UTF32");
                    eInFormEngine = EncodingForm.UTF32;
                }
                else
                {
                    Util.DebugWriteLine(this, "eInFormEngine UTF16");
                    eInFormEngine = EncodingForm.UTF16;
                }
            }
            else
            {
                // legacy
                Util.DebugWriteLine(this, "eInFormEngine LegacyBytes");
                eInFormEngine = EncodingForm.LegacyBytes;
            }

            if (NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode)
            {
                if (Util.IsUnix)
                {
                    Util.DebugWriteLine(this, "eOutFormEngine UTF32");
                    eOutFormEngine = EncodingForm.UTF32;
                }
                else
                {
                    Util.DebugWriteLine(this, "eOutFormEngine UTF16");
                    eOutFormEngine = EncodingForm.UTF16;
                }
            }
            else
            {
                Util.DebugWriteLine(this, "eOutFormEngine LegacyBytes");
                eOutFormEngine = EncodingForm.LegacyBytes;
            }

            // do the load at this point
            Load();

            // then do the C++ encoding form settings
            CppPreConvert((int)eInFormEngine,
                          (int)eOutFormEngine,
                          (int)eNormalizeOutput, bForward);
        }

        protected unsafe void Load()
        {
            Util.DebugWriteLine(this, "BEGIN");
            string strScriptPath = ConverterIdentifier;

            // first make sure it's there and get the last time it was modified
            DateTime timeModified = DateTime.Now; // don't care really, but have to initialize it.
            if (!DoesFileExist(strScriptPath, ref timeModified))
                EncConverters.ThrowError(ErrStatus.CantOpenReadMap, strScriptPath);

            // if it has been modified or it's not already loaded...
            if (timeModified > m_timeModified)
            {
                // keep track of the modified date, so we can detect a new version to reload
                m_timeModified = timeModified;

                Util.DebugWriteLine(this, "Calling CppInitialize");
                string strScriptName = Path.GetFileName(strScriptPath);
                string strScriptDir = Path.GetDirectoryName(strScriptPath);
                int status = CppInitialize(strScriptName, strScriptDir);
                if (status != 0)
                {
                    var strExtraValue = strScriptPath;
                    var errStatus = (ErrStatus)status;
                    EncConverters.ThrowError(errStatus, strExtraValue);
                }
                Util.DebugWriteLine(this, "Finished calling CppInitialize");
            }
            Util.DebugWriteLine(this, "END");
        }

        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
            int status = 0;
            fixed (int* pnOut = &rnOutLen)
            {
#if DEBUG
                if (Util.IsUnix)
                {
                    byte[] baIn = new byte[nInLen];
                    ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Sending bytes to CppDoConvert", baIn));
                }
#endif
                status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
            }

            if (status != 0)
            {
                EncConverters.ThrowError(status);
            }
        }

        protected override string GetConfigTypeName
        {
            get { return typeof(PyScriptEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides
    }
}
