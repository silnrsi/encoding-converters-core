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
    /// Managed Pyhton script EncConverter
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
        //[DllImport("PyScriptEncConverter", SetLastError=true)]
		[DllImport("PyScriptEncConverter", EntryPoint = "PyScriptEC_Initialize", CallingConvention = CallingConvention.Cdecl)]
        static extern int CppInitialize (
            [MarshalAs(UnmanagedType.LPStr)] string strScript,
            [MarshalAs(UnmanagedType.LPStr)] string strDir);

		[DllImport("PyScriptEncConverter", EntryPoint = "PyScriptEC_DoConvert", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int CppDoConvert(
            byte* lpInputBuffer, int nInBufLen,
            byte* lpOutputBuffer, int *npOutBufLen);
        #endregion DLLImport Statements

        #region Member Variable Definitions
        private DateTime    m_timeModified = DateTime.MinValue;

        public const string strDisplayName = "Python Script";
        public const string strHtmlFilename = "Python Script Plug-in About box.mht";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public PyScriptEncConverter() : base (
            typeof(PyScriptEncConverter).FullName,EncConverters.strTypeSILPyScript)
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
            DebugWriteLine("PyScript EC Initialize BEGIN");
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

            // this is the only one we support from now on (if the user really wants to do legacy to unicode, they have to deal with the legacy as coming in utf-8 format
            conversionType = ConvType.Unicode_to_Unicode;

            // if we're supposedly adding this one, then clobber our copy of its last modified 
            // (there was a problem with us instantiating lots of these things in a row and
            //  not detecting the change because the modified date was within a second of each 
            //  other)
            if( bAdding )
                m_timeModified = DateTime.MinValue;
            DebugWriteLine("PyScript EC Initialize END");
        }

        #endregion Initialization

        #region Misc helpers
        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16 in C#.
            return EncodingForm.UTF16;
        }

        protected unsafe void Load(string strScriptPath)
        {
            DebugWriteLine("PyScript Load BEGIN");
            // first make sure it's there and get the last time it was modified
            DateTime timeModified = DateTime.Now; // don't care really, but have to initialize it.
            if( !DoesFileExist(strScriptPath, ref timeModified) )
                EncConverters.ThrowError(ErrStatus.CantOpenReadMap, strScriptPath);

            // if it has been modified or it's not already loaded...
            if( timeModified > m_timeModified )
            {
                // keep track of the modified date, so we can detect a new version to reload
                m_timeModified = timeModified;

                DebugWriteLine("Calling CppInitialize");
                string strScriptName = Path.GetFileName(strScriptPath);
                string strScriptDir = Path.GetDirectoryName(strScriptPath);
                int status = 0;

                status = CppInitialize(strScriptName, strScriptDir);
                if( status != 0 )
                {
                    var strExtraValue = strScriptPath;
                    var errStatus = (ErrStatus) status;
                    switch(errStatus)
                    {
                        case ErrStatus.NameNotFound:
                            strExtraValue = "Convert";
                            break;
                    }
                    EncConverters.ThrowError(errStatus, strExtraValue);
                }
                DebugWriteLine("Finished calling CppInitialize");
            }
            DebugWriteLine("PyScript Load END");
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
            base.PreConvert( eInEncodingForm, ref eInFormEngine,
							eOutEncodingForm, ref eOutFormEngine,
							ref eNormalizeOutput, bForward);

            // On Linux the Python C++ EncConverter expects type char, which is UTF8 (narrow).
            if( NormalizeLhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                // returning this value will cause the input Unicode data (of any form, UTF16, BE, etc.)
                //	to be converted to UTF8 narrow bytes before calling DoConvert.
                eInFormEngine = EncodingForm.UTF16;
            }
            else
            {
                System.Diagnostics.Debug.Fail("This converter doesn't support a legacy side (anymore)");
                // legacy
                eInFormEngine = EncodingForm.LegacyBytes;
            }

            if( NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                eOutFormEngine = EncodingForm.UTF16;
            }
            else
            {
                System.Diagnostics.Debug.Fail("This converter doesn't support a legacy side (anymore)");
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
            int status = 0;
            fixed(int* pnOut = &rnOutLen)
            {
                status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
            }

            if( status != 0 )  
            {
                EncConverters.ThrowError(ErrStatus.Exception, "Python Script errored out during conversion!");
            }
        }

        protected override string   GetConfigTypeName
        {
            get { return typeof(PyScriptEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides
    }
}
