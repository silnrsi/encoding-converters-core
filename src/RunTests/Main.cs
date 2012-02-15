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
				try { test.TestDiscovery(); }
				catch(Exception e) {Console.WriteLine("TestDiscovery failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.AddIcuConvEncConverters(); }
				catch(Exception e) {Console.WriteLine("AddIcuConvEncConverters failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.AddIcuTransliterators(); }
				catch(Exception e) {Console.WriteLine("AddIcuTransliterators failed: {0}", e.Message); ++m_cfail;}
				++m_crun;
				test.CleanupAfterTest();
	
				test.InitBeforeTest();
				try { test.AddIcuRegexConverters(); }
				catch(Exception e) {Console.WriteLine("AddIcuRegexConverters failed: {0}", e.Message); ++m_cfail;}
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
