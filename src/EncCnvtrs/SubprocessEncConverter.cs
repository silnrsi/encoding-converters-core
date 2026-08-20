using System;
using System.Text;

using ECInterfaces;

namespace SilEncConverters40
{
    /// <summary>
    /// The subprocess-backed IEncConverter proxy for the generic subprocess fallback (see Handover.md,
    /// "Generic subprocess fallback for host-process conflicts"). EncConverters.AddEx constructs one of
    /// these -- built from the same (assembly, ProgID, converterName, converterIdentifier, ...) tuple
    /// Initialize() would otherwise have used -- in place of initializing a real converter in-process,
    /// whenever that converter's RiskyNativeDependencies conflict with something already resident in
    /// this host process (or previously did, per the registry cache).
    /// Deliberately implements IEncConverter directly rather than deriving from the EncConverter base
    /// class: EncConverter's Convert()/ConvertEx() machinery (InternalConvertEx, PreConvert, DoConvert)
    /// operates on raw byte buffers and per-engine EncodingForm decisions that only make sense for a
    /// converter actually doing the conversion itself. This proxy never does -- it forwards a wide
    /// string to the real converter running in the child process (which goes through that exact
    /// machinery there, correctly, using the real engine) and returns whatever wide string comes back.
    /// Reusing EncConverter's byte-buffer plumbing here would mean re-deriving the same encoding-form
    /// logic a second time for no benefit; a thin pass-through is simpler and has less to get wrong.
    /// </summary>
    internal sealed class SubprocessEncConverter : IEncConverter, IDisposable
    {
        private readonly string _hostExePath;
        private readonly string _assemblyReference;
        private readonly string _progId;

        private PersistentEncConverterHostSession _session;

        private string _converterIdentifier;
        private string _lhsEncodingId;
        private string _rhsEncodingId;
        private ConvType _conversionType;
        private int _processType;
        private int _codePageInput;
        private int _codePageOutput;
        private bool _directionForward = true;
        private NormalizeFlags _normalizeOutput = NormalizeFlags.None;

        /// <param name="hostExePath">Full path to the one canonical, installed copy of
        /// EncConverterHostExe.exe (see EncConverters.FindEncConverterHostExePath) -- never a
        /// caller-relative or per-plugin copy; see Handover.md for why.</param>
        /// <param name="assemblyReference">Full assembly display name the real converter type is
        /// defined in -- typically rConverter.GetType().Assembly.FullName from the already-constructed
        /// (but never Initialize()'d) real instance AddEx is discarding in favor of this proxy.</param>
        /// <param name="progId">The real converter's fully-qualified .NET type name (same value as its
        /// own ProgramID / typeof(X).FullName).</param>
        public SubprocessEncConverter(string hostExePath, string assemblyReference, string progId)
        {
            _hostExePath = hostExePath;
            _assemblyReference = assemblyReference;
            _progId = progId;
        }

        public string Name { get; set; }

        public void Initialize(string converterName, string converterIdentifier, ref string lhsEncodingId,
            ref string rhsEncodingId, ref ConvType conversionType, ref int processTypeFlags,
            int codePageInput, int codePageOutput, bool bAdding)
        {
            Name = converterName;
            _converterIdentifier = converterIdentifier;
            _lhsEncodingId = lhsEncodingId;
            _rhsEncodingId = rhsEncodingId;
            _conversionType = conversionType;
            _processType = processTypeFlags;
            _codePageInput = codePageInput;
            _codePageOutput = codePageOutput;

            // bAdding's documented contract ("attempt to instantiate the converter to check for syntax,
            // etc") is honored by launching the session eagerly here (which drives the real converter's
            // own Initialize() in the child process) rather than deferring to the first Convert() call,
            // regardless of bAdding -- the subprocess launch this proxy exists to route through is
            // already far more expensive than a normal in-process check, so there's no separate "cheap"
            // path worth preserving the way the real converters' own bAdding=false optimization does.
            _session?.Dispose();
            _session = new PersistentEncConverterHostSession(_hostExePath, _assemblyReference, _progId,
                converterName, converterIdentifier, lhsEncodingId, rhsEncodingId,
                conversionType, processTypeFlags, codePageInput, codePageOutput);
        }

        public string ConverterIdentifier => _converterIdentifier;
        public string ProgramID => _progId;
        public string ImplementType => "Subprocess";
        public ConvType ConversionType => _conversionType;

        public int ProcessType
        {
            get => _processType;
            set => _processType = value;
        }

        public int CodePageInput
        {
            get => _codePageInput;
            set => _codePageInput = value;
        }

        public int CodePageOutput
        {
            get => _codePageOutput;
            set => _codePageOutput = value;
        }

        public string LeftEncodingID => _lhsEncodingId;

        public string RightEncodingID
        {
            get => _rhsEncodingId;
            set => _rhsEncodingId = value;
        }

        // no GUI configuration surface for a transparent runtime fallback -- the real converter's own
        // Configurator (for the mapping this is standing in for) is what a user would configure anyway.
        public IEncConverterConfig Configurator => null;

        public bool IsInRepository { get; set; }

        public string ConvertToUnicode(byte[] baInput)
        {
            RequireConversionType(ConvType.Legacy_to_from_Unicode, ConvType.Unicode_to_from_Legacy, ConvType.Legacy_to_Unicode);
            var encoding = Encoding.GetEncoding(_codePageInput == 0 ? 1252 : _codePageInput);
            return Convert(encoding.GetString(baInput));
        }

        public byte[] ConvertFromUnicode(string sInput)
        {
            RequireConversionType(ConvType.Legacy_to_from_Unicode, ConvType.Unicode_to_from_Legacy, ConvType.Unicode_to_Legacy);
            var encoding = Encoding.GetEncoding(_codePageOutput == 0 ? 1252 : _codePageOutput);
            return encoding.GetBytes(Convert(sInput));
        }

        public string Convert(string sInput)
        {
            var result = _session.Convert(sInput, _directionForward, _normalizeOutput);
            AdoptAdjustedValuesFromSession();
            return result;
        }

        public string ConvertEx(string sInput, EncodingForm inEnc, int ciInput, EncodingForm outEnc,
            out int ciOutput, NormalizeFlags eNormalizeOutput, bool bForward)
        {
            var result = _session.Convert(sInput, bForward, eNormalizeOutput);
            AdoptAdjustedValuesFromSession();
            ciOutput = String.IsNullOrEmpty(result) ? 0 : result.Length;
            return result;
        }

        /// <summary>See EncConverterHostPipeResponse's Adjusted* doc comment: the first time a session
        /// is actually established, the child reports back whatever the real converter's own
        /// Initialize() adjusted (e.g. TechHindiSiteEncConverter's uni-directional detection when no
        /// reverse function is configured). Without this, a caller reading this proxy's own
        /// ConversionType/encoding IDs (e.g. to decide whether a reverse conversion is even valid) would
        /// see the original, pre-adjustment values forever, since AddEx's own subprocess fallback can
        /// substitute this proxy *before* the real converter's Initialize() ever runs.</summary>
        private void AdoptAdjustedValuesFromSession()
        {
            if (_session.AdjustedLhsEncodingId != null)
                _lhsEncodingId = _session.AdjustedLhsEncodingId;
            if (_session.AdjustedRhsEncodingId != null)
                _rhsEncodingId = _session.AdjustedRhsEncodingId;
            if (_session.AdjustedConversionType.HasValue)
                _conversionType = _session.AdjustedConversionType.Value;
            if (_session.AdjustedProcessTypeFlags.HasValue)
                _processType = _session.AdjustedProcessTypeFlags.Value;
        }

        public bool DirectionForward
        {
            get => _directionForward;
            set => _directionForward = value;
        }

        public EncodingForm EncodingIn { get; set; } = EncodingForm.Unspecified;
        public EncodingForm EncodingOut { get; set; } = EncodingForm.Unspecified;
        public bool Debug { get; set; }

        public NormalizeFlags NormalizeOutput
        {
            get => _normalizeOutput;
            set => _normalizeOutput = value;
        }

        // metadata attributes / enumeration are per-engine extras, not needed for the primary
        // in-process-conflict fix this proxy exists for; a real caller wanting these should go through
        // the real converter type directly outside the conflicting host, same as today.
        public string[] AttributeKeys => Array.Empty<string>();
        public string AttributeValue(string sKey) => null;
        public string[] ConverterNameEnum => Array.Empty<string>();

        public override string ToString()
        {
            return $"Subprocess-backed EncConverter for '{_progId}' (Name: '{Name}')";
        }

        private void RequireConversionType(params ConvType[] allowed)
        {
            foreach (var t in allowed)
            {
                if (_conversionType == t)
                    return;
            }
            EncConverters.ThrowError(ErrStatus.InvalidConversionType);
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
