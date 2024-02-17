// #define encryptingNewCredentials

namespace SilEncConverters40.EcTranslators.AzureOpenAI
{
	public class AzureOpenAiPromptExeTranslatorCommandLineArgs : PromptExeTranslatorCommandLineArgs
	{
		public string DeploymentId { get; set; }
		public string EndpointId { get; set; }

		/// <summary>
		/// What sampling temperature to use, between 0 and 2. Higher values means the model will take more risks. Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer. We generally recommend altering this or top_p but not both.
		/// </summary>
		public double? Temperature { get; set; } = 1;   // (from 0-2) the default per https://learn.microsoft.com/en-us/azure/ai-services/openai/reference#chat-completions

		/// <summary>
		/// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. We generally recommend altering this or temperature but not both.
		/// </summary>
		public double? TopP { get; set; } = 1;        // default
	}
}

