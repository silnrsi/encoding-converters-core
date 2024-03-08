// #define UseLocalExamples

#if DEBUG
#define LogResults
// #define DEBUG_LIVE
#endif

using Google.Apis.Auth.OAuth2;
using Google.Cloud.AIPlatform.V1;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Type;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;
using Value = Google.Protobuf.WellKnownTypes.Value;

namespace SilEncConverters40.EcTranslators.VertexAi.VertexAiExe
{
    internal class Program
    {
#if LogResults
        private const string LogFilePath = @"C:\btmp\VertexAiExeLog.txt";
#endif
        private const string EnvVarNameProjectId = "EncConverters_VertexAiProjectId";       // e.g. bright-coyote-381812
        private const string EnvVarNameLocationId = "EncConverters_VertexAiLocationId";     // e.g. us-central1
        private const string EnvVarNamePublisher = "EncConverters_VertexAiPublisher";       // e.g. google
        private const string EnvVarNameModelId = "EncConverters_VertexAiModelId";           // e.g. chat-bison-32k, chat-bison, gemini-pro, etc.

        private const string EnvVarNameGoogleApplicationCredentials = "GOOGLE_APPLICATION_CREDENTIALS"; // e.g. @"C:\Users\pete_\Downloads\bright-coyote-381812-bc584cec007f.json"

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

			try
			{
                var contents = File.ReadAllText(pathToEncryptedCommandLineParameterFile);
                var json = EncryptionClass.Decrypt(contents);
                var arguments = JsonConvert.DeserializeObject<VertexAiPromptExeTranslatorCommandLineArgs>(json);

                if (!IsValidParameters(arguments, out PredictionServiceClient client, out EndpointName endpointName,
									   out string systemPrompt))
                    return;

                await ProcessRequest(arguments, client, endpointName, systemPrompt);
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

        private static async Task<bool> ProcessRequest(VertexAiPromptExeTranslatorCommandLineArgs arguments, PredictionServiceClient client,
                                                       EndpointName endpointName, string systemPrompt)
        {
            var chatConversation = new ChatConversation
            {
                context = $"You are a Language Translator that Translates between different languages. Your job is to translate the text in the user prompt into the requested language according to the following instructions: {systemPrompt}",
                examples = new List<Example>(),
                messages = new List<ChatMessage>()
            };

            var numberOfExamples = arguments.ExamplesInputString.Count;
            for (int i = 0; i < numberOfExamples; i++)
            {
                chatConversation.examples.Add(new Example
                {
                    input = new Input { content = arguments.ExamplesInputString[i] },
                    output = new Output { content = arguments.ExamplesOutputString[i] }
                });
            }

            // You can construct Protobuf from JSON.
            var parametersJson = JsonConvert.SerializeObject(new
            {
                temperature = (arguments.Temperature == null) ? 0.3 : (double)arguments.Temperature,
                maxDecodeSteps = (arguments.MaxDecodeSteps == null) ? 200 : (int)arguments.MaxDecodeSteps,
                topP = (arguments.TopP == null) ? 0.8 : arguments.TopP,
                topK = (arguments.TopK == null) ? 40 : arguments.TopK,
            });
            var parameters = Value.Parser.ParseJson(parametersJson);

            // in case there are multiple lines (e.g. what Paratext will do if the verse has multiple paragraphs),
            //  process in a while loop
            var index = 0;
#if UseLocalExamples
            List<string> input = new List<string> { "उसने सब लोग जान से मारा।", "वहाँ वह विश्राम के दिन प्रार्थना घर में जाकर लोगों को परमेश्वर का वचन सुनाने लगा। सब लोग सुनकर चकित हो गये।", "", "परंतु कई तो यह भी कहने लगे, “यह ज्ञान इसको कहाँ से आया!? और ऐसे सामर्थ्‍य के काम यह कैसे करता है, जिसकी चर्चा सब लोग कर रहे हैं!?" };
#else
            List<string> input = null; // new List<string> { "वहाँ वह विश्राम के दिन प्रार्थना घर में जाकर लोगों को परमेश्वर का वचन सुनाने लगा। सब लोग सुनकर चकित हो गये।", "", "परंतु कई तो यह भी कहने लगे, “यह ज्ञान इसको कहाँ से आया!? और ऐसे सामर्थ्‍य के काम यह कैसे करता है, जिसकी चर्चा सब लोग कर रहे हैं!?" };
#endif
            while ((input != null && input.Count > index) || (input == null && Console.In.Peek() != -1))
            {
                var strInput = (input != null)
                                ? input[index++]
                                : Console.ReadLine();
                if (String.IsNullOrEmpty(strInput?.Trim(TrimmableChars)))   // don't actually trim them, but just for the sake of finding nothing to translate...
                    continue;

                // add the string to be translated to as the 'user' message
                chatConversation.messages.Add(new ChatMessage { author = "user", content = strInput });

                // call the service to process the user msg based on the given system prompt
                // Make the request.
                var json = chatConversation.ToString();
#if LogResults
                File.AppendAllText(LogFilePath, json + Environment.NewLine);
#endif
                var response = client.Predict(endpointName, new List<Value> { Value.Parser.ParseJson(json) }, parameters);

                var fields = response.Predictions.First().StructValue.Fields;
                // clean up and return the "assistent's response"
                var strOutput = HarvestResult(strInput, fields);

                // put that back in the chat, in case we are processing multiple lines
                //    (this'll make them 'related' for better translation)
                chatConversation.messages.Add(new ChatMessage { author = "1", content = strOutput });
            }

            for (var i = 0; i < chatConversation.messages.Count;)
            {
                var strInput = chatConversation.messages[i++].content;
                var strOutput = chatConversation.messages[i++].content;

#if LogResults
                File.AppendAllText(LogFilePath, string.Format("{1}=>{2}:{3}=>{4}{0}", Environment.NewLine, systemPrompt, i / 2, strInput, strOutput));
#endif
                // write the responses to the standard out to return it
                Console.WriteLine(strOutput);
            }

            return true;
        }

        private static string HarvestResult(string strInput, MapField<string, Value> fields)
        {
            var responseContent = fields["candidates"].ListValue.Values[0].StructValue.Fields["content"].StringValue;

            if (strInput?[0] != ' ' && responseContent?[0] == ' ')
                responseContent = responseContent.Substring(1);

            // these come from: https://cloud.google.com/vertex-ai/docs/generative-ai/configure-safety-attributes-palm
            if (responseContent.Contains("I'm not able to help with that, as I'm only a language model. If you believe this is an error, please send us your feedback."))
            {
                var blocked = fields["safetyAttributes"].ListValue.Values[0].StructValue.Fields["blocked"].BoolValue;
                if (blocked)
                {
                    var errors = fields["safetyAttributes"].ListValue.Values[0].StructValue.Fields["errors"].ListValue.Values[0].NumberValue.ToString();
                    if ((errors.Length == 3) && (errors[0] == '2'))
                    {
                        string contentFilteringReason = "violence, sexual content, etc.";
                        switch (errors.Substring(1))
                        {
                            case "20":
                                contentFilteringReason = "The supplied or returned language is unsupported.For a list of supported languages, see Language support.";
                                break;

                            case "30":
                                contentFilteringReason = "The prompt or response was blocked because it was found to be potentially harmful. A term is included from the terminology blocklist. Rephrase your prompt.";
                                break;

                            case "31":
                                contentFilteringReason = "The content might include Sensitive Personally Identifiable Information(SPII). Rephrase your prompt.";
                                break;

                            case "40":
                                contentFilteringReason = "The prompt or response was blocked because it was found to be potentially harmful. The content violates SafeSearch settings.Rephrase your prompt.";
                                break;

                            case "50":
                                contentFilteringReason = "The prompt or response was blocked because it might contain sexually explicit content.Rephrase your prompt.";
                                break;

                            case "51":
                                contentFilteringReason = "The prompt or response was blocked because it might contain hate speech content.Rephrase your prompt.";
                                break;

                            case "52":
                                contentFilteringReason = "The prompt or response was blocked because it might contain harassment content. Rephrase your prompt.";
                                break;

                            case "53":
                                contentFilteringReason = "The prompt or response was blocked because it might contain dangerous content. Rephrase your prompt.";
                                break;

                            case "54":
                                contentFilteringReason = "The prompt or response was blocked because it might contain toxic content. Rephrase your prompt.";
                                break;

                            default:
                            case "00":
                                contentFilteringReason = "Reason unknown. Rephrase your prompt.";
                                break;
                        }

                        responseContent = $"<VertexAI didn't translate the sentence due to Content Filtering: {contentFilteringReason}>";
                    }
                }
            }

            return CleanString(strInput, responseContent);
        }

        private static string CleanString(string input, string output)
        {
            if (!String.IsNullOrEmpty(input) && input.Length > 1)
            {
                if (input.First() != '"')
                    output = output.TrimStart('"');
                if (input.Length > 2 && input.Last() != '"')
                    output = output.TrimEnd('"');
            }

            return output;
        }

        private static bool IsValidParameters(VertexAiPromptExeTranslatorCommandLineArgs arguments, out PredictionServiceClient client,
                                              out EndpointName endpoint, out string systemPrompt)
        {
            client = null;
            endpoint = null;

            var projectId = arguments.ProjectId;
            var locationId = arguments.LocationId;
            var publisher = arguments.Publisher;
            var modelId = arguments.ModelId;

            systemPrompt = arguments.SystemPrompt;

            if (String.IsNullOrEmpty(projectId) || String.IsNullOrEmpty(locationId) || String.IsNullOrEmpty(publisher) || String.IsNullOrEmpty(modelId) || String.IsNullOrEmpty(systemPrompt))
            {
                Console.WriteLine(String.Format("Usage:{0}{0}{1} \"<PathToEncryptedCommandLineParameterFile>\"", Environment.NewLine, typeof(Program).Namespace));
                return false;
            }

            if (!IsValidParameter(EnvVarNameProjectId, ref projectId))
            {
                Console.WriteLine($"The calling program failed to send the GoogleCloud ProjectId (first) command line parameter. You can also pass it by setting the '{EnvVarNameProjectId}' environment variable.");
                return false;
            }

            if (!IsValidParameter(EnvVarNameLocationId, ref locationId))
            {
                Console.WriteLine($"The calling program failed to send the GoogleCloud LocationId (second) command line parameter (where the VertexAI resource is located, e.g. 'us-central1'). You can also pass it by setting the '{EnvVarNameLocationId}' environment variable.");
                return false;
            }

            if (!IsValidParameter(EnvVarNamePublisher, ref publisher))
            {
                Console.WriteLine($"The calling program failed to send the Vertex AI Publisher (third) command line parameter (e.g. 'google'). You can also pass it by setting the '{EnvVarNamePublisher}' environment variable.");
                return false;
            }

            if (!IsValidParameter(EnvVarNameModelId, ref modelId))
            {
                Console.WriteLine($"The calling program failed to send the Vertext AI ModelId (third) command line parameter (e.g. chat-bison-32k, chat-bison, gemini-pro, etc). You can also pass it by setting the '{EnvVarNameModelId}' environment variable.");
                return false;
            }

            if (String.IsNullOrEmpty(systemPrompt))
            {
                Console.WriteLine("The calling program failed to send the System Prompt (forth) command line parameter");
                return false;
            }

            var credentialsAsJson = arguments.Credentials;
            var credentials = !String.IsNullOrEmpty(credentialsAsJson) && !File.Exists(credentialsAsJson)
                                ? GoogleCredential.FromJson(credentialsAsJson)
                                : GoogleCredential.GetApplicationDefault();

            client = new PredictionServiceClientBuilder
            {
                Endpoint = $"{locationId}-aiplatform.googleapis.com",
                GoogleCredential = credentials,
            }.Build();

            // Configure the parent resource.
            endpoint = EndpointName.FromProjectLocationPublisherModel(projectId, locationId, publisher, modelId);

            return true;
        }

        private static bool IsValidParameter(string envVarName, ref string parameter)
        {
            return !string.IsNullOrEmpty(parameter) ||
                    !string.IsNullOrEmpty((parameter = Environment.GetEnvironmentVariable(envVarName)));
        }

        static string Base64ToString(string base64String)
        {
            byte[] base64Bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(base64Bytes);
        }
    }
}
