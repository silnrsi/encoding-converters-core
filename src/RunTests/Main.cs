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
			var test = new TestEncConverters();
			test.InitForClass();
			try
			{
				test.InitBeforeTest();
				try { test.TestBasics(); }
				catch(Exception e) {Console.WriteLine("TestBasics failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.TestIcuConvEncConverters(); }
				catch(Exception e) {Console.WriteLine("TestIcuConvEncConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.TestIcuTransliterators(); }
				catch(Exception e) {Console.WriteLine("TestIcuTransliterators failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.TestIcuRegexConverters(); }
				catch(Exception e) {Console.WriteLine("TestIcuRegexConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestTecKitConverters(); }
				catch(Exception e) {Console.WriteLine("TestTecKitConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestTecKitMapConverters(); }
				catch(Exception e) {Console.WriteLine("TestTecKitMapConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestCcEncConverters(); }
				catch(Exception e) {Console.WriteLine("TestCcEncConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestPerlEncConverter(); }
				catch(Exception e) {Console.WriteLine("TestPerlEncConverter failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
				
				test.InitBeforeTest();
				try { test.TestPyScriptEncConverters(); }
				catch(Exception e) {Console.WriteLine("TestPyScriptEncConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestEncConverters setup or teardown: {0}", e.Message); }
			test.CleanupForClass();
		}
		
		public static void RunTestIcuConvEncConverter()
		{
			var test = new TestIcuConvEncConverter();
			try
			{
				try { test.CheckForIcuConverters(); }
				catch(Exception e) {Console.WriteLine("CheckForIcuConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_1(); }
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_1 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_1(); }
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_1 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_10(); }
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_10 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_10(); }
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_10 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_11(); }
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_11 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_11(); }
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_11 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyFromIso8859_14(); }
				catch(Exception e) {Console.WriteLine("VerifyFromIso8859_14 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.VerifyToIso8859_14(); }
				catch(Exception e) {Console.WriteLine("VerifyToIso8859_14 failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestIcuConvEncConverter setup or teardown: {0}", e.Message); }
		}
		
		public static void RunTestIcuTranslit()
		{
			var test = new TestIcuTranslit();
			try
			{
				try { test.CheckForIcuTransliterators(); }
				catch(Exception e) {Console.WriteLine("CheckForIcuTransliterators failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.TestLatin_GreekTranslit(); }
				catch(Exception e) {Console.WriteLine("TestLatin_GreekTranslit failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				try { test.TestGreek_LatinTranslit(); }
				catch(Exception e) {Console.WriteLine("TestGreek_LatinTranslit failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestIcuTranslit setup or teardown: {0}", e.Message); }
		}
		
		public static void RunTestIcuRegex()
		{
			var test = new TestIcuRegex();
			try
			{
			try { test.TestRegexToVandC(); }
			catch(Exception e) {Console.WriteLine("TestRegexToVandC failed: {0}", e.Message); ++m_cfail;}
			++m_crun;
			}
			catch(Exception e) { Console.WriteLine("Exception caught during TestIcuRegex setup or teardown: {0}", e.Message); }
		}
	}
}
