// #define encryptingNewCredentials

namespace SilEncConverters40.EcTranslators.VertexAi
{
	public class VertexAiPromptExeTranslatorCommandLineArgs : PromptExeTranslatorCommandLineArgs
	{
		public string ProjectId { get; set; }
		public string LocationId { get; set; }
		public string Publisher { get; set; }
		public string ModelId { get; set; }
		public double? Temperature { get; set; } = 0.3; // the default from vertex example
		public int? MaxDecodeSteps { get; set; } = 200; // default 
		public double? TopP { get; set; } = 0.8;        // default
		public int? TopK { get; set; } = 40;			// default
	}
}

