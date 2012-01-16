//
// PerlExpressionEncConverter.cs
//
// Created by Jim Kornelsen on Nov 21 2011
//
// 28-Nov-11 JDK  Wrap Perl expression and write in temp file, rather than requiring input file.
// 09-Jan-12 JDK  eInFormEngine should be UTF8, but eOutFormEngine UTF16 because it is a C# string.
//
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;                  // for RegistryKey
using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
    /// <summary>
    /// Managed Perl expression EncConverter
    /// </summary>
    //[GuidAttribute("54E0185D-3603-4113-B323-E0222FAD4CCE")]
    // normally these subclasses are treated as the base class (i.e. the 
    //  client can use them orthogonally as IEncConverter interface pointers
    //  so normally these individual subclasses would be invisible), but if 
    //  we add 'ComVisible = false', then it doesn't get the registry 
    //  'HKEY_CLASSES_ROOT\SilEncConverters40.TecEncConverter' which is the basis of 
    //  how it is started (see EncConverters.AddEx).
    // [ComVisible(false)]
	public class PerlExpressionEncConverter : ExeEncConverter
    {
        #region Member Variable Definitions
        private string      strTempFile      = string.Empty;

        public const string strDisplayName = "Perl Script";
        public const string strHtmlFilename = "Perl Expression Plug-in About box.htm";
        public const string strExeDefPath   = "";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public PerlExpressionEncConverter()
            : base (
            typeof(PerlExpressionEncConverter).FullName,
            EncConverters.strTypeSILPerlExpression,
            ConvType.Unicode_to_from_Unicode,        // conversionType
            EncConverters.strDefUnicodeEncoding,     // lhsEncodingID
            EncConverters.strDefUnicodeEncoding,     // rhsEncodingID
            (Int32)ProcessTypeFlags.Transliteration, // lProcessType
            strExeDefPath)
        {
        }

        /// <summary>
        /// The class destructor. </summary>
        ~PerlExpressionEncConverter()
        {
            Unload();
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
            System.Diagnostics.Debug.WriteLine("PerlExpression EC Initialize BEGIN");
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID, 
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

/*
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
                //case ConvType.Unicode_to_from_Unicode:
                //    conversionType = ConvType.Unicode_to_Unicode;
                //    break;
                default:
                    break;
            }
*/

            System.Diagnostics.Debug.WriteLine("PerlExpression EC Initialize END");
        }
        #endregion Initialization

        public override string ExeName
        {
            get {
                if (ECNormalizeData.IsUnix)
                    return "perl";
                else
                    return "perl.exe";
            }
        }
        public override string Arguments
        {
            get {
                return this.strTempFile + " +s";
            }
        }

        #region Misc helpers
        protected bool IsFileLoaded()
        { 
            return !string.IsNullOrEmpty(strTempFile);
        }

        protected void Unload()
        { 
            System.Diagnostics.Debug.WriteLine("PerlExpressionEncConverter.Unload");
            if( IsFileLoaded() )
            {
                File.Delete(strTempFile);
                System.Diagnostics.Debug.WriteLine("Deleted file " + strTempFile);
                strTempFile = string.Empty;
                m_psi = null;
            }
        }

        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16
            return EncodingForm.UTF16;
        }

        protected unsafe void Load(string strExpression)
        {
            System.Diagnostics.Debug.WriteLine("PerlExpression Load BEGIN");
            //this.strFilepath = strExpression;

            if( IsFileLoaded() ) {
                //Unload();
                return;
            }

            bool useInOut = strExpression.Contains("$strInOut");
            if (!useInOut)
            {
                //
                // Check to make sure the expression contains required code.
                //
                if (!strExpression.Contains("$strIn"))
                {
                    throw new Exception (
                        "The Perl expression:\n\n'" + strExpression +
                        "'\n\ndoesn't contain the required reference to the input data " +
                        "string '$strIn ' or '$strInOut '\n\n(e.g. '$strOut = reverse($strIn);')");
                }
                if (!strExpression.Contains("$strOut"))
                {
                    throw new Exception (
                        "The Perl expression:\n\n'" + strExpression +
                        "'\n\ndoesn't contain the required assignment to the output data " +
                        "string '$strOut'\n\n(e.g. '$strOut = reverse($strIn);')");
                }
                if (strExpression.Contains("print $") || strExpression.Contains("print \"") ||
                    strExpression.Contains("print '"))
                {
                    throw new Exception (
                        "The Perl expression must not print output, as this will " +
                        "interfere with the resulting value.");
                }
                if (strExpression.Contains("STDIN"))
                {
                    throw new Exception (
                        "The Perl expression must not read from STDIN, as this will " +
                        "interfere with the input string.");
                }
            }

            strTempFile = string.Empty;
            try
            {
                strTempFile = Path.GetTempFileName();   // create a temporary file
                FileInfo fileInfo = new FileInfo(strTempFile);
                fileInfo.Attributes = FileAttributes.Temporary;
                System.Diagnostics.Debug.WriteLine("Temporary file created: " + strTempFile);

                TextWriter tw = new StreamWriter(strTempFile);
                tw.WriteLine("binmode(STDIN,  ':utf8');");
                tw.WriteLine("binmode(STDOUT, ':utf8');");
                tw.WriteLine("my $strIn = scalar(<STDIN>);");
                tw.WriteLine(@"$strIn =~ s/^\x{FEFF}//;");  // remove BOM
                tw.WriteLine("chomp $strIn;");              // remove endline characters
                if (useInOut) {
                    tw.WriteLine("my $strInOut = $strIn;");
                    tw.WriteLine("undef $strIn;");
                } else {
                    tw.WriteLine("my $strOut;");
                }
                tw.WriteLine(strExpression);
                if (useInOut)
                    tw.WriteLine("print $strInOut;");
                else
                    tw.WriteLine("print $strOut;");
                tw.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine (
                    "Unable to create TEMP file or set attributes: " + ex.Message);
            }

            //
            // Test to see if the script compiles.
            //
            ProcessStartInfo psi = new ProcessStartInfo(ExeName, "-c " + strTempFile);
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            StreamReader myErrOut = p.StandardError;
            string errOutput = myErrOut.ReadToEnd();
            if (!errOutput.Contains("syntax OK"))
            {
                throw new Exception (
                    "The Perl code did not compile correctly:\n\n" + errOutput);
            }
            System.Diagnostics.Debug.WriteLine("PerlExpression Load END");
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

            // The Perl converter expects UTF8 bytes as input (see the binmode line in Load() above).
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

            // Output will be stored in a typical C# string, so eOutFormEngine will be UTF16,
            // even though the Perl script is writing UTF8 bytes to output.
            if( NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode )
            {
                //eOutFormEngine = EncodingForm.UTF8Bytes;
                eOutFormEngine = EncodingForm.UTF16;
                //eOutFormEngine = EncodingForm.UTF16BE;
            }
            else
            {
                eOutFormEngine = EncodingForm.LegacyBytes;
            }

            // do the load at this point.
            Load(ConverterIdentifier);
        }

/*
        [CLSCompliant(false)]
        protected override unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
//          rde1.2.1.0 don't pad with space anymore
            rde2.2.0.0 Ahh... now I remember why this was there before: if you use boundary
            condition testing in CC (e.g. "prec(ws) 'i' fol(ws)", where  'ws' contains things 
            like spaces, nl, tabs, punctuation, etc) then those tests will fail on the first 
            and last character in the stream (which are at a boundary conditions, but can't be 
            detected by CC). Anyway, so I want to put back in the stream delimiting, but the 
            reason this was originally taken out was because someone had a CC table which was 
            eating spaces, so I'll use 'd10' (which never comes in on an Windows system by itself)
            to delimit the stream AND only then if it's a spelling fixer cc table (see Initialize)
//
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
            //{
            //status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
            System.Diagnostics.Debug.WriteLine("Using perl script " + this.strFilepath);
            ProcessStartInfo psi = new ProcessStartInfo (
                "perl",  // perl.exe on Windows
                this.strFilepath + " +s");
            psi.UseShellExecute = false;
            psi.RedirectStandardInput  = true;
            psi.RedirectStandardOutput = true;
            psi.StandardOutputEncoding = Encoding.UTF8;
            Process p=new Process();
            p.StartInfo = psi;
            p.Start();
            StreamWriter myStreamWriter = p.StandardInput;

            byte[] inBuffer_managed = new byte[nInLen];
            Marshal.Copy((IntPtr)lpInBuffer, inBuffer_managed, 0, nInLen);
            //ECNormalizeData.ByteStarToByteArr(lpInBuffer, nInLen, inBuffer_managed);
            string inString = System.Text.Encoding.UTF8.GetString(inBuffer_managed, 0, nInLen);
            //string inString = Marshal.PtrToStringUni((IntPtr)lpInBuffer, nInLen); // doesn't work well
            myStreamWriter.WriteLine(inString);
            myStreamWriter.Close();

            string output = p.StandardOutput.ReadToEnd();
            System.Diagnostics.Debug.WriteLine("Got result: " + output);
            p.WaitForExit();
//begin comment
            Regex re = new Regex("\r\n$");
            if (re.IsMatch(output)) {
                System.Diagnostics.Debug.WriteLine("stripping Windows-style newline");
                output = re.Replace(output, "");
            } else {
                System.Diagnostics.Debug.WriteLine("stripping Unix-style newline");
                re = new Regex("\n$");
                output = re.Replace(output, "");
            }
//end
            //TODO: Move this code into a method "CopyStringToUTF8ByteStar" /* for Linux /
            //int nLengthBytes = output.Length * 2; // for Unicode
            //int nLengthBytes = output.Length;   // for ANSI
            //rnOutLen = nLengthBytes;
            //CopyStringToByteStar((IntPtr)lpOutBuffer, output);
            byte[] baOut1 = Encoding.UTF8.GetBytes(output);
            rnOutLen = Marshal.SizeOf(baOut1[0]) * baOut1.Length + Marshal.SizeOf(baOut1[0]);
            Marshal.Copy(baOut1, 0, (IntPtr)lpOutBuffer, baOut1.Length);
            Marshal.WriteByte((IntPtr)lpOutBuffer, rnOutLen, 0);
//begin comment
            byte* stringPointer = (byte*) Marshal.StringToHGlobalUni(output).ToPointer();
            IntPtr stringPointer = Marshal.StringToHGlobalUni(output);
            //byte* stringPointer = (byte*) Marshal.StringToHGlobalAnsi(output).ToPointer();
            ECNormalizeData.MemMove(lpOutBuffer, stringPointer, nLengthBytes);
            //rnOutLen = ECNormalizeData.StringToByteStar(output, lpOutBuffer, nLengthBytes);
            //lpOutBuffer = (byte *)Marshal.StringToHGlobalAnsi(output);
            //lpOutBuffer = (byte *)System.Text.Encoding.UTF8.GetBytes(output);
//end
            //}

            if( status != 0 )  
            {
                EncConverters.ThrowError(ErrStatus.Exception, "Perl call failed.");
            }
#if !rde220
            else if( m_bUseDelimiters )
            {
//begin comment
                if( lpOutBuffer[0] == byDelim )
                    ECNormalizeData.MemMove(lpOutBuffer, lpOutBuffer + 1, --rnOutLen);
                if( lpOutBuffer[rnOutLen - 1] == byDelim )
                    rnOutLen--;                
//end comment
            }
#else
//begin comment
            // otherwise strip out that final space we added (sometimes it goes away by itself!!??, 
            //  so check first...)
            //  also only if the last of the input was *NOT* a space...
            else if( !bLastWasSpace && (lpOutBuffer[rnOutLen-1] == ' ') )
            {
                rnOutLen--;
            }
//end comment
#endif
            // Check to see how it turned out.
            byte[] baOut = new byte[rnOutLen];
            Marshal.Copy((IntPtr)lpOutBuffer, baOut, 0, rnOutLen);
            //ECNormalizeData.ByteStarToByteArr(lpOutBuffer, rnOutLen, baOut);
            //String strConvertedResult = System.Text.Encoding.Default.GetString(baOut);
            String strConvertedResult = System.Text.Encoding.UTF8.GetString(baOut);
            //String strConvertedResult = System.Text.Encoding.ASCII.GetString(baOut);
            System.Diagnostics.Debug.WriteLine("Returning val '" + strConvertedResult + "'");
            System.IO.TextWriter tw = new System.IO.StreamWriter(
                "/media/winD/Jim/computing/SEC_on_linux/testing/returning.txt");
            tw.WriteLine("output: '" + output + "'");
            tw.WriteLine("result: '" + strConvertedResult + "'");
//begin comment
            byte[] baOut2 = new byte[rnOutLen];
            ECNormalizeData.ByteStarToByteArr(stringPointer, rnOutLen, baOut);
            //String strPointerConv = System.Text.Encoding.Default.GetString(baOut);
            String strPointerConv = System.Text.Encoding.UTF8.GetString(baOut);
            tw.WriteLine("intermediate: '" + strPointerConv + "'");
            Marshal.FreeHGlobal((IntPtr)stringPointer);
//end comment
            tw.Close();
        }
*/

/*
        private static void CopyStringToByteStar(IntPtr pExportedDataAddress, string value)
        {
            int iByteSizeOfCharacter = 0;
            IntPtr pUnmanagedString = IntPtr.Zero;

            //if (false)  // change this as needed
            if (true)  // change this as needed
            {
                iByteSizeOfCharacter = sizeof(sbyte);
                pUnmanagedString = Marshal.StringToHGlobalAnsi(value);  // utf8 on Linux?
            }
            else
            {
                iByteSizeOfCharacter = sizeof(char);
                pUnmanagedString = Marshal.StringToHGlobalUni(value);
            }

            // Allocate a byte array with a size according to the character byte size of the
            // current string type. Also allocate space for a terminating NULL character.
            byte[] byArray = new byte[(value.Length * iByteSizeOfCharacter) + iByteSizeOfCharacter];

            // Copy all ANSI characters from pUnmanagedString to the byte array.
            Marshal.Copy(pUnmanagedString, byArray, 0, (value.Length * iByteSizeOfCharacter));
            // Make sure the last byte element is a null value.
            byArray[value.Length] = 0;

            // Now copy all bytes from the byte array to the pExportedDataAddress.
            Marshal.Copy(byArray, 0, pExportedDataAddress, byArray.Length);

            // We must not forget to free the unmanaged memory which contains
            // a copy of the "value".
            Marshal.FreeHGlobal(pUnmanagedString);
            pUnmanagedString = IntPtr.Zero;
        }
*/

        protected override string   GetConfigTypeName
        {
            get { return typeof(PerlExpressionEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides
    }
}
