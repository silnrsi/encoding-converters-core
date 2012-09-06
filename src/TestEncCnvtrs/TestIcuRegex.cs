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
	}
}

