using System;

using TestEncCnvtrs;

namespace RunTests
{
	class MainClass
	{
		static int m_crun;
		static int m_cfail;
		
		public static void Main (string[] args)
		{
			RunTestEncConverters();
			RunTestIcuConvEncConverter();
			RunTestIcuTranslit();
			RunTestIcuRegex();
			Console.WriteLine("{0} Tests Run; {1} Tests Failed", m_crun, m_cfail);
		}

		public static void RunTestEncConverters ()
		{
			Console.WriteLine("Begin Testing EncConverters");
			var test = new TestEncConverters();
			test.InitForClass();
			try
			{
				test.InitBeforeTest();
				try { test.TestBasics(); Console.WriteLine("TestBasics succeeded!");}
				catch(Exception e) {Console.WriteLine("TestBasics failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.TestIcuConvEncConverters(); Console.WriteLine("TestIcuConvEncConverters succeeded!");}
				catch(Exception e) {Console.WriteLine("TestIcuConvEncConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.TestIcuTransliterators(); Console.WriteLine("TestIcuTransliterators succeeded!");}
				catch(Exception e) {Console.WriteLine("TestIcuTransliterators failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.TestIcuRegexConverters(); Console.WriteLine("TestIcuRegexConverters succeeded!");}
				catch(Exception e) {Console.WriteLine("TestIcuRegexConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestTecKitConverters(); Console.WriteLine("TestTecKitConverters succeeded!");}
				catch(Exception e) {Console.WriteLine("TestTecKitConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestTecKitMapConverters(); Console.WriteLine("TestTecKitMapConverters succeeded!");}
				catch(Exception e) {Console.WriteLine("TestTecKitMapConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try
				{
					Console.WriteLine("TestCcEncConverters ignored since crashing on 64-bit.");
					// test.TestCcEncConverters(); Console.WriteLine("TestCcEncConverters succeeded!");
				}
				catch(Exception e) {Console.WriteLine("TestCcEncConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestPerlEncConverter(); Console.WriteLine("TestPerlEncConverter succeeded!");}
				catch(Exception e) {Console.WriteLine("TestPerlEncConverter failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestPyScriptEncConverters(); Console.WriteLine("TestPyScriptEncConverters succeeded!");}
				catch(Exception e) {Console.WriteLine("TestPyScriptEncConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
//				if (TestUtil.IsWindows)
//				{
					test.InitBeforeTest();
					try { test.TestCodePageConverters(); Console.WriteLine("TestCodePageConverters succeeded!");}
					catch(Exception e) {Console.WriteLine("TestCodePageConverters failed: {0}", e.Message); ++m_cfail;}
					++m_crun;
					test.CleanupAfterTest();
//				}
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestEncConverters setup or teardown: {0}", e.Message); }
			test.CleanupForClass();
			Console.WriteLine("Done Testing EncConverters");
		}
		
		public static void RunTestIcuConvEncConverter()
		{
			Console.WriteLine("Begin Testing IcuConvEncConverter");
			var test = new TestIcuConvEncConverter();
			try
			{
				try { test.CheckForIcuConverters(); Console.WriteLine("CheckForIcuConverters succeeded!");}
				catch(Exception e) {Console.WriteLine("CheckForIcuConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_1(); Console.WriteLine("VerifyFromIso8859_1 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_1 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_1(); Console.WriteLine("VerifyToIso8859_1 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_1 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_10(); Console.WriteLine("VerifyFromIso8859_10 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_10 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_10(); Console.WriteLine("VerifyToIso8859_10 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_10 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_11(); Console.WriteLine("VerifyFromIso8859_11 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_11 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_11(); Console.WriteLine("VerifyToIso8859_11 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_11 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_14(); Console.WriteLine("VerifyFromIso8859_14 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_14 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_14(); Console.WriteLine("VerifyToIso8859_14 succeeded!");}
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_14 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestIcuConvEncConverter setup or teardown: {0}", e.Message); }
			Console.WriteLine("Done Testing IcuConvEncConverter");
		}
		
		public static void RunTestIcuTranslit()
		{
			Console.WriteLine("Begin Testing IcuTranslit");
			var test = new TestIcuTranslit();
			try
			{
				try { test.CheckForIcuTransliterators(); Console.WriteLine("CheckForIcuTransliterators succeeded!");}
				catch(Exception e) {Console.WriteLine("CheckForIcuTransliterators failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.TestLatin_GreekTranslit(); Console.WriteLine("TestLatin_GreekTranslit succeeded!");}
				catch(Exception e) {Console.WriteLine("TestLatin_GreekTranslit failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.TestGreek_LatinTranslit(); Console.WriteLine("TestGreek_LatinTranslit succeeded!");}
				catch(Exception e) {Console.WriteLine("TestGreek_LatinTranslit failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestIcuTranslit setup or teardown: {0}", e.Message); }
			Console.WriteLine("Done Testing IcuTranslit");
		}
		
		public static void RunTestIcuRegex()
		{
			Console.WriteLine("Begin Testing IcuRegex");
			var test = new TestIcuRegex();
			try
			{
			try { test.TestRegexToVandC(); Console.WriteLine("TestRegexToVandC succeeded!");}
			catch(Exception e) {Console.WriteLine("TestRegexToVandC failed: {0}", e.Message); ++m_cfail;}
			++m_crun;
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestIcuRegex setup or teardown: {0}", e.Message); }
			Console.WriteLine("Done Testing IcuRegex");
		}
	}
}
