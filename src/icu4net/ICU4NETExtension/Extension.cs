using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICU4NET;

namespace ICU4NETExtension
{
    public static class Extension
    {
        public static IEnumerable<string> Enumerate(this BreakIterator bi)
        {
            var sb = new StringBuilder();
            string text = bi.GetCLRText();
            int start = bi.First(), end = bi.Next();
            while (end != BreakIterator.DONE)
            {
                yield return text.Substring(start, end - start);
                start = end; end = bi.Next();
            }
        }

    }
}

