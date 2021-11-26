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
        public static readonly string strHtmlFilename = "Perl_Expression_Plug-in_About_box.htm";
        public const string strExeDefPath = "";
        #endregion Member Variable Definitions

        #region Initialization
        /// <summary>
        /// The class constructor. </summary>
        public PerlExpressionEncConverter()
            : base(
            typeof(PerlExpressionEncConverter).FullName,
            EncConverters.strTypeSILPerlExpression,
            ConvType.Unicode_to_Unicode,             // conversionType
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
            // let the base class have first stab at it
            base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID,
                ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, bAdding );

            // this is the only one we support from now on (if the user really wants to do legacy to unicode, they have to deal with the legacy as coming in utf-8 format
            conversionType = ConvType.Unicode_to_Unicode;
        }
        #endregion Initialization

        public override string ExeName
        {
            get
            {
                if (Util.IsUnix)
                {
                    return "perl";
                }
                return
#if DEBUG
                        File.Exists(@"\temp\perl\bin\perl.exe")
                            ? @"\temp\perl\bin\perl.exe"
                            : "perl.exe";
#else
                        "perl.exe";
#endif
            }
        }

        public override string Arguments
        {
            get
            {
                // put the path to the temp file in quotes
                return String.Format("\"{0}\" +s", strTempFile);
            }
        }

        #region Misc helpers
        protected bool IsFileLoaded()
        {
            return !string.IsNullOrEmpty(strTempFile);
        }

        protected void Unload()
        {
            Util.DebugWriteLine(this, "BEGIN");
            if( IsFileLoaded() )
            {
                File.Delete(strTempFile);
                Util.DebugWriteLine(this, "Deleted file " + strTempFile);
                strTempFile = string.Empty;
                m_psi = null;
            }
        }

        protected override EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            // if it's unspecified, then we want UTF-16
            return EncodingForm.UTF16;
        }

        protected void Load(string strExpression)
        {
            Util.DebugWriteLine(this, "BEGIN");
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
                Util.DebugWriteLine (
                    this, "Unable to create TEMP file or set attributes: " + ex.Message);
            }

            //
            // Test to see if the script compiles.
            //
            var psi = new ProcessStartInfo(ExeName, String.Format("-c \"{0}\"", strTempFile))
                          {
                              RedirectStandardError = true,
                              UseShellExecute = false,
                              CreateNoWindow = true
                          };
            var p = new Process
                        {
                            StartInfo = psi
                        };
            p.Start();
            var myErrOut = p.StandardError;
            var errOutput = myErrOut.ReadToEnd();
            if (!errOutput.Contains("syntax OK"))
            {
                throw new ApplicationException(
                    String.Format("The Perl expression did not compile correctly:{0}{0}{1}",
                                  Environment.NewLine, errOutput));
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
            base.PreConvert( eInEncodingForm, ref eInFormEngine,
							eOutEncodingForm, ref eOutFormEngine,
							ref eNormalizeOutput, bForward);

            // do the load at this point.
            Load(ConverterIdentifier);
        }

        protected override string   GetConfigTypeName
        {
            get { return typeof(PerlExpressionEncConverterConfig).AssemblyQualifiedName; }
        }

        #endregion Abstract Base Class Overrides
    }
}
