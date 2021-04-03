// created by Steve McConnel, Feb 7, 2012.
// 18-May-13 JDK  Overwrite repo file if it exists.
// 29-Jun-13 JDK  Test all python scripts in MapsTables.

using System;
using System.IO;
using Microsoft.Win32;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;
using System.Reflection;

namespace TestEncCnvtrs
{
    /// ------------------------------------------------------------------------
    /// <summary>
    /// Set of tests to excercise the methods in the EncConverters class.
    /// </summary>
    /// ------------------------------------------------------------------------
    [TestFixture]
	public class TestPyScriptEncConverters
	{
		EncConverters m_encConverters;
		bool m_fWroteRepoFile;
		bool m_fSetRegistryValue;
		string m_repoFile;

		private const string EnvironmentVariableNamePythonHome = "PYTHONHOME";
		string m_originalPythonHome;
		string m_testPythonHome;

        /// --------------------------------------------------------------------
        /// <summary>
        /// Global initialization, called once before any test in this class
        /// ("fixture") is run.
        /// </summary>
        /// --------------------------------------------------------------------
        [TestFixtureSetUp]
		public void InitForClass()
		{
			Util.DebugWriteLine(this, "BEGIN");
			if (Util.IsUnix)
			{
				// Make sure we get set up to be able to access Registry.LocalMachine.
				string machine_store = Environment.GetEnvironmentVariable("MONO_REGISTRY_PATH");
				if (machine_store == null)
				{
					Console.WriteLine("First, make sure that " + Util.CommonAppDataPath() +
									  " exists and is writeable by everyone.");
					Console.WriteLine("eg, sudo mkdir -p " + Util.CommonAppDataPath() +
									  " && sudo chmod +wt " + Util.CommonAppDataPath());
					Console.WriteLine("Then add the following line to ~/.profile, logout, and login again.");
					Console.WriteLine("MONO_REGISTRY_PATH=" + Util.CommonAppDataPath() +
									  "/registry; export MONO_REGISTRY_PATH");
					// doesn't work on Maverick, but try it anyway...
					Environment.SetEnvironmentVariable("MONO_REGISTRY_PATH", Util.CommonAppDataPath() + "/registry");
				}
			}
			else
            {
				// for Python, we have to use the correct installation (and we need both x86 and x64
				//  to *be* installed if you want the tests to work in both x86 and x64 configurations)
				//	I used C:\Python27x86 and C:\Python27x64 for their installation folders respectively,
				//	the code below should also work if x86 is installed in just the plain c:\Python27 folder too
				m_originalPythonHome = Environment.GetEnvironmentVariable(EnvironmentVariableNamePythonHome);
				m_testPythonHome = null;

				if (Environment.Is64BitProcess)
                {
					if (!Directory.Exists(@"C:\Python27x64"))						
					{
						Assert.Fail("The Python tests below will not work unless you have a x64 version of Python installed");
					}
					else 
                    {
						m_testPythonHome = @"C:\Python27x64";
					}
				}
				else  // x86
                {
					if (!Directory.Exists(@"C:\Python27x86") && !Directory.Exists(@"C:\Python27"))
					{
						Assert.Fail("The Python tests below will not work unless you have a x86 version of Python installed");
					}
					else if (Directory.Exists(@"C:\Python27x86"))
					{
						m_testPythonHome = @"C:\Python27x86";
					}
					else if (Directory.Exists(@"C:\Python27"))
					{
						m_testPythonHome = @"C:\Python27";
					}
				}

				// set the environment variable to the proper installation
				if (!String.IsNullOrEmpty(m_testPythonHome) && (m_originalPythonHome != m_testPythonHome))
                {
					Environment.SetEnvironmentVariable(EnvironmentVariableNamePythonHome, m_testPythonHome);
				}
				var path = Environment.GetEnvironmentVariable(EnvironmentVariableNamePythonHome);
				Assert.True(String.IsNullOrEmpty(m_testPythonHome) || (path == m_testPythonHome));
			}

			m_repoFile = null;
			RegistryKey key = Registry.CurrentUser.OpenSubKey(EncConverters.HKLM_PATH_TO_XML_FILE, true);
			if (key != null)
				m_repoFile = key.GetValue(EncConverters.strRegKeyForStorePath) as string;
			else
				key = Registry.CurrentUser.CreateSubKey(EncConverters.HKLM_PATH_TO_XML_FILE);
			if (String.IsNullOrEmpty(m_repoFile) && key != null)
			{
				m_repoFile = EncConverters.DefaultRepositoryPath;
				key.SetValue(EncConverters.strRegKeyForStorePath, m_repoFile);
				key.Flush();
				m_fSetRegistryValue = true;
			}
			if (key != null)
			{
				key.Close();
				key = null;
			}
			if (!String.IsNullOrEmpty(m_repoFile) &&
				File.Exists(m_repoFile) &&
				!File.Exists(m_repoFile + "-RESTOREME"))
			{
				File.Move(m_repoFile, m_repoFile + "-RESTOREME");
			}
			if (!File.Exists(m_repoFile))
				m_fWroteRepoFile = true;
			try
			{
				m_encConverters = new EncConverters();
			}
			catch (Exception e)
			{
				Util.DebugWriteLine(this, e.Message);
			}
			Util.DebugWriteLine(this, "END");
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Initialization called before each test in this class ("fixture") is
		/// run.
		/// </summary>
		/// --------------------------------------------------------------------
		[SetUp]
		public void InitBeforeTest()
		{
			if (m_encConverters != null)
			{
				RemoveAnyAddedConverters();
				m_encConverters.Reinitialize();
			}
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Cleanup called after each test in this class ("fixture") is run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TearDown]
		public void CleanupAfterTest()
		{
			RemoveAnyAddedConverters();

			// reset the python home env variable back to what it was before testing (if o
			Environment.SetEnvironmentVariable(EnvironmentVariableNamePythonHome, m_originalPythonHome);
		}

		/// <summary>
		/// Remove any added converters that are left behind due to a test
		/// failure or other crash.
		/// </summary>
		private void RemoveAnyAddedConverters()
		{
			if (m_encConverters.ContainsKey("UnitTesting-Python-UnicodeNames"))
				m_encConverters.Remove("UnitTesting-Python-UnicodeNames");
			if (m_encConverters.ContainsKey("UnitTesting-Python-1252-To-Unicode"))
				m_encConverters.Remove("UnitTesting-Python-1252-To-Unicode");
			if (m_encConverters.ContainsKey("UnitTesting-Python-Unicode-To-1252"))
				m_encConverters.Remove("UnitTesting-Python-Unicode-To-1252");

			if (m_encConverters.ContainsKey("UnitTesting-Python-ReverseString-Unicode"))
				m_encConverters.Remove("UnitTesting-Python-ReverseString-Unicode");
			if (m_encConverters.ContainsKey("UnitTesting-Python-ReverseString-Legacy"))
				m_encConverters.Remove("UnitTesting-Python-ReverseString-Legacy");
			if (m_encConverters.ContainsKey("UnitTesting-Python-ToLowerCase"))
				m_encConverters.Remove("UnitTesting-Python-ToLowerCase");
			if (m_encConverters.ContainsKey("UnitTesting-Python-ToUpperCase"))
				m_encConverters.Remove("UnitTesting-Python-ToUpperCase");
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Global cleanup, called once after all tests in this class
		/// ("fixture") have been run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TestFixtureTearDown]
		public void CleanupForClass()
		{
			m_encConverters = null;
			if (m_fWroteRepoFile)
			{
				File.Delete(m_repoFile);
				m_fWroteRepoFile = false;
			}
			if (!String.IsNullOrEmpty(m_repoFile) &&
				File.Exists(m_repoFile + "-RESTOREME"))
			{
				if (File.Exists(m_repoFile))
				{
					File.Delete(m_repoFile); // to overwrite file
				}
				File.Move(m_repoFile + "-RESTOREME", m_repoFile);
			}
			if (m_fSetRegistryValue)
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey(EncConverters.HKLM_PATH_TO_XML_FILE, true);
				key.DeleteValue(EncConverters.strRegKeyForStorePath);
				key.Flush();
				key.Close();
				m_fSetRegistryValue = false;
			}
			m_repoFile = null;
		}

		/// <summary>
		/// If this test fails the first time you run it, just run it again by itself. We're swapping environment
		/// variables in the init for this test class and if these tests run in parallel, it might skew the results.
		/// Usually, it runs fine by itself
		/// </summary>
		[Test]
		public void TestPyScriptEncConvertersAll()
		{
			Util.DebugWriteLine(this, "BEGIN");
			int countOrig = m_encConverters.Count;
			int countCurrent = countOrig;
			int n = 0;	// the current test number
			const int TOTAL_PY_TESTS = 9;
			string[] filenames = new string[TOTAL_PY_TESTS];
			string[] convnames = new string[TOTAL_PY_TESTS];

			var dir = TestEncConverters.GetTestSourceFolder();
			filenames[n] = "From1252.py";
			convnames[n] = "UnitTesting-Python-1252-To-Unicode";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Legacy_to_from_Unicode,
				"LEGACY", "UNICODE", ProcessTypeFlags.PythonScript);
			countCurrent++;
			int countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			string[] encodings = m_encConverters.Encodings;
			Assert.LessOrEqual(2, encodings.Length, "Should have at least 2 encodings now.");
			string[] mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(1, mappings.Length, "Should have at least 1 mapping now.");
			int countKeys = m_encConverters.Keys.Count;
			int countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(1, countKeys, "Should have at least one key now.");
			IEncConverter ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageInput = EncConverters.cnIso8859_1CodePage;
			string input = TestUtil.GetPseudoStringFromBytes(TestEncConverters.m_1252bytes);
			string output = ec.Convert(input);
			Assert.AreEqual(TestEncConverters.m_1252Converted, output, filenames[n] + " should convert data properly!");

			n++;
			filenames[n] = "To1252.py";
			convnames[n] = "UnitTesting-Python-Unicode-To-1252";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			mappings = m_encConverters.Mappings;
			Assert.LessOrEqual(2, mappings.Length, "Should have at least two mappings now.");
			countKeys = m_encConverters.Keys.Count;
			countValues = m_encConverters.Values.Count;
			Assert.AreEqual(countKeys, countValues, "Should have same number of keys and values!");
			Assert.LessOrEqual(2, countKeys, "Should have at least two keys now.");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			string outputRaw = ec.Convert(TestEncConverters.m_1252Converted);
			byte[] outputBytes = TestUtil.GetBytesFromPseudoString(outputRaw);
			Assert.AreEqual(TestEncConverters.m_1252bytes, outputBytes, filenames[n] + " should convert data properly!");

			dir = Path.Combine(TestEncConverters.GetMapsTablesFolder(), "PythonExamples");

			n++;
			filenames[n] = "ReverseString.py";
			convnames[n] = "UnitTesting-Python-ReverseString-Unicode";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			output = ec.Convert("\u0bae\u0bcb\u0baa");  // Tamil Ma + Oo + Pa
			Assert.AreEqual("\u0baa\u0bcb\u0bae", output, filenames[n] + " should convert data properly!");

			n++;
			filenames[n] = "ReverseString.py";
			convnames[n] = "UnitTesting-Python-ReverseString-Legacy";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Legacy_to_from_Legacy,
				"LEGACY", "LEGACY", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			output = ec.Convert("mop");
			Assert.AreEqual("pom", output, filenames[n] + " should convert data properly!");

			n++;
			filenames[n] = "ToLowerCase.py";
			convnames[n] = "UnitTesting-Python-ToLowerCase";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Legacy_to_from_Legacy,
				"LEGACY", "LEGACY", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			output = ec.Convert("ASdG");
			Assert.AreEqual("asdg", output, filenames[n] + " should convert data properly!");

			n++;
			filenames[n] = "ToUpperCase.py";
			convnames[n] = "UnitTesting-Python-ToUpperCase";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Legacy_to_from_Legacy,
				"LEGACY", "LEGACY", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			output = ec.Convert("asdg");
			Assert.AreEqual("ASDG", output, filenames[n] + " should convert data properly!");

			n++;
			filenames[n] = "ToUnicodeNames.py";
			convnames[n] = "UnitTesting-Python-UnicodeNames";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Unicode_to_from_Legacy,
				"UNICODE", "LEGACY", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			output = ec.Convert("\u0915\u093c");
			Assert.AreEqual("DEVANAGARI LETTER KA; DEVANAGARI SIGN NUKTA; ", output, filenames[n] + " should convert data properly!");
			n++;
			filenames[n] = "ToNfc.py";
			convnames[n] = "UnitTesting-Python-ToNfc";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			output = ec.Convert("\u0045\u0328");
			Assert.AreEqual("\u0118", output, filenames[n] + " should convert data properly!");

			n++;
			filenames[n] = "ToNfd.py";
			convnames[n] = "UnitTesting-Python-ToNfd";
			m_encConverters.Add(convnames[n], Path.Combine(dir, filenames[n]),
				ConvType.Unicode_to_from_Unicode,
				"UNICODE", "UNICODE", ProcessTypeFlags.PythonScript);
			countCurrent++;
			countNew = m_encConverters.Count;
			Assert.AreEqual(countCurrent, countNew, "Should have " + countCurrent + " converter(s) now");
			ec = m_encConverters[convnames[n]];
			Assert.IsNotNull(ec, "Added converter " + convnames[n] + " should exist!");
			ec.CodePageOutput = EncConverters.cnIso8859_1CodePage;
			output = ec.Convert("\u0958");
			Assert.AreEqual("\u0915\u093c", output, filenames[n] + " should convert data properly!");

			for (int i = 0; i < TOTAL_PY_TESTS; i++)
				m_encConverters.Remove(convnames[i]);
			int countAfter = m_encConverters.Count;
			Assert.AreEqual(countOrig, countAfter, "Should have the original number of converters now.");
			Util.DebugWriteLine(this, "END");
		}
	}
}
