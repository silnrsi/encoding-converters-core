using ECInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SilEncConverters40.EncConverterHostExe
{
    /// <summary>
    /// The per-session pipe envelope used once a keep-alive session (EncConverterHostRequest.
    /// KeepAliveSeconds > 0) is established - a distinct wire concept from EncConverterHostRequest
    /// itself (which only ever describes the ONE launch/first call). See Handover.md, "Generic
    /// subprocess fallback for host-process conflicts", for the full protocol.
    /// </summary>
    public class EncConverterHostPipeRequest
    {
        /// <summary>The next string to convert.</summary>
        public string InputText { get; set; }

        /// <summary>null (default) = leave the converter's current DirectionForward as-is; otherwise
        /// override it for this call (and it stays that way for subsequent calls too, until changed
        /// again - same semantics as setting the property directly on a live IEncConverter).</summary>
        public bool? DirectionForward { get; set; }

        /// <summary>null (default) = leave NormalizeOutput as-is; see DirectionForward for the same
        /// "sticky until changed" semantics.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NormalizeFlags? NormalizeOutput { get; set; }

        /// <summary>Caller-requested graceful exit - acknowledged with one final response, then the
        /// process exits on its own (distinct from the idle-timeout exit path).</summary>
        public bool Shutdown { get; set; }
    }

    public class EncConverterHostPipeResponse
    {
        public bool Success { get; set; }

        /// <summary>The converted string. Only meaningful when Success is true.</summary>
        public string Result { get; set; }

        /// <summary>Only meaningful when Success is false. Unlike the one-shot path's stderr, this is
        /// the ONLY error channel once a session is in its pipe loop - stderr is already closed by
        /// then (see Program.cs / Handover.md for why).</summary>
        public string ErrorMessage { get; set; }

        /// <summary>Process.GetCurrentProcess().Id of the server answering this request - purely a
        /// verification aid, so a caller/test can assert the SAME process served every call in a
        /// session rather than inferring reuse from timing.</summary>
        public int ServerProcessId { get; set; }

        // Populated only on the one-time handshake response Program.RunKeepAliveLoop sends right after
        // accepting the connection, before any conversion request - null on every response after that.
        // Reports back whatever the real converter's own Initialize() adjusted in the child (e.g.
        // TechHindiSiteEncConverter.MakeUniDirectional, applied when no reverse function is configured)
        // via the SAME ref parameters IEncConverter.Initialize()'s own contract already provides for
        // this. Confirmed necessary, not theoretical: without this, EncConverters.AddEx's own subprocess
        // fallback (which substitutes a SubprocessEncConverter proxy *before* the real converter's
        // Initialize() has ever run - see AddEx's doc comments) left the proxy stuck reporting the
        // caller's original, pre-adjustment ConversionType forever, which broke a later, unrelated
        // uni-directional check (TestTechHindiSiteConverter's reverse-conversion assertion) with
        // "The configured conversion type (Unicode vs. non-Unicode) is invalid for this converter" -
        // the proxy claimed to be bidirectional when the real (child-side) converter had already
        // correctly downgraded itself to uni-directional.
        public string AdjustedLhsEncodingId { get; set; }
        public string AdjustedRhsEncodingId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ConvType? AdjustedConversionType { get; set; }
        public int? AdjustedProcessTypeFlags { get; set; }
    }
}
