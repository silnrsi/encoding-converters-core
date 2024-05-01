using Paratext.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilEncConverters40.PtxConverters
{
    internal class PtxPluginHelpers
    {
        // normally, text tokens are publishable, but there are some that aren't (e.g. the text content of an \id marker).
        // And there's one case that seems like a bug to me, but which I've been told has worked that way forever and so
        // there's no changing it now... vis-a-vis:
        // the \va...\va* inline marker is defined differently depending on whether it comes immediately after a \v [num(s)] 
        // marker than if it comes elsewhere in a verse. The relevant difference is that when it comes immediately after a \v 
        // marker, it's value for IsPublishableVernacular (false) and IsMetadata (true) are opposite from the other case. 
        // So... if IsPublishableVernacular is false, at least check if this is that case, and return true, so we'll try to 
        // translate it as the others are (bkz we only send IsPub text segments for translation)
        public static bool IsPublishableVernacular(IUSFMTextToken t, List<IUSFMToken> tokens)
        {
            return t.IsPublishableVernacular ||
                   (PreviousToken(t, tokens, out IUSFMMarkerToken mt) && (mt.Marker == "va") && mt.IsMetadata);
        }

        public static bool PreviousToken(IUSFMTextToken t, List<IUSFMToken> tokens, out IUSFMMarkerToken previousToken)
        {
            var index = tokens.IndexOf(t) - 1;
            if ((index >= 0) && (index < tokens.Count) && (tokens[index] is IUSFMMarkerToken prevToken))
            {
                previousToken = prevToken;
                return true;
            }

            previousToken = null;
            return false;
        }

        public static bool IsMatchingVerse(IVerseRef verseReferenceFromToken, IVerseRef verseReference)
        {
            return ((verseReferenceFromToken?.ToString() == verseReference?.ToString()) ||
                    (verseReferenceFromToken.AllVerses?.Any(vr => vr.ToString() == verseReference?.ToString()) ?? false) ||
                    (verseReference?.AllVerses?.Any(vr => vr.ToString() == verseReferenceFromToken.ToString()) ?? false));
        }

        // when generating the 'alternate' source translation (i.e. ignoring, or rather, moving inline markers w/ text to the end),
        //  treat the \q1-4 markers special, so their text segments get combined even though they're in different paragraphs (leads to better translation).
        // NB: BUT there is one glitch: if a \q1 paragraph marker is immediately followed by a \v marker, then technically,
        //  the \q1 is in the preceding verse reference; not this one. So though we'd want to say that we're 'processingQs' in this case,
        //  we can't, so the text on that line will be translated separately from the \q2, etc., that follows it.
        // By making this a global member, it will remember going from verse-to-verse. But one place this would not work: 
        //  if the user processes a verse that ends with a \q1-4 marker, but rather than clicking 'Next', goes to some other, non-sequential verse, 
        //  we'd be mistaken that we're processingQs... (but since this is just a preview and not something substantive, let's just ignore this
        //  hopefully unusual case.
        public static string GetSourceAlternate(List<IUSFMToken> tokens, List<IUSFMTextToken> textTokens)
        {
			bool processingQs;
			string sourceStringAlternate = null;
            string textValuesAlternate = null;
            List<string> textValuesAlternateFootnotes = new();
            foreach (var token in tokens)
            {
                // if the token is a paragraph break token (\i.e. \p and \q{digit}), then put a new line in the running text
                if (IsParagraphToken(token))
                {
                    // but not for \q1-\q4, bkz we want those to be combined into a single run of text
                    processingQs = (token is IUSFMMarkerToken markerToken) && (markerToken.Marker.Contains("q"));
                    if (!processingQs)
                    {
                        // if we have already started collecting text, then add a newline to distinguish
                        //  this from any possible previous paragraphs
                        if (!String.IsNullOrEmpty(textValuesAlternate))
                            textValuesAlternate += Environment.NewLine;
                        continue;
                    }
                }

                // if it's not something we want to translate (e.g. not a text marker or a va or vp verse numbers (which are text markers))...
                if (!textTokens.Contains(token) ||
                    (AsTextToken(token, out IUSFMTextToken textToken) && !IsTranslatable(textToken, tokens)))
                    continue;   // skip it

                // if it's scripture text (i.e. the translatable stuff)...
                if (IsScriptureText(textToken))
                {
                    textValuesAlternate += textToken.Text;  // add it to the running accumulation
                }
                else
                {
                    // must be a footnote
                    textValuesAlternateFootnotes.Add(textToken.Text);
                }
            }

            // combine the text fragments of inline markers too, but add them after the main, regular text of the verse, 
            //  so they don't interfere with the translation of the main text
            sourceStringAlternate = textValuesAlternateFootnotes.Aggregate(textValuesAlternate?.Replace("  ", " "),
                                                                            (curr, next) => curr + Environment.NewLine + next);

            return sourceStringAlternate;

            static bool AsTextToken(IUSFMToken token, out IUSFMTextToken textToken)
            {
                if (token is IUSFMTextToken)
                {
                    textToken = token as IUSFMTextToken;
                    return true;
                }
                textToken = null;
                return false;
            }
        }

        private static readonly List<string> _additionalMarkersToTranslate = Properties.Settings.Default.AdditionalMarkersToTranslate.Cast<string>().ToList();

        // this would return true for both regular scripture text (i.e.  text after any of these markers:
        // \v, \q[1-3], \m, \pc, etc) and footnote text that is translatable (i.e. \ft)
        public static bool IsTranslatable(IUSFMTextToken token, List<IUSFMToken> tokens)
        {
            PtxPluginHelpers.PreviousToken(token, tokens, out IUSFMMarkerToken mt);
            return (IsScriptureText(token) && (mt.Marker != "va")) ||
                    _additionalMarkersToTranslate.Contains(mt.Marker);
        }

        public static bool IsScriptureText(IUSFMTextToken token)
        {
            return (token.IsPublishableVernacular && token.IsScripture);
        }


        public static bool IsParagraphToken(IUSFMToken token)
        {
            return (token is IUSFMMarkerToken markerToken) && (markerToken.Type == MarkerType.Paragraph); // not needed? if so, initialize list from a setting: && _paragraphMarkers.Contains(markerToken.Marker);
        }
    }
}
