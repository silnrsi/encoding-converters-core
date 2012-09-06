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
        [DllImport("PyScriptEncConverter", EntryPoint="PyScriptEC_Initialize")]
        static extern unsafe int CppInitialize (
            [MarshalAs(UnmanagedType.LPStr)] string strScript,
            [MarshalAs(UnmanagedType.LPStr)] string strDir);

        [DllImport("PyScriptEncConverter", EntryPoint="PyScriptEC_DoConvert")]
        static extern unsafe int CppDoConvert(
            byte* lpInputBuffer, int nInBufLen,
            byte* lpOutputBuffer, int *npOutBufLen);
        #endregion DLLImport Statements

        #region Member Variable Definitions
        private Int32       m_hTable = 0;
        private DateTime    m_timeModified = DateTime.MinValue;
        private bool        m_bUseDelimiters = false;

        public const string strDisplayName = "Python Script";
        public const string strHtmlFilename = "Python Script Plug-in About box.htm";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public PyScriptEncConverter() : base (
            typeof(PyScriptEncConverter).FullName,EncConverters.strTypeSILPyScript)
        {
        }

        /// <summary>
        /// The class destructor. </summary>
        ~PyScriptEncConverter()
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
            System.Diagnostics.Debug.WriteLine("PyScript EC Initialize BEGIN");
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

            // the only thing we want to add (now that the convType can be less than accurate) 
            //  is to make sure it's unidirectional
            switch(conversionType)
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

            // if we're supposedly adding this one, then clobber our copy of its last modified 
            // (there was a problem with us instantiating lots of these things in a row and
            //  not detecting the change because the modified date was within a second of each 
            //  other)
            if( bAdding )
                m_timeModified = DateTime.MinValue;
            System.Diagnostics.Debug.WriteLine("PyScript EC Initialize END");
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
            //System.Diagnostics.Debug.WriteLine("CcEncConverter.Unload");
            //if( IsFileLoaded() )
            //{
            //    CCUnloadTable(m_hTable);
            //    m_hTable = 0;
            //}
        }

        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16 in C#.
            return EncodingForm.UTF16;
        }

        protected unsafe void Load(string strScriptPath)
        {
            System.Diagnostics.Debug.WriteLine("PyScript Load BEGIN");
            // first make sure it's there and get the last time it was modified
            DateTime timeModified = DateTime.Now; // don't care really, but have to initialize it.
            if( !DoesFileExist(strScriptPath, ref timeModified) )
                EncConverters.ThrowError(ErrStatus.CantOpenReadMap, strScriptPath);

            // if it has been modified or it's not already loaded...
            if( timeModified > m_timeModified )
            {
                // keep track of the modified date, so we can detect a new version to reload
                m_timeModified = timeModified;

                if( IsFileLoaded() )
                    Unload();

                System.Diagnostics.Debug.WriteLine("Calling CppInitialize");
                string strScriptName = Path.GetFileName(strScriptPath);
                string strScriptDir = Path.GetDirectoryName(strScriptPath);
                int status = 0;
                try {
                    status = CppInitialize(strScriptName, strScriptDir);
                } catch (DllNotFoundException exc) {
                    throw new Exception("Failed to load .so file. Check path.");
                } catch (EntryPointNotFoundException exc) {
                    throw new Exception("Failed to find function in .so file.");
                }
                if( status != 0 )  
                {
                    throw new Exception("CppInitialize failed.");
                }
                System.Diagnostics.Debug.WriteLine("Finished calling CppInitialize");
            }
            System.Diagnostics.Debug.WriteLine("PyScript Load END");
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
                eInFormEngine = EncodingForm.UTF8Bytes;
            }
            else
            {
                // legacy
                eInFormEngine = EncodingForm.LegacyBytes;
            }

            if( NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                eOutFormEngine = EncodingForm.UTF8Bytes;
            }
            else
            {
                eOutFormEngine = EncodingForm.LegacyBytes;
            }

            // do the load at this point.
            Load(ConverterIdentifier);
        }

        [CLSCompliant(false)]
        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
/*          rde1.2.1.0 don't pad with space anymore
            rde2.2.0.0 Ahh... now I remember why this was there before: if you use boundary
            condition testing in CC (e.g. "prec(ws) 'i' fol(ws)", where  'ws' contains things 
            like spaces, nl, tabs, punctuation, etc) then those tests will fail on the first 
            and last character in the stream (which are at a boundary conditions, but can't be 
            detected by CC). Anyway, so I want to put back in the stream delimiting, but the 
            reason this was originally taken out was because someone had a CC table which was 
            eating spaces, so I'll use 'd10' (which never comes in on an Windows system by itself)
            to delimit the stream AND only then if it's a spelling fixer cc table (see Initialize)
*/
#if !rde220
            // the delimiter (if used) is actually '\n', but this normally isn't received by CC 
            // without '\r' as well, so it makes a good delimiter in that CC tables aren't likely 
            // to be looking to eat it up (which was the problem we had when we delimited with
            // a space).
            const byte byDelim = 10;
            if( m_bUseDelimiters )
            {
                // move the input data down to make room for the initial delimiter
                ECNormalizeData.MemMove(lpInBuffer+1, lpInBuffer, nInLen);

                lpInBuffer[0] = byDelim;
                lpInBuffer[nInLen + 1] = byDelim;
                nInLen += 2;
            }
#else
            bool bLastWasD10 = false;
            if( lpInBuffer[nInLen-1] == ' ' )
            {
                bLastWasSpace = true;
            }
            else
            {
                lpInBuffer[nInLen++] = (byte)' ';
                lpInBuffer[nInLen] = 0;
            }
#endif

            int status = 0;
            fixed(int* pnOut = &rnOutLen)
            {
                status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
            }

            if( status != 0 )  
            {
                EncConverters.ThrowError(ErrStatus.Exception, "CppDoConvert() failed.");
            }
#if !rde220
            else if( m_bUseDelimiters )
            {
                if( lpOutBuffer[0] == byDelim )
                    ECNormalizeData.MemMove(lpOutBuffer, lpOutBuffer + 1, --rnOutLen);
                if( lpOutBuffer[rnOutLen - 1] == byDelim )
                    rnOutLen--;                
            }
#else
/*
            // otherwise strip out that final space we added (sometimes it goes away by itself!!??, 
            //  so check first...)
            //  also only if the last of the input was *NOT* a space...
            else if( !bLastWasSpace && (lpOutBuffer[rnOutLen-1] == ' ') )
            {
                rnOutLen--;
            }
*/
#endif
        }

        protected override string   GetConfigTypeName
        {
            get { return typeof(PyScriptEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides
    }
}
