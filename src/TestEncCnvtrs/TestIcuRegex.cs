// created by Steve McConnel, Feb 9, 2012.

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;

namespace TestEncCnvtrs
{
	[TestFixture]
	public class TestIcuRegex
	{
		[Test]
		public void TestRegexToVandC()
		{
			IcuRegexEncConverter rec = new IcuRegexEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICURegularExpression;
			rec.Initialize("Vowels->V", "[aeiou]->V",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = rec.Convert("abcdefghijklmnopqrstuvwxyz");
			Assert.AreEqual("VbcdVfghVjklmnVpqrstVvwxyz", output,
				"ICU regex converter should work properly!");
			// Note this is not case-sensitive, so the capital Vs should remain.
			rec.Initialize("Consonants->C", "[bcdfghjklmnpqrstvwxyz]->C",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output2 = rec.Convert(output);
			Assert.AreEqual("VCCCVCCCVCCCCCVCCCCCVCCCCC", output2,
				"reinitialized ICU regex converter should work properly!");
		}

		[Test]
		public void TestRegexForNonAnsiRange()
		{
			IcuRegexEncConverter rec = new IcuRegexEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICURegularExpression;
			rec.Initialize("RemoveDigits", "\\d->",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = rec.Convert("पोथी रे ते मियाः 1234567890123456789057 हेनाः");
			Assert.AreEqual("पोथी रे ते मियाः  हेनाः", output,
				"ICU regex converter should work properly for non-latin ranges of unicode also (for which UTF8String won't work on Windows)!");
		}

		[Test]
		[TestCase("\\u0930->r", "पोथी रथ", "पोथी rथ")]
		[TestCase("र->r", "पोथी रथ", "पोथी rथ")]
		// these don't work bkz the C++ side of it (or maybe a bug in ICU, I can't tell)
		//  doesn't convert, for example, \\u200d (or …) to UTF8 properly
		// [TestCase("[\\u200c\\u200d]->", "व्‍यक्‌ति", "व्यक्ति")]
		// [TestCase("[\\u2020\\u2021\\u200c\\u200d\\u230a\\u230b]->", "⌊व्‍यक्‌ति⌋ † ‡", "व्यक्ति  ")]
		// [TestCase("(a\uE03C|[⌊⌋\u0027\u002a\u2018\u2019\u201c\u201d\u0303\u0304\u0310\u0314\u0325\u200c\u200d\uE03C\uFFFD])->", "ham… ", "ham… ")]
		public void TestRegexForUnicodeEscapeCharacters(string converterSpec, string strInput, string strOutput)
		{
			var rec = new IcuRegexEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICURegularExpression;
			rec.Initialize("TransliterateDevanagariR", converterSpec,
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = rec.Convert(strInput);
			Assert.AreEqual(strOutput, output,
				$"ICU regex converter should work properly for Unicode Escape sequences (e.g. {converterSpec}) also!");
		}
	}
}

