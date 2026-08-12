using ECInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SilEncConverters40.EncConverterHostExe
{
    /// <summary>
    /// Describes one IEncConverter to instantiate and drive to a single conversion, out-of-process.
    /// This is the same tuple IEncConverter.Initialize()/EncConverters.InstantiateIEncConverter()
    /// already require - see Handover.md, "Generic subprocess fallback for host-process conflicts",
    /// for the full rationale. Every field here is required to fully describe "which converter, how
    /// it's configured, and what to convert" without any dependency on the EncConverters
    /// dispatcher/repository.
    /// </summary>
    public class EncConverterHostRequest
    {
        /// <summary>Full assembly display name, e.g. "SilIndicEncConverters40, Version=4.0.0.0,
        /// Culture=neutral, PublicKeyToken=..." - passed directly to Activator.CreateInstance so the
        /// assembly is resolved by normal .NET probing (must sit next to this exe, or otherwise be
        /// resolvable), no registry/COM involved.</summary>
        public string AssemblyReference { get; set; }

        /// <summary>The "ProgID" in EncConverters' sense - actually the fully-qualified .NET type name
        /// of the implementation class, e.g. "SilEncConverters40.TechHindiSiteEncConverter" (every
        /// EncConverter subclass passes typeof(X).FullName as its own ProgID).</summary>
        public string ProgId { get; set; }

        /// <summary>Friendly name / collection key - IEncConverter.Initialize()'s converterName.</summary>
        public string ConverterName { get; set; }

        /// <summary>The opaque, converter-type-specific configuration string - IEncConverter.Initialize()'s
        /// ConverterIdentifier. Syntax is owned entirely by whichever converter type this targets (e.g.
        /// a CC table path, an ICU transliterator ID, or TechHindiSiteEncConverter's semicolon-delimited
        /// URI;inputId;outputId;fn;[reverseFn];[browser] spec) - this program never parses it itself.</summary>
        public string ConverterIdentifier { get; set; }

        public string LhsEncodingId { get; set; }
        public string RhsEncodingId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ConvType ConversionType { get; set; }

        public int ProcessTypeFlags { get; set; }
        public int CodePageInput { get; set; }
        public int CodePageOutput { get; set; }

        /// <summary>Defaults to true (forward) if not specified.</summary>
        public bool DirectionForward { get; set; } = true;

        [JsonConverter(typeof(StringEnumConverter))]
        public NormalizeFlags NormalizeOutput { get; set; } = NormalizeFlags.None;

        /// <summary>The actual data to convert. Ignored when KeepAliveSeconds &gt; 0 - in that mode
        /// every conversion, including the first, is sent as a pipe request (see
        /// EncConverterHostPipeRequest) once the session is established.</summary>
        public string InputText { get; set; }

        /// <summary>0 (default) = today's exact one-shot behavior: convert InputText once and exit.
        /// &gt;0 = skip converting InputText here entirely; instead, after Initialize(), stay resident
        /// and serve every Convert() call - including the first - over a named pipe (see
        /// SessionPipeName) against the SAME already-initialized converter instance, self-terminating
        /// if idle this long with no pipe activity. See Handover.md, "Generic subprocess fallback for
        /// host-process conflicts", for the full rationale, including why nothing is ever written to
        /// stdout in this mode (an earlier attempt tried signaling "first result ready, but still
        /// running" via closing Console.Out - confirmed empirically that doesn't reliably work on
        /// Windows/.NET). Windows-only (PipeTransmissionMode.Message is Windows-only) - consistent
        /// with this whole subsystem.</summary>
        public int KeepAliveSeconds { get; set; } = 0;

        /// <summary>Required when KeepAliveSeconds &gt; 0. A pipe name the CALLER generates (e.g. a
        /// GUID) before launching this process, so no discovery round-trip is needed - the caller
        /// already knows what to connect a NamedPipeClientStream to.</summary>
        public string SessionPipeName { get; set; }
    }
}
