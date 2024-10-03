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
	public class TestNetRegex
	{
		[Test]
		public void TestRegexToVandC()
		{
			NetRegexEncConverter rec = new NetRegexEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICURegularExpression;
			rec.Initialize("Vowels->V", "{[aeiou]}->{V};1",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = rec.Convert("abcdEfGhijklMnopqrstUwxyz");
			Assert.AreEqual("VbcdVfGhVjklMnVpqrstVwxyz", output,
				"Net regex converter should work properly!");
			// Note this is not case-sensitive, so the capital Vs should remain.
			rec.Initialize("Consonants->C", "{[bcdfghjklmnpqrstwxyz]}->{C};1",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output2 = rec.Convert(output);
			Assert.AreEqual("VCCCVCCCVCCCCCVCCCCCVCCCC", output2,
				"reinitialized Net regex converter should work properly!");
		}

		[Test]
		public void TestRegexForNonAnsiRange()
		{
			NetRegexEncConverter rec = new NetRegexEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICURegularExpression;
			rec.Initialize("RemoveDigits", "{\\d}->{}",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = rec.Convert("पोथी रे ते मियाः 1234567890123456789057 हेनाः");
			Assert.AreEqual("पोथी रे ते मियाः  हेनाः", output,
				"Net regex converter should work properly for non-latin ranges of unicode also (for which UTF8String won't work on Windows)!");
		}

		[Test]
		[TestCase("{[a-e]}->{\u2020};1", "an Apple", "†n †ppl†")]
		[TestCase("{\\u0930}->{r}", "पोथी रथ", "पोथी rथ")]
		[TestCase("{र}->{r}", "पोथी रथ", "पोथी rथ")]
		[TestCase("{[\\u200c\\u200d]}->{}", "व्‍यक्‌ति", "व्यक्ति")]
		[TestCase("{[\\u2020\\u2021\\u200c\\u200d\\u230a\\u230b]}->{}", "⌊व्‍यक्‌ति⌋ † ‡", "व्यक्ति  ")]
		[TestCase("{(a\uE03C|[⌊⌋\u0027\u002a\u2018\u2019\u201c\u201d\u0303\u0304\u0310\u0314\u0325\u200c\u200d\uE03C\uFFFD])}->{}", "ham… ", "ham… ")]
		[TestCase("{\\r\\n|\\n|\\r}->{ };0", @"multiple
lines", "multiple lines")]
		[TestCase("{([.!:;।…]+)(\\s*)}->{\\r\\n};0␞{([?])(\\s*)}->{$1\\r\\n};0␞{\\r\\n([?])}->{$1};0␞{(^ |[“‘]|[”’](\\s*))}->{};2", "क्‍या आने वाले मसीहा आप ही हो? या हम किसी दूसरे की प्रतीक्षा करें?", @"क्‍या आने वाले मसीहा आप ही हो?
या हम किसी दूसरे की प्रतीक्षा करें?
")]
		[TestCase("{([.!:;।…]+)(\\s*)}->{\\r\\n};0␞{([?])(\\s*)}->{$1\\r\\n};0␞{\\r\\n([?])}->{$1};0␞{(^ |[“‘]|[”’](\\s*))}->{};2", "क्‍या आने वाले मसीहा आप ही हो!? या हम किसी दूसरे की प्रतीक्षा करें?", @"क्‍या आने वाले मसीहा आप ही हो?
या हम किसी दूसरे की प्रतीक्षा करें?
")]
		// In repository:
		// {([.!:;।…]+)(\s*)}-&gt;{\r\n};0␞{([?])(\s*)}-&gt;{$1\r\n};0␞{\r\n([?])}-&gt;{$1};0␞{(^ |[“‘]|[”’](\s*))}-&gt;{};2
		[TestCase("{([.!:;।…]+)(\\s*)}->{\\r\\n};0␞{([?])(\\s*)}->{$1\\r\\n};0␞{\\r\\n([?])}->{$1};0␞{(^ |[“‘]|[”’](\\s*))}->{};2", "क्‍या आने वाले मसीहा आप ही हो!?’” या हम किसी दूसरे की प्रतीक्षा करें?", @"क्‍या आने वाले मसीहा आप ही हो?
या हम किसी दूसरे की प्रतीक्षा करें?
")]
		[TestCase("{a}->{b};0␞{b}->{A};0", "a", "A")]
		[TestCase("{a}->{b};0␞{b}->{A};65535", "a", "b")]   // 65535 means the 2nd regex is disabled
		[TestCase("{a}->{b};0␞{b}->{A};0x8000", "a", "b")]	// 0x8000 can also be used to mean the 2nd regex is disabled
		public void TestRegexForUnicodeEscapeCharacters(string converterSpec, string strInput, string strOutput)
		{
			var rec = new NetRegexEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICURegularExpression;
			rec.Initialize("TransliterateDevanagariR", converterSpec,
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = rec.Convert(strInput);
			Assert.AreEqual(strOutput, output,
				$"Net regex converter should work properly for Unicode Escape sequences (e.g. {converterSpec}) also!");
		}
	}
}

