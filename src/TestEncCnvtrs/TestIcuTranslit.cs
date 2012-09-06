// created by Steve McConnel, Feb 8, 2012.

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;

namespace TestEncCnvtrs
{
	/// ------------------------------------------------------------------------
	/// <summary>
	/// Set of tests to exercise the methods in the IcuTranslitEncConverter class.
	/// These use the tranliterators that are part of the ICU code.
	/// </summary>
	/// ------------------------------------------------------------------------
	[TestFixture]
	public class TestIcuTranslit
	{
		/// --------------------------------------------------------------------
		/// <summary>
		/// Check that ICU transliterators exist, and verify that reading their
		/// names iteratively works.
		/// </summary>
		/// --------------------------------------------------------------------
		[Test]
		public void CheckForIcuTransliterators()
		{
			List<string> transliterators = IcuTranslitEncConverter.GetAvailableConverterSpecs();
			Assert.Less(100, transliterators.Count, "There should be at least one hundred ICU transliterators available!");
			for (int i = 0; i < transliterators.Count; ++i)
			{
				Assert.IsNotNull(transliterators[i], "The returned ICU converter name should not be null!");
			}
			Assert.Contains("Latin-Hebrew", transliterators, "ICU transliterators should include 'Latin-Hebrew'");
			Assert.Contains("Hebrew-Latin", transliterators, "ICU transliterators should include 'Hebrew-Latin'");
			Assert.Contains("Latin-Greek", transliterators, "ICU transliterators should include 'Latin-Greek'");
			Assert.Contains("Greek-Latin", transliterators, "ICU transliterators should include 'Greek-Latin'");
			string display1 = IcuTranslitEncConverter.GetDisplayName("Latin-Hebrew");
			Assert.IsNotNull(display1);
			string display2 = IcuTranslitEncConverter.GetDisplayName ("Greek-Latin");
			Assert.IsNotNull(display2);
		}
		
		string m_latinInput  = ":?ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		//"·;ἈΒΚΔΕΦΓἹΙΚΛΜΝΟΠΚΡΣΤΥΥΥΞΥΖἀβκδεφγἱικλμνοπκρστυυυξυζ";
		string m_greekOutput =
			"\u00b7\u003b\u1f08\u0392\u039a\u0394\u0395\u03a6" +
			"\u0393\u1f39\u0399\u039a\u039b\u039c\u039d\u039f" +
			"\u03a0\u039a\u03a1\u03a3\u03a4\u03a5\u03a5\u03a5" +
			"\u039e\u03a5\u0396\u03b1\u03b2\u03ba\u03b4\u03b5" +
			"\u03c6\u03b3\u1f31\u03b9\u03ba\u03bb\u03bc\u03bd" +
			"\u03bf\u03c0\u03ba\u03c1\u03c3\u03c4\u03c5\u03c5" +
			"\u03c5\u03be\u03c5\u03b6";

		[Test]
		public void TestLatin_GreekTranslit()
		{
			IcuTranslitEncConverter conv = new IcuTranslitEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICUTransliteration;
			conv.Initialize("Latin-Greek", "Latin-Greek",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = conv.Convert(m_latinInput);
			Assert.AreEqual(m_greekOutput, output, "Latin-Greek transliterator should work properly!");
		}
		
		string m_greekInput =
			"\u037A\u037E" +
			"\u0386\u0387\u0388\u0389\u038A\u038C\u038E\u038F" +
			"\u0390\u0391\u0392\u0393\u0394\u0395\u0396\u0397\u0398\u0399\u039A\u039B\u039C\u039D\u039E\u039F" +
			"\u03A0\u03A1\u03A3\u03A4\u03A5\u03A6\u03A7\u03A8\u03A9\u03AA\u03AB\u03AC\u03AD\u03AE\u03AF" +
			"\u03B0\u03B1\u03B2\u03B3\u03B4\u03B5\u03B6\u03B7\u03B8\u03B9\u03BA\u03BB\u03BC\u03BD\u03BE\u03BF" +
			"\u03C0\u03C1\u03C2\u03C3\u03C4\u03C5\u03C6\u03C7\u03C8\u03C9\u03CA\u03CB\u03CC\u03CD\u03CE";

		// "iͻͼͽ?΄΅Á:ÉḖÍ΋Ó΍ÝṒḯABGDEZĒTHIKLMN'XOPRSTYPHCHPSŌÏŸáéḗíÿ́abgdezēthiklmn'xoprs̱styphchpsōïÿóýṓ";
		string m_latinOutput =
			"\u0069?\u00c1:\u00c9\u1e16\u00cd\u00d3\u00dd\u1e52\u1e2f" +
			"ABGDEZ\u0112THIKLMN'XOPRSTYPHCHPS" +
			"\u014c\u00cf\u0178\u00e1\u00e9\u1e17\u00ed\u00ff" +
			"\u0301abgdez\u0113thiklmn'xoprs\u0331styphchps\u014d" +
			"\u00ef\u00ff\u00f3\u00fd\u1e53";
			
		[Test]
		public void TestGreek_LatinTranslit()
		{
			IcuTranslitEncConverter conv = new IcuTranslitEncConverter();
			string lhsEncoding = "UNICODE";
			string rhsEncoding = "UNICODE";
			ConvType convType = ConvType.Unicode_to_from_Unicode;
			int procFlags = (int)ProcessTypeFlags.ICUTransliteration;
			conv.Initialize("Greek-Latin", "Greek-Latin",
				ref lhsEncoding, ref rhsEncoding, ref convType, ref procFlags, 0, 0, false);
			string output = conv.Convert(m_greekInput);
//			var xxx = output.ToCharArray();
//			if (xxx != null)
//			{
//				Console.WriteLine("Latin output characters");
//				for (int i = 0; i < xxx.Length; ++i)
//				{
//					char ch = xxx[i];
//					Console.WriteLine("Latin[{0}] = '{1}' = {2:d5} = {2:x4}", i, ch, (int)ch);
//				}
//			}
			Assert.AreEqual(m_latinOutput, output, "Latin-Greek transliterator should work properly!");
		}
	}
}
