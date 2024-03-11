// Copyright 2022 DeepL SE (https://www.deepl.com)
// Use of this source code is governed by an MIT
// license that can be found in the LICENSE file.

using System.Text.Json.Serialization;

namespace DeepL.Model
{
	/// <summary>The result of a text translation.</summary>
	public sealed class NllbTextResult
	{
		/// <summary>Initializes a new instance of <see cref="NllbTextResult" />.</summary>
		/// <param name="text">Translated text.</param>
		/// <param name="detectedSourceLanguageCode">The detected language code of the input text.</param>
		/// <remarks>
		///   The constructor for this class (and all other Model classes) should not be used by library users. Ideally it
		///   would be marked <see langword="internal" />, but needs to be <see langword="public" /> for JSON deserialization.
		///   In future this function may have backwards-incompatible changes.
		/// </remarks>
		[JsonConstructor]
		public NllbTextResult(string originalText, string translatedText)
		{
			OriginalText = originalText;
			TranslatedText = translatedText;
		}

		/// <summary>The translated text.</summary>
		[JsonPropertyName("translatedText")]
		public string TranslatedText { get; }

		/// <summary>The language code of the source text detected by DeepL.</summary>
		[JsonPropertyName("originalText")]
		public string OriginalText { get; }

		/// <summary>Returns the translated text.</summary>
		/// <returns>The translated text.</returns>
		public override string ToString() => TranslatedText;
	}
}
