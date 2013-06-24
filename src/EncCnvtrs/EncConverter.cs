#define v22_AllowEmptyReturn    // turn off the code that disallowed empty returns

using System;
using System.Runtime.InteropServices;   // for the class attributes
using System.Collections;               // for Hashtable
using System.Resources;                 // for ResourceManager
using System.IO;
using System.ComponentModel;
using System.Diagnostics;               // for Debug.Assert
using System.Runtime.Remoting;
using System.Text;
using ECInterfaces;                     // for IEncConverter

// This file contains the definitions of all the things used by the EncConverter interface
//  (currently, this includes the enums used by the interface. The actual interface
//  is defined by the EncConverter class).
namespace SilEncConverters40
{
    /// <summary>
    /// Definition of the base managed class of EncConverter.
    /// </summary>
    // by not making this 'AutoDual', the "EncConverter" members don't surface in VBA clients
    //  which encourages people to use the "IEncConverter" interface instead. This is a good
    //  thing, because if the object implementing that interface is really from an unmanaged
    //  DLL (e.g. as it is currently with the ICU plug-ins), then treating it as an
    //  "EncConverter" (a managed class) will usually fail anyway. But managed clients can
    //  still use this guy if they want--even without the 'AutoDual' attribute.
    // [ClassInterface(ClassInterfaceType.AutoDual)]
    /// <summary>
    /// Base class implementation of the IEncConverter interface.</summary>
    /// <remarks>
    /// This is the base class that implements most of the IEncConverter interface methods.
    /// All .Net transducer implementations should derive from this class or one of its
    /// base classes and implement their specializations in the required virtual functions.
    /// For C++/MFC/ATL implementations of IEncConverter, derive from the <seealso>CECEncConverter</seealso>
    /// class in the file ECEncConverters.h</remarks>
    [Guid("4D9C56D5-BA0B-4a5e-B8CA-BA4DD5F321CA")]
    public abstract class EncConverter : IEncConverter
    {
        #region Member Variable Definitions
        private string          m_strProgramID;         // indicates the Program ID from the registry (e.g. "SilEncConverters40.TecEncConverter")
        protected string        m_strImplementType;     // eg. "SIL.tec" rather than the program ID
        private string          m_strName;              // something nice and friendly (e.g. "Annapurna<>Unicode")
        private string          m_strConverterID;       // file spec to the map (or some plug-in specific identifier, such as "Devanagari-Latin" (for ICU) or "65001" (for code page UTF8)
        protected string        m_strLhsEncodingID;     // something unique/unchangable (at least from TK) (e.g. SIL-UNICODE_DEVANAGRI-2002<>SIL-UNICODE_IPA-2002)
        protected string        m_strRhsEncodingID;     // something unique/unchangable (at least from TK) (e.g. SIL-UNICODE_DEVANAGRI-2002<>SIL-UNICODE_IPA-2002)
        protected Int32         m_lProcessType;         // process type (see .idl)
        protected ConvType      m_eConversionType;      // conversion type (see .idl)
        private bool            m_bForward;             // default direction of conversion (for bidirectional conversions; e.g. *not* CC)
        protected bool          m_bDebugDisplayMode;    // should we display debug information?
        private EncodingForm    m_eEncodingInput;       // encoding form of input (see .idl)
        private EncodingForm    m_eEncodingOutput;      // encoding form of output (see .idl)
        private NormalizeFlags  m_eNormalizeOutput;     // should we normalize the output?
        private int             m_nCodePageInput;       // for narrowizing LegacyString data on the input side
        private int             m_nCodePageOutput;      // same for Leg<>Leg (for the output side)--not overridable; only set via Initialize
        protected Hashtable     m_mapProperties;        // map of all attributes (filled during get_AttributeKeys)
        protected bool          m_bInitialized;         // indicates whether Initialize has been called
        protected bool          m_bIsInRepository;      // indicates whether this converter is in the static repository (true) or not (false)

        #endregion Member Variable Definitions

        #region Public Interface
        /// <summary>
        /// The class constructor. </summary>
        public EncConverter(string sProgId, string sImplementType)
        {
            m_strProgramID = sProgId;
            m_strImplementType = sImplementType;
            m_lProcessType = (Int32)ProcessTypeFlags.DontKnow;
            m_eConversionType = ConvType.Legacy_to_from_Unicode;
            m_bForward = true;
            m_eEncodingInput = EncodingForm.Unspecified;
            m_eEncodingOutput = EncodingForm.Unspecified;
            m_eNormalizeOutput = NormalizeFlags.None;
            m_nCodePageInput = 0;
            m_nCodePageOutput = 0;
            m_bDebugDisplayMode = false;
            //m_bDebugDisplayMode = true;
            m_bIsInRepository = false;
        }

        // [DispId(0)]
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        // [DispId(1)]
        public virtual void Initialize(string converterName, string converterSpec, ref string lhsEncodingID, ref string rhsEncodingID, ref ConvType conversionType, ref Int32 processTypeFlags, Int32 codePageInput, Int32 codePageOutput, bool bAdding)
        {
            Util.DebugWriteLine(this, "EncConverter BEGIN");
            m_bInitialized = true;
            m_strName = converterName;
            m_strConverterID = converterSpec;
            m_strLhsEncodingID = lhsEncodingID;
            m_strRhsEncodingID = rhsEncodingID;
            m_eConversionType = conversionType;
            m_lProcessType = processTypeFlags;
            m_nCodePageInput = codePageInput;
            m_nCodePageOutput = codePageOutput;

            // some specs have bad things in them (as far as the .Net File classes are concerned)
            if(     (!String.IsNullOrEmpty(m_strConverterID))
                &&  (m_strConverterID.Length > 9)
                &&  (m_strConverterID.Substring(0,9).ToLower() == "file::///")
                )
            {
                m_strConverterID = m_strConverterID.Substring(9);
            }
            Util.DebugWriteLine(this, "END");
        }

        // [DispId(2)]
        public string ConverterIdentifier
        {
            get { return m_strConverterID; }
        }

        // [DispId(3)]
        public string ProgramID
        {
            get { return m_strProgramID; }
        }

        // [DispId(4)]
        public string ImplementType
        {
            get { return m_strImplementType; }
        }

        // [DispId(5)]
        public virtual ConvType ConversionType
        {
            get { return m_eConversionType; }
        }

        // [DispId(6)]
        public Int32 ProcessType
        {
            get { return m_lProcessType; }
            set { m_lProcessType = value; }
        }

        // [DispId(7)]
        public Int32 CodePageInput
        {
            get { return m_nCodePageInput; }
            set { m_nCodePageInput = value; }
        }

        // [DispId(8)]
        public Int32 CodePageOutput
        {
            get { return m_nCodePageOutput; }
            set { m_nCodePageOutput = value; }
        }

        // [DispId(9)]
        public string LeftEncodingID
        {
            get { return m_strLhsEncodingID; }
        }

        // [DispId(10)]
        public string RightEncodingID
        {
            get { return m_strRhsEncodingID; }
            set { m_strRhsEncodingID = value; }
        }

        // [DispId(12)]
        public IEncConverterConfig Configurator
        {
            get
            {
                // implemented by the sub-classes
                string strConfigTypeName = GetConfigTypeName;
                if( !String.IsNullOrEmpty(strConfigTypeName) )
                {
                    Type typeECConfig = Type.GetType(strConfigTypeName);
                    if (typeECConfig != null)
                    {
                        IEncConverterConfig rIConfigurator = (IEncConverterConfig)Activator.CreateInstance(typeECConfig);

                        rIConfigurator.ParentEncConverter = this;

                        // initialize it (if 'this' was initialized)
                        if (m_bInitialized)
                        {
                            rIConfigurator.ConverterFriendlyName = this.Name;
                            rIConfigurator.ConverterIdentifier = this.ConverterIdentifier;
                            rIConfigurator.LeftEncodingID = this.LeftEncodingID;
                            rIConfigurator.RightEncodingID = this.RightEncodingID;
                            rIConfigurator.ConversionType = this.ConversionType;
                            rIConfigurator.ProcessType = this.ProcessType;
                            rIConfigurator.IsInRepository = this.IsInRepository;
                        }

                        return rIConfigurator;
                    }
                }

                return null;
            }
        }

        // [DispId(13)]
        public bool IsInRepository
        {
            get { return m_bIsInRepository; }
            set { m_bIsInRepository = value; }
        }

        // [DispId(15)]
        public virtual string ConvertToUnicode(byte [] baInput)
        {
            if(     (ConversionType != ConvType.Legacy_to_from_Unicode)
                &&  (ConversionType != ConvType.Unicode_to_from_Legacy)
                &&  (ConversionType != ConvType.Legacy_to_Unicode)
                )
            {
                EncConverters.ThrowError(ErrStatus.InvalidConversionType);
            }

            bool bForward = !(ConversionType == ConvType.Unicode_to_from_Legacy);

            // since 'InternalConvert' is expecting a string, convert the given byte []
            //  to a string and set the input encoding form as LegacyBytes.
            //  (not as efficent as adding a new InternalConvertToUnicode which takes a
            //  byte [] instead, but a) that would require a lot of changes which I'm
            //  afraid would break something, and b) this is far more maintainable).
            // EXCEPT THAT C++ BSTR DOES NOT MAP ONTO C# string!  BSTR's can actually
            // be byte arrays internally without any problem.  C# insists that a string
            // always contains valid data, eg, surrogate pairs must be matched when you
            // construct one from a character array.  This obviously cannot be guaranteed
            // in legacy encoding data!
            int ciOutput;
            string retval = InternalConvertEx(EncodingForm.LegacyBytes, baInput,
                EncodingForm.UTF16, NormalizeOutput, out ciOutput, bForward);
            return retval;
        }

        // [DispId(16)]
        public virtual byte[] ConvertFromUnicode(string sInput)
        {
            if(     (ConversionType != ConvType.Legacy_to_from_Unicode)
                &&  (ConversionType != ConvType.Unicode_to_from_Legacy)
                &&  (ConversionType != ConvType.Unicode_to_Legacy)
                )
            {
                EncConverters.ThrowError(ErrStatus.InvalidConversionType);
            }

            bool bForward = !(ConversionType == ConvType.Legacy_to_from_Unicode);

            // similarly as above, use the normal 'InternalConvert' which is expecting to
            //  return a string, and then convert it to a byte [].
#if __MonoCS__
            // EXCEPT THAT C++ BSTR DOES NOT MAP ONTO C# string!  BSTR's can actually
            // be byte arrays internally without any problem.  C# insists that a string
            // always contains valid data, eg, surrogate pairs must be matched when you
            // construct one from a character array.  This obviously cannot be guaranteed
            // in legacy encoding data!
            int ciOutput;
            byte[] retval = InternalConvertEx(EncodingForm.UTF16, sInput,
                EncodingForm.LegacyBytes, NormalizeOutput, out ciOutput, bForward);
            return retval;
#else
            string sOutput = InternalConvert(EncodingForm.UTF16, sInput, EncodingForm.LegacyBytes,
                NormalizeOutput, bForward);

            return ECNormalizeData.StringToByteArr(sOutput, false);
#endif
        }

        // [DispId(17)]
        public virtual string Convert(string sInput)
        {
            return InternalConvert(EncodingIn, sInput, EncodingOut, NormalizeOutput, DirectionForward);
        }

        // [DispId(18)]
        public virtual string ConvertEx(string sInput, EncodingForm inEnc, int ciInput, EncodingForm outEnc, out int ciOutput, NormalizeFlags eNormalizeOutput, bool bForward)
        {
            return InternalConvertEx(inEnc, sInput, ciInput, outEnc, eNormalizeOutput, out ciOutput, bForward);
        }

        // [DispId(20)]
        public bool DirectionForward
        {
            get { return m_bForward; }
            set { m_bForward = value; }
        }

        // [DispId(21)]
        public EncodingForm EncodingIn
        {
            get { return m_eEncodingInput; }
            set { m_eEncodingInput = value; }
        }

        // [DispId(22)]
        public EncodingForm EncodingOut
        {
            get { return m_eEncodingOutput; }
            set { m_eEncodingOutput = value; }
        }

        // [DispId(23)]
        public bool Debug
        {
            get { return m_bDebugDisplayMode; }
            set { m_bDebugDisplayMode = value; }
        }

        // [DispId(24)]
        public NormalizeFlags NormalizeOutput
        {
            get { return m_eNormalizeOutput; }
            set { m_eNormalizeOutput = value; }
        }

        // [DispId(25)]
        public string [] AttributeKeys
        {
            get
            {
                // we keep track of the key value pairs for subsequent calls to getAttributeValue
                //  (in the map, m_mapProperties), so first clear it out in case it was previously
                //  filled
                if( m_mapProperties == null )
                    m_mapProperties = new Hashtable();
                else
                    m_mapProperties.Clear();

                // next create the safearray that the subclass will fill and ask the subclass to
                //  fill it.
                string [] rSa = null;
                GetAttributeKeys(out rSa);

                return rSa;
            }
        }

        // [DispId(26)]
        public string AttributeValue(string sKey)
        {
            return (string) m_mapProperties[sKey];
        }

        // [DispId(27)]
        public string [] ConverterNameEnum
        {
            get
            {
                string [] rSa = null;
                GetConverterNameEnum(out rSa);  // sub-classes fill
                return rSa;
            }
        }

        // [DispId(28)]
        public override string ToString()
        {
            // give something useful, for example, for a tooltip.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Converter Details:");

            // indicate whether it's temporary or not
            if( !this.IsInRepository )
                sb.Insert(0, "Temporary ");

            sb.AppendLine(String.Format("    Name: '{0}'", Name));
            sb.AppendLine(String.Format("    Identifier: '{0}'", ConverterIdentifier));
            sb.AppendLine(String.Format("    Implementation Type: '{0}'", ImplementType));
            sb.AppendLine(String.Format("    Conversion Type: '{0}'", ConversionType));
            if( (ProcessTypeFlags)ProcessType != ProcessTypeFlags.DontKnow )
                sb.AppendLine(String.Format("    Process Type: '{0}'", strProcessType(ProcessType)));
            if( !String.IsNullOrEmpty(LeftEncodingID) )
                sb.AppendLine(String.Format("    Left side Encoding ID: '{0}'", LeftEncodingID));
            if( !String.IsNullOrEmpty(RightEncodingID) )
                sb.AppendLine(String.Format("    Right side Encoding ID: '{0}'", RightEncodingID));

            // also include the current conversion option values
            sb.AppendLine(String.Format("    {0}{0}Current Conversion Options:", Environment.NewLine));
            sb.AppendLine(String.Format("    Direction: '{0}'", (this.DirectionForward) ? "Forward" : "Reverse"));
            sb.AppendLine(String.Format("    Normalize Output: '{0}'", this.NormalizeOutput.ToString()));
            sb.Append(String.Format("    Debug: '{0}'", this.Debug.ToString()));
#if !__MonoCS__ // this seems to hang in the MonoDevelop debugger for some reason...
           DirectableEncConverter aDEC = new DirectableEncConverter(this);
            if (aDEC.IsLhsLegacy)
                sb.Append(String.Format("{1}    Input Code Page: '{0}'", aDEC.CodePageInput, Environment.NewLine));
            if (aDEC.IsRhsLegacy)
                sb.Append(String.Format("{1}    Output Code Page: '{0}'", aDEC.CodePageOutput, Environment.NewLine));
#endif
            return sb.ToString();
        }

        protected string strProcessType(long lProcessType)
        {
            StringBuilder sb = new StringBuilder();
            if( (lProcessType & (long)ProcessTypeFlags.UnicodeEncodingConversion) != 0 )
                sb.Append("UnicodeEncodingConversion, ");
            if( (lProcessType & (long)ProcessTypeFlags.Transliteration) != 0 )
                sb.Append("Transliteration, ");
            if( (lProcessType & (long)ProcessTypeFlags.ICUTransliteration) != 0 )
                sb.Append("ICUTransliteration, ");
            if( (lProcessType & (long)ProcessTypeFlags.ICUConverter) != 0 )
                sb.Append("ICUConverter, ");
            if( (lProcessType & (long)ProcessTypeFlags.ICURegularExpression) != 0 )
                sb.Append("ICURegularExpression, ");
            if( (lProcessType & (long)ProcessTypeFlags.CodePageConversion) != 0 )
                sb.Append("CodePageConversion, ");
            if( (lProcessType & (long)ProcessTypeFlags.NonUnicodeEncodingConversion) != 0 )
                sb.Append("NonUnicodeEncodingConversion, ");
            if( (lProcessType & (long)ProcessTypeFlags.SpellingFixerProject) != 0 )
                sb.Append("SpellingFixerProject, ");
            if( (lProcessType & (long)ProcessTypeFlags.PythonScript) != 0 )
                sb.Append("PythonScript, ");
            if( (lProcessType & (long)ProcessTypeFlags.PerlExpression) != 0 )
                sb.Append("PerlExpression, ");
            if( (lProcessType & (long)ProcessTypeFlags.UserDefinedSpare1) != 0 )
                sb.Append("UserDefinedSpare #1, ");
            if( (lProcessType & (long)ProcessTypeFlags.UserDefinedSpare2) != 0 )
                sb.Append("UserDefinedSpare #2, ");

            // strip off the final ", "
            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        protected string FormatTabbedTip(string strFormat, string strValue)
        {
            return Environment.NewLine + "    " + String.Format(strFormat, strValue);
        }

        protected abstract string   GetConfigTypeName
        {
            get;
        }

        #endregion Public Interface

        #region Internal Helpers
        // Since each sub-class has to do basic input/output encoding format processing, they
        //  should all mostly come thru this and the next functions.
        protected virtual string InternalConvert
            (
            EncodingForm    eInEncodingForm,
            string          sInput,
            EncodingForm    eOutEncodingForm,
            NormalizeFlags  eNormalizeOutput,
            bool            bForward
            )
        {
            // this routine is only called by one of the 'implicit' methods (e.g.
            //  ConvertToUnicode). For these "COM standard" methods, the length of the string
            //  is specified by the BSTR itself and always/only supports UTF-16-like (i.e. wide)
            //  data. So, pass 0 so that the function will determine the length from the BSTR
            //  itself (just in case the user happens to have a value of 0 in the data (i.e.
            //  it won't necessarily be null terminated...
            int   ciOutput = 0;
            return InternalConvertEx
                (
                eInEncodingForm,
                sInput,
                0,
                eOutEncodingForm,
                eNormalizeOutput,
                out ciOutput,
                bForward
                );
        }

        /// legacy data as a byte array as input, we need to treat it as a byte array.
        /// </summary>
        protected virtual unsafe string InternalConvertEx(EncodingForm eInEncodingForm,
            byte[] baInput,
            EncodingForm eOutEncodingForm,
            NormalizeFlags eNormalizeOutput,
            out int rciOutput,
            bool bForward)
        {
            Util.DebugWriteLine(this, "(input bytes) BEGIN");
            if( baInput == null )
                EncConverters.ThrowError(ErrStatus.IncompleteChar);
            if (baInput.Length == 0) {
                rciOutput = 0;
                return "";
            }
            // if the user hasn't specified, then take the default case for the ConversionType:
            //  if L/RHS == eLegacy, then LegacyString
            //  if L/RHS == eUnicode, then UTF16
            CheckInitEncForms(bForward, ref eInEncodingForm, ref eOutEncodingForm);

            // allow the converter engine's (and/or its COM wrapper) to do some preprocessing.
            EncodingForm eFormEngineIn = EncodingForm.Unspecified, eFormEngineOut = EncodingForm.Unspecified;
            PreConvert(
                eInEncodingForm,        // [in] form in the BSTR
                ref eFormEngineIn,      // [out] form the conversion engine wants, etc.
                eOutEncodingForm,
                ref eFormEngineOut,
                ref eNormalizeOutput,
                bForward);
            int nBufSize = baInput.Length;
            fixed (byte* lpInBuffer = baInput)
            {
                int nOutLen = Math.Max(10000, nBufSize * 6);
                byte[] abyOutBuffer = new byte[nOutLen];
                fixed (byte* lpOutBuffer = abyOutBuffer)
                {
                    lpOutBuffer[0] = lpOutBuffer[1] = lpOutBuffer[2] = lpOutBuffer[3] = 0;

                    // call the wrapper sub-classes' DoConvert to let them do it.
                    Util.DebugWriteLine(this, "Calling DoConvert");
                    DoConvert(lpInBuffer, nBufSize, lpOutBuffer, ref nOutLen);
                    Util.DebugWriteLine(this, "Output length " + nOutLen.ToString());

                    byte[] baOut = new byte[nOutLen];
                    ECNormalizeData.ByteStarToByteArr(lpOutBuffer, nOutLen, baOut);
#if DEBUG
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Output In Bytes", baOut));
                    Util.DebugWriteLine(this, "Got val '" + System.Text.Encoding.Unicode.GetString(baOut) + "'");
#endif
                    string result = ECNormalizeData.GetString(lpOutBuffer, nOutLen, eOutEncodingForm,
                        ((bForward) ? CodePageOutput : CodePageInput), eFormEngineOut, eNormalizeOutput,
                        out rciOutput, ref m_bDebugDisplayMode);
#if DEBUG
                    Util.DebugWriteLine(this, "normalized result '" + result + "'");
                    byte[] baResult = System.Text.Encoding.BigEndianUnicode.GetBytes(result);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Normalized Output in UTF16BE", baResult));
                    baResult = System.Text.Encoding.Unicode.GetBytes(result);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Normalized Output in UTF16LE", baResult));
                    baResult = System.Text.Encoding.UTF8.GetBytes(result);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Normalized Output In UTF8", baResult));
                    Util.DebugWriteLine(this, "Returning.");
#endif
                    return result;
                }
            }
        }

        /// <summary>
        /// If we're returning legacy data as a byte array, we need to return it as a byte array.
        /// </summary>
        /// <returns>
        protected virtual unsafe byte[] InternalConvertEx(EncodingForm eInEncodingForm,
            string sInput,
            EncodingForm eOutEncodingForm,
            NormalizeFlags eNormalizeOutput,
            out int rciOutput,
            bool bForward)
        {
            Util.DebugWriteLine(this, "(output bytes) BEGIN");
            if( sInput == null )
                EncConverters.ThrowError(ErrStatus.IncompleteChar);
            Util.DebugWriteLine(this, "sInput.Length() is " + sInput.Length.ToString() + ".");
            if (sInput.Length == 0) {
                // this section added 11/10/2011 by Jim K
                rciOutput = 0;
                return new byte[0];
            }
            Util.DebugWriteLine(this, "sInput is " + sInput + ".");
            // if the user hasn't specified, then take the default case for the ConversionType:
            //  if L/RHS == eLegacy, then LegacyString
            //  if L/RHS == eUnicode, then UTF16
            CheckInitEncForms(bForward, ref eInEncodingForm, ref eOutEncodingForm);

            // allow the converter engine's (and/or its COM wrapper) to do some preprocessing.
            EncodingForm eFormEngineIn = EncodingForm.Unspecified, eFormEngineOut = EncodingForm.Unspecified;
            PreConvert(
                eInEncodingForm,    // [in] form in the BSTR
                ref eFormEngineIn,  // [out] form the conversion engine wants, etc.
                eOutEncodingForm,
                ref eFormEngineOut,
                ref eNormalizeOutput,
                bForward);
            // get enough space for us to normalize the input data (6x ought to be enough)
            int nBufSize = sInput.Length * 6;
            byte[] abyInBuffer = new byte[nBufSize];
            fixed (byte* lpInBuffer = abyInBuffer)
            {
                // use a helper class to normalize the data to the format needed by the engine
                Util.DebugWriteLine(this, "Calling GetBytes");
                ECNormalizeData.GetBytes(sInput, sInput.Length, eInEncodingForm,
                    ((bForward) ? CodePageInput : CodePageOutput), eFormEngineIn, lpInBuffer,
                    ref nBufSize, ref m_bDebugDisplayMode);

                // get some space for the converter to fill with, but since this is allocated
                //  on the stack, don't muck around; get 10000 bytes for it.
                int nOutLen = Math.Max(10000, nBufSize * 6);
                byte[] abyOutBuffer = new byte[nOutLen];
                fixed (byte* lpOutBuffer = abyOutBuffer)
                {
                    lpOutBuffer[0] = lpOutBuffer[1] = lpOutBuffer[2] = lpOutBuffer[3] = 0;

                    // call the wrapper sub-classes' DoConvert to let them do it.
                    Util.DebugWriteLine(this, "Calling DoConvert");
                    DoConvert(lpInBuffer, nBufSize, lpOutBuffer, ref nOutLen);
                    byte[] baOut = new byte[nOutLen];
                    ECNormalizeData.ByteStarToByteArr(lpOutBuffer, nOutLen, baOut);
#if DEBUG
                    Util.DebugWriteLine(this, "Output length " + nOutLen.ToString());
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Output In Bytes", baOut));
                    Util.DebugWriteLine(this, "Returning.");
#endif
                	rciOutput = nOutLen;
                    return baOut;
                }
            }
        }

        // This function is the meat of the conversion process. It is really long, which
        //	normally wouldn't be a virtue (especially as an "in-line" function), but in an
        //	effort to save memory fragmentation by using stack memory to buffer the input
        //	and output data, I'm using the alloca memory allocation function. Because of this
        //	it can't be allocated in some subroutine and returned to a calling program (or the
        //	stack will have erased them), so it has to be one big fat long function...
        //	The basic structure is:
        //
        //	o	Check Input Data
        //  o   Give the sub-class (via PreConvert) the opportunity to load tables and do
        //      any special preprocessing it needs to ahead of the actual conversion
        //  o   Possibly call the TECkit COM interface to convert Unicode flavors that the
        //      engine (for this conversion) might not support (indicated via PreConvert)
        //  o   Normalize the input data to a byte array based on it's input EncodingForm
        //  o       Allocate (on the stack) a buffer for the output data (min 10000 bytes)
        //  o       Call the subclass (via DoConvert) to do the actual conversion.
        //  o   Normalize the output data to match the requested output EncodingForm (including
        //      possibly calling the TECkit COM interface).
        //  o   Return the resultant BSTR and size of items to the output pointer variables.
        //
        protected virtual unsafe string InternalConvertEx
            (
            EncodingForm    eInEncodingForm,
            string        	sInput,
            int             ciInput,
            EncodingForm    eOutEncodingForm,
            NormalizeFlags  eNormalizeOutput,
            out int         rciOutput,
            bool            bForward
            )
        {
            Util.DebugWriteLine(this, "BEGIN");
            if( sInput == null )
                EncConverters.ThrowError(ErrStatus.IncompleteChar);
            if (sInput.Length == 0) {
                rciOutput = 0;
                return "";
            }

#if DEBUG && __MonoCS__
// for debugging only BEGIN
            //byte[] baIn = System.Text.Encoding.UTF8.GetBytes(sInput);            // works
            byte[] baIn = System.Text.Encoding.BigEndianUnicode.GetBytes(sInput);  // easier to read
            Util.DebugWriteLine(this, Util.getDisplayBytes("Input BigEndianUnicode", baIn));
            baIn = System.Text.Encoding.Unicode.GetBytes(sInput);
            Util.DebugWriteLine(this, Util.getDisplayBytes("Input Unicode", baIn));

            int nInLen = sInput.Length;
            byte [] baIn2 = new byte[nInLen];
            for (int i = 0; i < nInLen; i++)
                baIn2[i] = (byte)(sInput[i] & 0xFF);
            Util.DebugWriteLine(this, Util.getDisplayBytes("Input Narrowized", baIn2));
/*
            System.Text.Encoding encFrom = System.Text.Encoding.GetEncoding(12000);
            System.Text.Encoding encTo   = System.Text.Encoding.UTF8;

            // Perform the conversion from one encoding to the other.
            Util.DebugWriteLine(this, "Starting with " + baIn.Length.ToString() + " bytes.");
            byte[] baOut2 = System.Text.Encoding.Convert(encFrom, encTo, baIn);
            Util.DebugWriteLine(this, "Converted to " + baOut2.Length.ToString() + " bytes.");
            string resultString = System.Text.Encoding.Default.GetString(baOut2, 0, baOut2.Length);
            Util.DebugWriteLine(this, "Test output '" + resultString + "'");
*/
// for debugging only END
#endif

            // if the user hasn't specified, then take the default case for the ConversionType:
            //  if L/RHS == eLegacy, then LegacyString
            //  if L/RHS == eUnicode, then UTF16
            CheckInitEncForms
                (
                bForward,
                ref eInEncodingForm,
                ref eOutEncodingForm
                );

            // allow the converter engine's (and/or its COM wrapper) to do some preprocessing.
            EncodingForm eFormEngineIn = EncodingForm.Unspecified, eFormEngineOut = EncodingForm.Unspecified;
            PreConvert
                (
                eInEncodingForm,	// [in] form in the BSTR
                ref eFormEngineIn,  // [out] form the conversion engine wants, etc.
                eOutEncodingForm,
                ref eFormEngineOut,
                ref eNormalizeOutput,
                bForward
                );

            // get enough space for us to normalize the input data (6x ought to be enough)
            int nBufSize = sInput.Length * 6;
            byte[] abyInBuffer = new byte[nBufSize];
            fixed (byte* lpInBuffer = abyInBuffer)
            {
                // use a helper class to normalize the data to the format needed by the engine
                Util.DebugWriteLine(this, "Calling GetBytes");
                ECNormalizeData.GetBytes(sInput, ciInput, eInEncodingForm,
                    ((bForward) ? CodePageInput : CodePageOutput), eFormEngineIn, lpInBuffer,
                    ref nBufSize, ref m_bDebugDisplayMode);
#if DEBUG && __MonoCS__
                	byte[] baOut = new byte[nBufSize];
                    ECNormalizeData.ByteStarToByteArr(lpInBuffer, nBufSize, baOut);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Input Bytes", baOut));
#endif

                // get some space for the converter to fill with, but since this is allocated
                //  on the stack, don't muck around; get 10000 bytes for it.
                int nOutLen = Math.Max(10000, nBufSize * 6);
                byte[] abyOutBuffer = new byte[nOutLen];
                fixed (byte* lpOutBuffer = abyOutBuffer)
                {
                    lpOutBuffer[0] = lpOutBuffer[1] = lpOutBuffer[2] = lpOutBuffer[3] = 0;

                    // call the wrapper sub-classes' DoConvert to let them do it.
                    Util.DebugWriteLine(this, "Calling DoConvert");
                    DoConvert(lpInBuffer, nBufSize, lpOutBuffer, ref nOutLen);
#if DEBUG && __MonoCS__
                    Util.DebugWriteLine(this, "Output length " + nOutLen.ToString());
                	byte[] baOut2 = new byte[nOutLen];
                    ECNormalizeData.ByteStarToByteArr(lpOutBuffer, nOutLen, baOut2);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Output In Bytes", baOut2));
                    Util.DebugWriteLine(this, "Got val '" + System.Text.Encoding.Unicode.GetString(baOut2) + "'");
#endif
                    string result = ECNormalizeData.GetString(lpOutBuffer, nOutLen, eOutEncodingForm,
                        ((bForward) ? CodePageOutput : CodePageInput), eFormEngineOut, eNormalizeOutput,
                        out rciOutput, ref m_bDebugDisplayMode);
#if DEBUG && __MonoCS__
                    Util.DebugWriteLine(this, "normalized result '" + result + "'");
                    byte[] baResult = System.Text.Encoding.BigEndianUnicode.GetBytes(result);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Normalized Output in UTF16BE", baResult));
                    baResult = System.Text.Encoding.Unicode.GetBytes(result);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Normalized Output in UTF16LE", baResult));
                    baResult = System.Text.Encoding.UTF8.GetBytes(result);
                    Util.DebugWriteLine(this, Util.getDisplayBytes("Normalized Output In UTF8", baResult));
                    Util.DebugWriteLine(this, "Returning.");
#endif
                    return result;
                }
            }
        }

        protected void CheckInitEncForms
            (
            bool                bForward,
            ref EncodingForm    eInEncodingForm,
            ref EncodingForm    eOutEncodingForm
            )
        {
            // if the user hasn't specified, then take the default case for the ConversionType:
            //  if L/RHS == eLegacy, then LegacyString
            //  if L/RHS == eUnicode, then UTF16
            if( eInEncodingForm == EncodingForm.Unspecified )
            {
                NormConversionType eType;
                if( bForward )
                    eType = NormalizeLhsConversionType(m_eConversionType);
                else
                    eType = NormalizeRhsConversionType(m_eConversionType);

                if( eType == NormConversionType.eLegacy )
                    eInEncodingForm = EncodingForm.LegacyString;
                else // eUnicode
                    eInEncodingForm = DefaultUnicodeEncForm(bForward,true);
            }

            // do the same for the output form
            if( eOutEncodingForm == EncodingForm.Unspecified )
            {
                NormConversionType eType;
                if( bForward )
                    eType = NormalizeRhsConversionType(m_eConversionType);
                else
                    eType = NormalizeLhsConversionType(m_eConversionType);

                if( eType == NormConversionType.eLegacy )
                    eOutEncodingForm = EncodingForm.LegacyString;
                else // eUnicode
                    eOutEncodingForm = DefaultUnicodeEncForm(bForward,false);
            }

            CheckForBadForm(bForward, eInEncodingForm, eOutEncodingForm);
        }

        protected void CheckForBadForm
            (
            bool            bForward,
            EncodingForm    inEnc,
            EncodingForm    outEnc
            )
        {
            if( EncConverters.IsUnidirectional(m_eConversionType) && !bForward )
            {
                EncConverters.ThrowError(ErrStatus.InvalidConversionType);
            }
            else
            {
                bool bLhsUnicode = (NormalizeLhsConversionType(m_eConversionType) == NormConversionType.eUnicode);
                bool bRhsUnicode = (NormalizeRhsConversionType(m_eConversionType) == NormConversionType.eUnicode);
                if( bForward )
                {
                    if( bLhsUnicode )
                    {
                        if( IsLegacyFormat(inEnc) )
                            EncConverters.ThrowError(ErrStatus.InEncFormNotSupported);
                    }
                    else    // !bLhsUnicode
                    {
                        if( !IsLegacyFormat(inEnc) )
                            EncConverters.ThrowError(ErrStatus.InEncFormNotSupported);
                    }
                    if( bRhsUnicode )
                    {
                        if( IsLegacyFormat(outEnc) )
                            EncConverters.ThrowError(ErrStatus.OutEncFormNotSupported);
                    }
                    else    // !bRhsUnicode
                    {
                        if( !IsLegacyFormat(outEnc) )
                            EncConverters.ThrowError(ErrStatus.OutEncFormNotSupported);
                    }
                }
                else    // reverse
                {
                    if( bLhsUnicode )
                    {
                        if( IsLegacyFormat(outEnc) )
                            EncConverters.ThrowError(ErrStatus.OutEncFormNotSupported);
                    }
                    else    // !bLhsUnicode
                    {
                        if( !IsLegacyFormat(outEnc) )
                            EncConverters.ThrowError(ErrStatus.OutEncFormNotSupported);
                    }
                    if( bRhsUnicode )
                    {
                        if( IsLegacyFormat(inEnc) )
                            EncConverters.ThrowError(ErrStatus.InEncFormNotSupported);
                    }
                    else    // !bRhsUnicode
                    {
                        if( !IsLegacyFormat(inEnc) )
                            EncConverters.ThrowError(ErrStatus.InEncFormNotSupported);
                    }
                }
            }
        }

        public static bool IsLegacyFormat(EncodingForm eForm)
        {
            return ((eForm == EncodingForm.LegacyString)
                ||  (eForm == EncodingForm.LegacyBytes));
        }

        // some converters (e.g. cc) use a different Unicode form as basic
        protected virtual EncodingForm  DefaultUnicodeEncForm(bool bForward, bool bLHS)
        {
            return EncodingForm.UTF16;
        }

        protected bool DoesFileExist(string strFileName, ref DateTime TimeModified)
        {
            bool bRet = true;

            try
            {
                FileInfo fi = new FileInfo(strFileName);
                TimeModified = fi.LastWriteTime;
                bRet = fi.Exists;
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }

        public static NormConversionType NormalizeLhsConversionType(ConvType type)
        {
            NormConversionType eType = NormConversionType.eUnicode;
            switch(type)
            {
                case ConvType.Unicode_to_from_Legacy:
                case ConvType.Unicode_to_from_Unicode:
                case ConvType.Unicode_to_Legacy:
                case ConvType.Unicode_to_Unicode:
                    eType = NormConversionType.eUnicode;
                    break;

                case ConvType.Legacy_to_from_Legacy:
                case ConvType.Legacy_to_from_Unicode:
                case ConvType.Legacy_to_Legacy:
                case ConvType.Legacy_to_Unicode:
                    eType = NormConversionType.eLegacy;
                    break;

                default:
                    // Debug.Assert();  // doesn't work for some reason!!??
                    break;
            };

            return eType;
        }

        public static NormConversionType NormalizeRhsConversionType(ConvType type)
        {
            NormConversionType eType = NormConversionType.eUnicode;
            switch(type)
            {
                case ConvType.Legacy_to_from_Legacy:
                case ConvType.Legacy_to_Legacy:
                case ConvType.Unicode_to_from_Legacy:
                case ConvType.Unicode_to_Legacy:
                    eType = NormConversionType.eLegacy;
                    break;

                case ConvType.Legacy_to_from_Unicode:
                case ConvType.Legacy_to_Unicode:
                case ConvType.Unicode_to_from_Unicode:
                case ConvType.Unicode_to_Unicode:
                    eType = NormConversionType.eUnicode;
                    break;

                default:
                    // Debug.Assert();  // doesn't work for some reason!!??
                    break;
            };

            return eType;
        }
        #endregion Internal Helpers

        #region Virtual Functions implemented by subclasses
        protected virtual void PreConvert
            (
            EncodingForm        eInEncodingForm,
            ref EncodingForm    eInFormEngine,
            EncodingForm        eOutEncodingForm,
            ref EncodingForm    eOutFormEngine,
            ref NormalizeFlags  eNormalizeOutput,
            bool                bForward
            )
        {
            // by default, the form it comes in is okay for the engine (never really true, so
            //  each engine's COM wrapper must override this; but this is here to see what you
            //  must do). For example, for CC, the input must be UTF8Bytes for Unicode, so
            //  you'd set the eInFormEngine to UTF8Bytes.
            eInFormEngine = eInEncodingForm;
            eOutFormEngine = eOutEncodingForm;
        }

        // this is where the sub-classes do the actual work... i.e. override this one for sure.

        [CLSCompliant(false)]
        protected virtual unsafe void DoConvert
            (
            byte*       lpInBuffer,
            int         nInLen,
            byte*       lpOutBuffer,
            ref int     rnOutLen
            )
        {
            // Debug.Assert(true);
        }

        // sub-classes override to add their attributes.
        protected virtual void GetAttributeKeys(out string [] rSa)
        {
            rSa = null;
        }

        // sub-classes override to add their attributes.
        protected virtual void GetConverterNameEnum(out string [] rSa)
        {
            rSa = null;
        }
        #endregion Virtual Functions implemented by subclasses
    }
}
