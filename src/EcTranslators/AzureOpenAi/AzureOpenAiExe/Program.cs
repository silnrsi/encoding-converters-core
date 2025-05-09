#if DEBUG
#define LogResults
// #define DEBUG_LIVE
#endif

using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Security.Policy;
using OpenAI.Chat;

namespace SilEncConverters40.EcTranslators.AzureOpenAI.AzureOpenAiExe
{
    // for debugging: command line parameters are:
    // gpt-translator-encconverter https://azure-openai-encconverter.openai.azure.com/ 5bc... "Translate from Hindi into English."
    internal class Program
    {
#if LogResults
        private const string LogFilePath = @"C:\btmp\AzureOpenAiLog.txt";
#endif
		public const string AzureOpenAiFailureDueToContentFilter = "Omitted content due to a content filter flag.";

		private const string ResponsePrefix = "Free translation: ";
        private const string EnvVarNameDeploymentName = "EncConverters_AzureOpenAiDeploymentName";
        private const string EnvVarNameEndPoint = "EncConverters_AzureOpenAiEndpoint";
        private const string EnvVarNameKey = "EncConverters_AzureOpenAiKey";
		private static readonly char[] TrimmableChars = new char[] { '\r', '\n', ' ' };

        static async Task Main(string[] args)
        {
            // set the stdin/out to Unicode
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            string pathToEncryptedCommandLineParameterFile = null;
            if ((args.Length == 0) || !File.Exists((pathToEncryptedCommandLineParameterFile = args[0])))
            {
                Console.WriteLine(String.Format("Usage:{0}{0}{1} \"<PathToEncryptedCommandLineParameterFile>\"", Environment.NewLine, typeof(Program).Namespace));
                return;
            }

#if DEBUG_LIVE
            System.Diagnostics.Debug.Fail("Click Retry to debug AzureOpenAiExe");
#endif
            var contents = File.ReadAllText(pathToEncryptedCommandLineParameterFile);
            var json = EncryptionClass.Decrypt(contents);
            var arguments = JsonConvert.DeserializeObject<AzureOpenAiPromptExeTranslatorCommandLineArgs>(json);

            if (!IsValidParameters(arguments, out string deploymentName, out string endpoint, out string key, out string systemPrompt))
                return;

            try
            {
                await ProcessRequest(arguments, deploymentName, endpoint, key, systemPrompt);
            }
            catch (Exception ex)
            {
				Console.WriteLine(GetExceptionMessage(ex));
			}

			static string GetExceptionMessage(Exception ex)
			{
				var message = ex.Message;
				var msg = "Error occurred: " + message;
				while (ex.InnerException != null)
				{
					ex = ex.InnerException;
					if (message.Contains(ex.Message))
						continue;   // skip identical msgs
					message = ex.Message;
					msg += $"{Environment.NewLine}because: (InnerException): {message}";
				}
				return msg;
			}
		}

		private static async Task ProcessRequest(AzureOpenAiPromptExeTranslatorCommandLineArgs arguments, string deploymentName, string endpoint,
                                                 string key, string systemPrompt)
        {
            // Pass the deployment name you chose when you created/deployed the model in Azure OpenAI Studio.
            var azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

			var chatClient = azureClient.GetChatClient(deploymentName);

			// create the ChatMessages w/ the given systemPrompt
			var chatMessages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt)
            };

            // microsoft just does examples like they are previous communication w/ the model
            var numberOfExamples = arguments.ExamplesInputString.Count;
            for (int i = 0; i < numberOfExamples; i++)
            {
                chatMessages.Add(new UserChatMessage(arguments.ExamplesInputString[i]));
                chatMessages.Add(new AssistantChatMessage(arguments.ExamplesOutputString[i]));
            }

            var prefixMessages = chatMessages.Count;
			var chatCompletionOptions = new ChatCompletionOptions
			{
				Temperature = arguments.Temperature
			};

            // in case there are multiple lines (e.g. what Paratext will do if the verse has multiple paragraphs),
            //  process in a while loop
            var index = 0;
            List<string> input = null; // new List<string> { "वहाँ वह विश्राम के दिन प्रार्थना घर में जाकर लोगों को परमेश्वर का वचन सुनाने लगा। सब लोग सुनकर चकित हो गये।", "", "परंतु कई तो यह भी कहने लगे, “यह ज्ञान इसको कहाँ से आया!? और ऐसे सामर्थ्‍य के काम यह कैसे करता है, जिसकी चर्चा सब लोग कर रहे हैं!?" };
            while ((input != null && input.Count > index) || (input == null && Console.In.Peek() != -1))
			{
				var strInput = (input != null)
								? input[index++]
								: Console.ReadLine();
				if (String.IsNullOrEmpty(strInput?.Trim(TrimmableChars)))   // don't actually trim them, but just for the sake of finding nothing to translate...
					continue;

				// add the string to be translated to as the 'user' message
				chatMessages.Add(new UserChatMessage(strInput));

#if LogResults
				var json = JsonConvert.SerializeObject(chatCompletionOptions, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
				File.AppendAllText(LogFilePath, json + Environment.NewLine);
#endif

				// call the service to process the user msg based on the given system prompt
				var chatCompletions = await chatClient.CompleteChatAsync(chatMessages, chatCompletionOptions);

				// clean up and return the "assistent's response"
				var strOutput = ExtractResult(strInput, chatCompletions);

				// put that back in the chat, in case we are processing multiple lines
				//    (this'll make them 'related' for better translation)
				chatMessages.Add(new AssistantChatMessage(strOutput));
			}

			for (var i = prefixMessages; i < chatMessages.Count;)
            {
                var strInput = ((UserChatMessage)chatMessages[i++]).Content[0].Text;
                var strOutput = ((AssistantChatMessage)chatMessages[i++]).Content[0].Text;

#if LogResults
                File.AppendAllText(LogFilePath, string.Format("{1}=>{2}:{3}=>{4}{0}", Environment.NewLine, systemPrompt, i / 2, strInput, strOutput));
#endif
                // write the responses to the standard out to return it
                Console.WriteLine(strOutput);
            }

			static string ExtractResult(string strInput, System.ClientModel.ClientResult<ChatCompletion> chatCompletions)
			{
				switch (chatCompletions.Value.FinishReason)
				{
					case ChatFinishReason.Stop:
						return CleanString(strInput, chatCompletions.Value.Content[0].Text);
					case ChatFinishReason.Length:
						return "Incomplete model output due to MaxTokens parameter or token limit exceeded.";
					case ChatFinishReason.ContentFilter:
						return AzureOpenAiFailureDueToContentFilter;
					default:
						return chatCompletions.Value.FinishReason.ToString();
				};
			}
		}

        private static bool IsValidParameters(AzureOpenAiPromptExeTranslatorCommandLineArgs arguments, out string deploymentName, out string endpoint, out string key, out string systemPrompt)
        {
            if (arguments == null)
            {
                Console.WriteLine(String.Format("Usage:{0}{0}{1} \"<PathToEncryptedCommandLineParameterFile>", Environment.NewLine, typeof(Program).Namespace));
                deploymentName = endpoint = systemPrompt = key = null;
                return false;
            }

            deploymentName = arguments.DeploymentId;
            endpoint = arguments.EndpointId;
            key = arguments.Credentials;
            systemPrompt = arguments.SystemPrompt;

            if (!IsValidParameter(EnvVarNameDeploymentName, ref deploymentName))
            {
                Console.WriteLine($"The calling program failed to send the DeploymentName (first) command line parameter. You can also pass it by setting the '{EnvVarNameDeploymentName}' environment variable.");
                return false;
            }

            if (!IsValidParameter(EnvVarNameEndPoint, ref endpoint))
            {
                Console.WriteLine($"The calling program failed to send the Azure OpenAI Resource Endpoint (second) command line parameter. You can also pass it by setting the '{EnvVarNameEndPoint}' environment variable.");
                return false;
            }

            if (!IsValidParameter(EnvVarNameKey, ref key))
            {
                Console.WriteLine($"The calling program failed to send the Azure OpenAI Resource key (third) command line parameter. You can also pass it by setting the '{EnvVarNameKey}' environment variable.");
                return false;
            }

            if (String.IsNullOrEmpty(systemPrompt))
            {
                Console.WriteLine("The calling program failed to send the System Prompt (forth) command line parameter");
                return false;
            }

            return true;
        }

        private static bool IsValidParameter(string envVarName, ref string parameter)
        {
            return  !string.IsNullOrEmpty(parameter) ||
                    !string.IsNullOrEmpty((parameter = Environment.GetEnvironmentVariable(envVarName)));
        }

#if false
		private static string HarvestResult(string strInput, ChatChoice chatChoice)
        {
            var content = chatChoice.Message.Content;

            // if the content is empty...
            if (string.IsNullOrEmpty(content))
            {
                // most likely a content filter happened
                var contentFilterReason = String.Empty;
                if (chatChoice.ContentFilterResults.Violence.Filtered)
                    contentFilterReason = "Violence";
                if (chatChoice.ContentFilterResults.Hate.Filtered)
                    contentFilterReason = "Hate";
                if (chatChoice.ContentFilterResults.SelfHarm.Filtered)
                    contentFilterReason = "SelfHarm";
                if (chatChoice.ContentFilterResults.Sexual.Filtered)
                    contentFilterReason = "Sexual";

                if (!String.IsNullOrEmpty(contentFilterReason))
                    content = $"<Azure OpenAI didn't translate the sentence due to Content Filtering for {contentFilterReason}>";
                else
                    content = "<no translation returned>";
            }
            return CleanString(strInput, content);
        }
#endif

        private static string CleanString(string input, string output)
        {
            if (!String.IsNullOrEmpty(input) && input.Length > 1)
            {
                if (input.First() != '"')
                    output = output.TrimStart('"');
                if (input.Length > 2 && input.Last() != '"')
                    output = output.TrimEnd('"');
            }

            if (output.StartsWith(ResponsePrefix))
                output = output.Substring(ResponsePrefix.Length);

            return output;
        }
    }
}
