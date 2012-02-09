// created by Steve McConnel, Feb 3, 2012.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;

namespace TestEncCnvtrs
{
	/// ------------------------------------------------------------------------
	/// <summary>
	/// Set of tests to exercise the methods in the IcuConvEncConverter class.
	/// These use the converters that are part of the ICU code.
	/// </summary>
	/// ------------------------------------------------------------------------
	[TestFixture]
	public class TestIcuConvEncConverter
	{
		// Access the C++ methods that aren't exposed otherwise.
		#region DLLImport Statements
		[DllImport("IcuConvEC", EntryPoint="IcuConvEC_ConverterNameList_start")]
		static extern unsafe int CppConverterNameList_start();

		[DllImport("IcuConvEC", EntryPoint="IcuConvEC_ConverterNameList_next")]
		static extern unsafe string CppConverterNameList_next();

		[DllImport("IcuConvEC", EntryPoint="IcuConvEC_GetDisplayName")]
		static extern unsafe string CppGetDisplayName(string strID);
		#endregion DLLImport Statements

		/// --------------------------------------------------------------------
		/// <summary>
		/// Check that ICU converters exist, and verify that reading their names
		/// iteratively works.
		/// </summary>
		/// --------------------------------------------------------------------
		[Test]
		public void CheckForIcuConverters()
		{
			int count = CppConverterNameList_start();
			Assert.Less(100, count, "There should be at least one hundred ICU converters available!");
			List<string> converters = new List<string>();
			for (int i = 0; i < count; ++i)
			{
				var name = CppConverterNameList_next();
				Assert.IsNotNull(name, "The returned ICU converter name should not be null!");
				converters.Add(name);
			}
			var noname = CppConverterNameList_next();
			Assert.IsNull(noname, "Excess converter names should be null.");
			// Check for the specific converters used in the following tests.
			Assert.Contains("ISO-8859-1", converters, "The set of ICU converters should contain 'ISO-8859-1'.");
			Assert.Contains("iso-8859_10-1998", converters, "The set of ICU converters should contain 'iso-8859_10-1998'.");
			Assert.Contains("iso-8859_11-2001", converters, "The set of ICU converters should contain 'iso-8859_11-2001'.");
			Assert.Contains("iso-8859_14-1998", converters, "The set of ICU converters should contain 'iso-8859_14-1998'.");
			string display1 = CppGetDisplayName("ISO-8859-1");
			Assert.IsNotNull(display1);
			string display10 = CppGetDisplayName ("iso-8859_10-1998");
			Assert.IsNotNull(display10);
		}
		
		byte[] m_bytesIso8859 = new byte[191] {
			 32,  33,  34,  35,  36,  37,  38,  39,  40,  41,  42,  43,  44,  45,  46,  47,
			 48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,  63,
			 64,  65,  66,  67,  68,  69,  70,  71,  72,  73,  74,  75,  76,  77,  78,  79,
			 80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  91,  92,  93,  94,  95,
			 96,  97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
			112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,
			192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
			208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
			224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
			240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
			};

		string m_convertedIso8859_1 = " !\"#$%&'()*+,-./0123456789:;<=>?" +
				"@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
				"\xA0\xA1\xA2\xA3\xA4\xA5\xA6\xA7\xA8\xA9\xAA\xAB\xAC\xAD\xAE\xAF" +
				"\xB0\xB1\xB2\xB3\xB4\xB5\xB6\xB7\xB8\xB9\xBA\xBB\xBC\xBD\xBE\xBF" +
				"\xC0\xC1\xC2\xC3\xC4\xC5\xC6\xC7\xC8\xC9\xCA\xCB\xCC\xCD\xCE\xCF" +
				"\xD0\xD1\xD2\xD3\xD4\xD5\xD6\xD7\xD8\xD9\xDA\xDB\xDC\xDD\xDE\xDF" +
				"\xE0\xE1\xE2\xE3\xE4\xE5\xE6\xE7\xE8\xE9\xEA\xEB\xEC\xED\xEE\xEF" +
				"\xF0\xF1\xF2\xF3\xF4\xF5\xF6\xF7\xF8\xF9\xFA\xFB\xFC\xFD\xFE\xFF";
		
		[Test]
		public void VerifyFromIso8859_1()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "LEGACY";
			string rhsEncodingId = "UNICODE";
			ConvType conversionType = ConvType.Legacy_to_from_Unicode;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-1", "ISO-8859-1", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			string output = icuConv.ConvertToUnicode(m_bytesIso8859);
			Assert.AreEqual(m_convertedIso8859_1, output);
		}
		
		[Test]
		public void VerifyToIso8859_1()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "UNICODE";
			string rhsEncodingId = "LEGACY";
			ConvType conversionType = ConvType.Unicode_to_from_Legacy;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-1", "ISO-8859-1", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			byte[] output = icuConv.ConvertFromUnicode(m_convertedIso8859_1);
			Assert.AreEqual(m_bytesIso8859, output);
		}
		
		string m_convertedIso8859_10 = " !\"#$%&'()*+,-./0123456789:;<=>?" +
				"@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
				"\u00A0\u0104\u0112\u0122\u012A\u0128\u0136\u00A7" +
				"\u013B\u0110\u0160\u0166\u017D\u00AD\u016A\u014A" +
				"\u00B0\u0105\u0113\u0123\u012B\u0129\u0137\u00B7" +
				"\u013C\u0111\u0161\u0167\u017E\u2015\u016B\u014B" +
				"\u0100\u00C1\u00C2\u00C3\u00C4\u00C5\u00C6\u012E" +
				"\u010C\u00C9\u0118\u00CB\u0116\u00CD\u00CE\u00CF" +
				"\u00D0\u0145\u014C\u00D3\u00D4\u00D5\u00D6\u0168" +
				"\u00D8\u0172\u00DA\u00DB\u00DC\u00DD\u00DE\u00DF" +
				"\u0101\u00E1\u00E2\u00E3\u00E4\u00E5\u00E6\u012F" +
				"\u010D\u00E9\u0119\u00EB\u0117\u00ED\u00EE\u00EF" +
				"\u00F0\u0146\u014D\u00F3\u00F4\u00F5\u00F6\u0169" +
				"\u00F8\u0173\u00FA\u00FB\u00FC\u00FD\u00FE\u0138";

		[Test]
		public void VerifyFromIso8859_10()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "LEGACY";
			string rhsEncodingId = "UNICODE";
			ConvType conversionType = ConvType.Legacy_to_from_Unicode;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-10", "iso-8859_10-1998", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			string output = icuConv.ConvertToUnicode(m_bytesIso8859);
			Assert.AreEqual(m_convertedIso8859_10, output);
		}
		
		[Test]
		public void VerifyToIso8859_10()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "UNICODE";
			string rhsEncodingId = "LEGACY";
			ConvType conversionType = ConvType.Unicode_to_from_Legacy;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-1", "ISO-8859-1", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			byte[] output = icuConv.ConvertFromUnicode(m_convertedIso8859_1);
			Assert.AreEqual(m_bytesIso8859, output);
		}

		string m_convertedIso8859_11 = " !\"#$%&'()*+,-./0123456789:;<=>?" +
				"@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
				"\u00A0\u0E01\u0E02\u0E03\u0E04\u0E05\u0E06\u0E07" +
				"\u0E08\u0E09\u0E0A\u0E0B\u0E0C\u0E0D\u0E0E\u0E0F" +
				"\u0E10\u0E11\u0E12\u0E13\u0E14\u0E15\u0E16\u0E17" +
				"\u0E18\u0E19\u0E1A\u0E1B\u0E1C\u0E1D\u0E1E\u0E1F" +
				"\u0E20\u0E21\u0E22\u0E23\u0E24\u0E25\u0E26\u0E27" +
				"\u0E28\u0E29\u0E2A\u0E2B\u0E2C\u0E2D\u0E2E\u0E2F" +
				"\u0E30\u0E31\u0E32\u0E33\u0E34\u0E35\u0E36\u0E37" +
				"\u0E38\u0E39\u0E3A\uFFFD\uFFFD\uFFFD\uFFFD\u0E3F" +
				"\u0E40\u0E41\u0E42\u0E43\u0E44\u0E45\u0E46\u0E47" +
				"\u0E48\u0E49\u0E4A\u0E4B\u0E4C\u0E4D\u0E4E\u0E4F" +
				"\u0E50\u0E51\u0E52\u0E53\u0E54\u0E55\u0E56\u0E57" +
				"\u0E58\u0E59\u0E5A\u0E5B\uFFFD\uFFFD\uFFFD\uFFFD";

		byte[] m_bytesValidIso8859_11 = new byte[183] {
			 32,  33,  34,  35,  36,  37,  38,  39,  40,  41,  42,  43,  44,  45,  46,  47,
			 48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,  63,
			 64,  65,  66,  67,  68,  69,  70,  71,  72,  73,  74,  75,  76,  77,  78,  79,
			 80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  91,  92,  93,  94,  95,
			 96,  97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
			112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 
			160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
			176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,
			192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
			208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 223,
			224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
			240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251
			};
		
		string m_validIso8859_11 = " !\"#$%&'()*+,-./0123456789:;<=>?" +
				"@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
				"\u00A0\u0E01\u0E02\u0E03\u0E04\u0E05\u0E06\u0E07" +
				"\u0E08\u0E09\u0E0A\u0E0B\u0E0C\u0E0D\u0E0E\u0E0F" +
				"\u0E10\u0E11\u0E12\u0E13\u0E14\u0E15\u0E16\u0E17" +
				"\u0E18\u0E19\u0E1A\u0E1B\u0E1C\u0E1D\u0E1E\u0E1F" +
				"\u0E20\u0E21\u0E22\u0E23\u0E24\u0E25\u0E26\u0E27" +
				"\u0E28\u0E29\u0E2A\u0E2B\u0E2C\u0E2D\u0E2E\u0E2F" +
				"\u0E30\u0E31\u0E32\u0E33\u0E34\u0E35\u0E36\u0E37" +
				"\u0E38\u0E39\u0E3A\u0E3F" +
				"\u0E40\u0E41\u0E42\u0E43\u0E44\u0E45\u0E46\u0E47" +
				"\u0E48\u0E49\u0E4A\u0E4B\u0E4C\u0E4D\u0E4E\u0E4F" +
				"\u0E50\u0E51\u0E52\u0E53\u0E54\u0E55\u0E56\u0E57" +
				"\u0E58\u0E59\u0E5A\u0E5B";
		
		[Test]
		public void VerifyFromIso8859_11()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "LEGACY";
			string rhsEncodingId = "UNICODE";
			ConvType conversionType = ConvType.Legacy_to_from_Unicode;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-11", "iso-8859_11-2001", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			string output = icuConv.ConvertToUnicode(m_bytesIso8859);
			Assert.AreEqual(m_convertedIso8859_11, output);
			string output2 = icuConv.ConvertToUnicode(m_bytesValidIso8859_11);
			Assert.AreEqual(m_validIso8859_11, output2);
		}
		
		[Test]
		public void VerifyToIso8859_11()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "UNICODE";
			string rhsEncodingId = "LEGACY";
			ConvType conversionType = ConvType.Unicode_to_from_Legacy;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-11", "iso-8859_11-2001", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			byte[] output = icuConv.ConvertFromUnicode(m_validIso8859_11);
			Assert.AreEqual(m_bytesValidIso8859_11, output);
		}
		
		string m_convertedIso8859_14 = " !\"#$%&'()*+,-./0123456789:;<=>?" +
				"@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
				"\u00A0\u1E02\u1E03\u00A3\u010A\u010B\u1E0A\u00A7" +
				"\u1E80\u00A9\u1E82\u1E0B\u1EF2\u00AD\u00AE\u0178" +
				"\u1E1E\u1E1F\u0120\u0121\u1E40\u1E41\u00B6\u1E56" +
				"\u1E81\u1E57\u1E83\u1E60\u1EF3\u1E84\u1E85\u1E61" +
				"\u00C0\u00C1\u00C2\u00C3\u00C4\u00C5\u00C6\u00C7" +
				"\u00C8\u00C9\u00CA\u00CB\u00CC\u00CD\u00CE\u00CF" +
				"\u0174\u00D1\u00D2\u00D3\u00D4\u00D5\u00D6\u1E6A" +
				"\u00D8\u00D9\u00DA\u00DB\u00DC\u00DD\u0176\u00DF" +
				"\u00E0\u00E1\u00E2\u00E3\u00E4\u00E5\u00E6\u00E7" +
				"\u00E8\u00E9\u00EA\u00EB\u00EC\u00ED\u00EE\u00EF" +
				"\u0175\u00F1\u00F2\u00F3\u00F4\u00F5\u00F6\u1E6B" +
				"\u00F8\u00F9\u00FA\u00FB\u00FC\u00FD\u0177\u00FF";

		[Test]
		public void VerifyFromIso8859_14()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "LEGACY";
			string rhsEncodingId = "UNICODE";
			ConvType conversionType = ConvType.Legacy_to_from_Unicode;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-14", "iso-8859_14-1998", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			string output = icuConv.ConvertToUnicode(m_bytesIso8859);
			Assert.AreEqual(m_convertedIso8859_14, output);
		}
		
		[Test]
		public void VerifyToIso8859_14()
		{
			IcuConvEncConverter icuConv = new IcuConvEncConverter();
			string lhsEncodingId = "UNICODE";
			string rhsEncodingId = "LEGACY";
			ConvType conversionType = ConvType.Unicode_to_from_Legacy;
			int processTypeFlags = (int)ProcessTypeFlags.ICUConverter;
			int codePageInput = 0;
			int codePageOutput = 0;
			icuConv.Initialize("ISO-8859-1", "ISO-8859-1", ref lhsEncodingId, ref rhsEncodingId,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput, false);
			byte[] output = icuConv.ConvertFromUnicode(m_convertedIso8859_1);
			Assert.AreEqual(m_bytesIso8859, output);
		}
	}
}
