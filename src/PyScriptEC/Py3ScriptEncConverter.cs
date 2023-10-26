// Created by Jim Kornelsen on Nov 14 2011
//
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;                  // for RegistryKey
using ECInterfaces;                     // for IEncConverter
using Python.Runtime;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SilEncConverters40
{
    /// <summary>
    /// Managed Python script EncConverter for Python 3
    /// </summary>
    //[GuidAttribute("54E0185D-3603-4113-B323-E0222FAD4CCE")]
    // normally these subclasses are treated as the base class (i.e. the
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if
    //  we add 'ComVisible = false', then it doesn't get the registry
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.TecEncConverter' which is the basis of
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)]
    public class Py3ScriptEncConverter : EncConverter
    {
        #region Member Variable Definitions
        private DateTime m_timeModified = DateTime.MinValue;
        protected bool m_bLegacy;

        public const string strDisplayName = "Python 3 Script";
        public const string strHtmlFilename = "Python_3_Script_Plug-in_About_box.htm";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public Py3ScriptEncConverter() : base(
            typeof(Py3ScriptEncConverter).FullName, EncConverters.strTypeSILPy3Script)
        {
        }

        ~Py3ScriptEncConverter()
        {
            moduleImported?.Dispose();
            //if (PythonEngine.IsInitialized)
            //    PythonEngine.Shutdown();
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
            m_bLegacy = (EncConverter.NormalizeLhsConversionType(conversionType) == NormConversionType.eLegacy);

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
        }

        public static string ScriptPath(string converterIdentifier)
        {
            return converterIdentifier?.Split(';')?[0];
        }

        public static string DistroPath(string converterIdentifier)
        {
            return (converterIdentifier?.Contains(";") ?? false) ? converterIdentifier?.Split(';')?[1] : null;
        }

        protected unsafe void Load()
        {
            Util.DebugWriteLine(this, "BEGIN");
            string strScriptPath = ScriptPath(ConverterIdentifier);

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
                string strScriptName = Path.GetFileNameWithoutExtension(strScriptPath);
                string strScriptDir = Path.GetDirectoryName(strScriptPath);
                var strErrorExtraValue = strScriptPath;
                try
                {
                    var distroPath = DistroPath(ConverterIdentifier);
                    if (Runtime.PythonDLL != distroPath)
                        Runtime.PythonDLL = distroPath;

                    if (!PythonEngine.IsInitialized)
                    {
                        PythonEngine.Initialize();
                    }

                    // causes access violation and may not be needed (since this isn't async):
                    //  PythonEngine.BeginAllowThreads();
                    dynamic sysModule = Py.Import("sys");
                    strScriptDir = strScriptDir.Replace(@"\", "/");
                    sysModule.path.append(strScriptDir);

                    using (Py.GIL())
                    {
                        moduleImported = Py.Import(strScriptName);
                    }
                }
                catch (Exception ex)
                {
                    strErrorExtraValue = LogExceptionMessage("Py3ScriptEncConverter.Load", ex);
                }
                finally
                {
                    if (moduleImported == null)
                    {
                        EncConverters.ThrowError(ErrStatus.CompilationFailed, strErrorExtraValue);
                    }
                }
                Util.DebugWriteLine(this, "Finished calling CppInitialize");
            }
            Util.DebugWriteLine(this, "END");
        }

        protected dynamic moduleImported;

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
                byte[] baIn = new byte[nInLen];
                ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, baIn);
#if DEBUG
                if (Util.IsUnix)
                {
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Sending bytes to CppDoConvert", baIn));
                }
#endif

                Encoding enc;
                if (m_bLegacy)
                {
                    try
                    {
                        enc = Encoding.GetEncoding(this.CodePageInput);
                    }
                    catch
                    {
                        enc = Encoding.GetEncoding(EncConverters.cnIso8859_1CodePage);
                    }
                }
                else
                {
                    enc = Encoding.Unicode;
                }

                char[] caIn = Encoding.Unicode.GetChars(baIn);

                // here's our input string
                var strInput = new string(caIn);

                // status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
                using (Py.GIL())
                {
                    string strOutput = moduleImported?.Convert(strInput);
                    StringToProperByteStar(strOutput, lpOutBuffer, ref rnOutLen);
                    Util.DebugWriteLine(this, "Result len " + rnOutLen.ToString());
                    Util.DebugWriteLine(this, "END");
                }
            }

            if (status != 0)
            {
                EncConverters.ThrowError(status);
            }
        }

        protected override string GetConfigTypeName
        {
            get { return typeof(Py3ScriptEncConverterConfig).AssemblyQualifiedName; }
        }

        internal static unsafe void StringToProperByteStar(string strOutput, byte* lpOutBuffer, ref int rnOutLen)
        {
            int nLen = strOutput.Length * 2;
            if (nLen > (int)rnOutLen)
                EncConverters.ThrowError(ErrStatus.OutputBufferFull);
            rnOutLen = nLen;
            ECNormalizeData.StringToByteStar(strOutput, lpOutBuffer, rnOutLen, false);
        }

        internal static string LogExceptionMessage(string className, Exception ex)
        {
            string msg = "Error occurred: " + ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                msg += $"{Environment.NewLine}because: (InnerException): {ex.Message}";
            }

            Util.DebugWriteLine(className, msg);
            return msg;
        }

        #endregion Abstract Base Class Overrides
    }
}
