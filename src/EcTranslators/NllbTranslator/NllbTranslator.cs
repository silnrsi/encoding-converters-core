// Copyright 2022 DeepL SE (https://www.deepl.com)
// Use of this source code is governed by an MIT
// license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DeepL.Internal;
using DeepL.Model;

namespace Nllb
{

    public interface ITranslator : IDisposable
    {
        /// <summary>Translate specified text from source language into target language.</summary>
        /// <param name="text">Text to translate; must not be empty.</param>
        /// <param name="sourceLanguageCode">Language code of the input language, or null to use auto-detection.</param>
        /// <param name="targetLanguageCode">Language code of the desired output language.</param>
        /// <param name="options"><see cref="TextTranslateOptions" /> influencing translation.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Text translated into specified target language.</returns>
        /// <exception cref="ArgumentException">If any argument is invalid.</exception>
        /// <exception cref="DeepLException">
        ///   If any error occurs while communicating with the DeepL API, a
        ///   <see cref="DeepLException" /> or a derived class will be thrown.
        /// </exception>
        Task<string> TranslateTextAsync(
              string text,
              string? sourceLanguageCode,
              string targetLanguageCode,
              DeepL.TextTranslateOptions? options = null,
              CancellationToken cancellationToken = default);

        /// <summary>
        ///   Client for the DeepL API. To use the DeepL API, initialize an instance of this class using your DeepL
        ///   Authentication Key. All functions are thread-safe, aside from <see cref="Translator.Dispose" />.
        /// </summary>
        public sealed class Translator : ITranslator
        {
            /// <summary>Base URL for the Nllb accounts.</summary>
            private const string NllbLServerUrl = "http://localhost:8000";

            /// <summary>Internal class implementing HTTP requests.</summary>
            private readonly DeepLClient _client;

            /// <summary>Initializes a new <see cref="Translator" /> object using your authentication key.</summary>
            /// <param name="authKey">
            ///   Authentication Key as found in your
            ///   <a href="https://www.deepl.com/pro-account/">DeepL API account</a>.
            /// </param>
            /// <param name="options">Additional options controlling Translator behaviour.</param>
            /// <exception cref="ArgumentNullException">If authKey argument is null.</exception>
            /// <exception cref="ArgumentException">If authKey argument is empty.</exception>
            /// <remarks>
            ///   This function does not establish a connection to the DeepL API. To check connectivity, use
            ///   <see cref="GetUsageAsync" />.
            /// </remarks>
            public Translator(string authKey, DeepL.TranslatorOptions? options = null)
            {
                options ??= new DeepL.TranslatorOptions();

                if (authKey == null)
                {
                    throw new ArgumentNullException(nameof(authKey));
                }

                authKey = authKey.Trim();

                if (authKey.Length == 0)
                {
                    throw new ArgumentException($"{nameof(authKey)} is empty");
                }

                var serverUrl = new Uri(options.ServerUrl ?? NllbLServerUrl);

                var headers = new Dictionary<string, string?>(options.Headers, StringComparer.OrdinalIgnoreCase);

                if (!headers.ContainsKey("Authorization"))
                {
                    headers.Add("Authorization", authKey);
                }

                var clientFactory = options.ClientFactory ?? (() =>
                      DeepLClient.CreateDefaultHttpClient(
                            options.PerRetryConnectionTimeout,
                            options.OverallConnectionTimeout,
                            options.MaximumNetworkRetries));

                _client = new DeepLClient(
                      serverUrl,
                      clientFactory,
                      headers);
            }

            /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="Translator" />.</summary>
            public void Dispose() => _client.Dispose();

            /// <summary>Retrieves the version string, with format MAJOR.MINOR.BUGFIX.</summary>
            /// <returns>String containing the library version.</returns>
            public static string Version()
            {
                var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                      ?.InformationalVersion ?? "1.3.0";
                return version;
            }

            /// <summary>
            ///   Determines if the given DeepL Authentication Key belongs to an API Free account or an API Pro account.
            /// </summary>
            /// <param name="authKey">
            ///   DeepL Authentication Key as found in your
            ///   <a href="https://www.deepl.com/pro-account/">DeepL API account</a>.
            /// </param>
            /// <returns>
            ///   <c>true</c> if the Authentication Key belongs to an API Free account, <c>false</c> if it belongs to an API Pro
            ///   account.
            /// </returns>
            public static bool AuthKeyIsFreeAccount(string authKey) => true;

            /// <inheritdoc />
            public async Task<Usage> GetUsageAsync(CancellationToken cancellationToken = default)
            {
                using var responseMessage = await _client.ApiGetAsync("/api/v1/usage", cancellationToken).ConfigureAwait(false);
                await DeepLClient.CheckStatusCodeAsync(responseMessage).ConfigureAwait(false);
                var usageFields = await JsonUtils.DeserializeAsync<Usage.JsonFieldsStruct>(responseMessage)
                      .ConfigureAwait(false);
                return new Usage(usageFields);
            }

            public class TranslateMsg
            {
                [Newtonsoft.Json.JsonProperty("sourceLanguage")]
                public string SourceLanguage { get; set; }
                [Newtonsoft.Json.JsonProperty("targetLanguage")]
                public string TargetLanguage { get; set; }
                [Newtonsoft.Json.JsonProperty("text")]
                public string Text { get; set; }
            }

            /// <inheritdoc />
            public async Task<string> TranslateTextAsync(
                  string text,
                  string? sourceLanguageCode,
                  string targetLanguageCode,
                  DeepL.TextTranslateOptions? options = null,
                  CancellationToken cancellationToken = default)
            {
                // call it once for each paragraph of text
                var strings = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var result = String.Empty;
                foreach (var str in strings)
                {
                    var bodyParams = new TranslateMsg { SourceLanguage = sourceLanguageCode, TargetLanguage = targetLanguageCode, Text = str };

                    using var responseMessage = await _client
                          .ApiPostJsonAsync("/api/v1/translate/", cancellationToken, bodyParams).ConfigureAwait(false);

                    // Read response as a string.
                    if (!String.IsNullOrEmpty(result))
                        result += ",";
                    result += await responseMessage.Content.ReadAsStringAsync();
                    if (!responseMessage.IsSuccessStatusCode)
                        throw new ApplicationException(result);
                }
                return $"[{result}]";
            }

            /// <summary>Internal function to retrieve available languages.</summary>
            /// <param name="target"><c>true</c> to retrieve target languages, <c>false</c> to retrieve source languages.</param>
            /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
            /// <returns>Array of <see cref="Language" /> objects containing information about the available languages.</returns>
            /// <exception cref="DeepLException">
            ///   If any error occurs while communicating with the DeepL API, a
            ///   <see cref="DeepLException" /> or a derived class will be thrown.
            /// </exception>
            private async Task<TValue[]> GetLanguagesAsync<TValue>(
                  CancellationToken cancellationToken = default)
            {
                using var responseMessage =
                      await _client.ApiGetAsync("/api/v1/translate/languages/", cancellationToken)
                            .ConfigureAwait(false);

                await DeepLClient.CheckStatusCodeAsync(responseMessage).ConfigureAwait(false);
                return await JsonUtils.DeserializeAsync<TValue[]>(responseMessage).ConfigureAwait(false);
            }

            /// <inheritdoc />
            public async Task<string[]> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default) =>
                  await GetLanguagesAsync<string> (cancellationToken).ConfigureAwait(false);

            /// <summary>
            ///   Checks the specified languages and options are valid, and returns an enumerable of tuples containing the parameters
            ///   to include in HTTP request.
            /// </summary>
            /// <param name="sourceLanguageCode">
            ///   Language code of translation source language, or null if auto-detection should be
            ///   used.
            /// </param>
            /// <param name="targetLanguageCode">Language code of translation target language.</param>
            /// <param name="options">Extra <see cref="TextTranslateOptions" /> influencing translation.</param>
            /// <returns>Enumerable of tuples containing the parameters to include in HTTP request.</returns>
            /// <exception cref="ArgumentException">If the specified languages or options are invalid.</exception>
            private IEnumerable<(string Key, string Value)> CreateHttpParams(
                  string? sourceLanguageCode,
                  string targetLanguageCode,
                  string text,
                  DeepL.TextTranslateOptions? options)
            {
                targetLanguageCode = DeepL.LanguageCode.Standardize(targetLanguageCode);
                sourceLanguageCode = sourceLanguageCode == null ? null : DeepL.LanguageCode.Standardize(sourceLanguageCode);

                CheckValidLanguages(sourceLanguageCode, targetLanguageCode);

                var bodyParams = new List<(string Key, string Value)> { ("sourceLanguage", sourceLanguageCode), ("targetLanguage", targetLanguageCode), ("text", text) };

                return bodyParams;
            }

            /// <summary>
            ///   Checks the specified languages and options are valid, and returns an enumerable of tuples containing the parameters
            ///   to include in HTTP request.
            /// </summary>
            /// <param name="sourceLanguageCode">
            ///   Language code of translation source language, or null if auto-detection should be
            ///   used.
            /// </param>
            /// <param name="targetLanguageCode">Language code of translation target language.</param>
            /// <param name="options">Extra <see cref="DocumentTranslateOptions" /> influencing translation.</param>
            /// <returns>Enumerable of tuples containing the parameters to include in HTTP request.</returns>
            /// <exception cref="ArgumentException">If the specified languages or options are invalid.</exception>
            private IEnumerable<(string Key, string Value)> CreateHttpParams(
                  string? sourceLanguageCode,
                  string targetLanguageCode,
                  DeepL.DocumentTranslateOptions? options)
            {
                targetLanguageCode = DeepL.LanguageCode.Standardize(targetLanguageCode);
                sourceLanguageCode = sourceLanguageCode == null ? null : DeepL.LanguageCode.Standardize(sourceLanguageCode);

                CheckValidLanguages(sourceLanguageCode, targetLanguageCode);

                var bodyParams = new List<(string Key, string Value)> { ("target_lang", targetLanguageCode) };
                if (sourceLanguageCode != null)
                {
                    bodyParams.Add(("source_lang", sourceLanguageCode));
                }

                return bodyParams;
            }

            /// <summary>Checks the specified source and target language are valid, and throws an exception if not.</summary>
            /// <param name="sourceLanguageCode">Language code of translation source language, or null if auto-detection is used.</param>
            /// <param name="targetLanguageCode">Language code of translation target language.</param>
            /// <exception cref="ArgumentException">If source or target language code are not valid.</exception>
            private static void CheckValidLanguages(string? sourceLanguageCode, string targetLanguageCode)
            {
                if (sourceLanguageCode is { Length: 0 })
                {
                    throw new ArgumentException($"{nameof(sourceLanguageCode)} must not be empty");
                }

                if (targetLanguageCode.Length == 0)
                {
                    throw new ArgumentException($"{nameof(targetLanguageCode)} must not be empty");
                }

                switch (targetLanguageCode)
                {
                    case "en":
                        throw new ArgumentException(
                              $"{nameof(targetLanguageCode)}=\"en\" is deprecated, please use \"en-GB\" or \"en-US\" instead");
                    case "pt":
                        throw new ArgumentException(
                              $"{nameof(targetLanguageCode)}=\"pt\" is deprecated, please use \"pt-PT\" or \"pt-BR\" instead");
                }
            }

            /// <summary>
            ///   Determines recommended time to wait before checking document translation again, using an optional hint of
            ///   seconds remaining.
            /// </summary>
            /// <param name="hintSecondsRemaining">Optional hint of the number of seconds remaining.</param>
            /// <returns><see cref="TimeSpan" /> to wait.</returns>
            private static TimeSpan CalculateDocumentWaitTime(int? hintSecondsRemaining)
            {
                var secs = ((hintSecondsRemaining ?? 0) / 2.0) + 1.0;
                secs = Math.Max(1.0, Math.Min(secs, 60.0));
                return TimeSpan.FromSeconds(secs);
            }

            /// <summary>Class used for JSON-deserialization of text translate results.</summary>
            private readonly struct TextTranslateResult
            {
                /// <summary>Initializes a new instance of <see cref="TextTranslateResult" />, used for JSON deserialization.</summary>
                [JsonConstructor]
                public TextTranslateResult(NllbTextResult translation)
                {
                    Translation = translation;
                }

                /// <summary>Array of <see cref="NllbTextResult" /> objects holding text translation results.</summary>
                public NllbTextResult Translation { get; }
            }
        }
    }
}