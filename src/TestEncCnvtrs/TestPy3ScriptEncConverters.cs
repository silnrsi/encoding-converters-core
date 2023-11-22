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
using NUnit.Framework.Interfaces;

namespace TestEncCnvtrs
{
    /// ------------------------------------------------------------------------
    /// <summary>
    /// Set of tests to excercise the methods in the EncConverters class.
    /// </summary>
    /// ------------------------------------------------------------------------
    [TestFixture]
    public class TestPy3ScriptEncConverters
    {
        EncConverters m_encConverters;
        bool m_fWroteRepoFile;
        bool m_fSetRegistryValue;
        string m_repoFile;

        // best if you use these as the install path for the two flavors of 3.9 Python for these tests
        private const string PathPython64 = @"D:\Python\Python39";
        private const string PathPython86 = @"D:\Python\Python39-32";

        private const string EnvironmentVariableNamePythonHome = "PYTHONHOME";

        string m_originalPythonHome;
        string m_testPythonHome;

        /// --------------------------------------------------------------------
        /// <summary>
        /// Global initialization, called once before any test in this class
        /// ("fixture") is run.
        /// </summary>
        /// --------------------------------------------------------------------
        [OneTimeSetUp]
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
                //    I used C:\Python27x86 and C:\Python27x64 for their installation folders respectively,
                //    the code below should also work if x86 is installed in just the plain c:\Python27 folder too
                m_originalPythonHome = Environment.GetEnvironmentVariable(EnvironmentVariableNamePythonHome);
                m_testPythonHome = null;

                if (Environment.Is64BitProcess)
                {
                    if (!Directory.Exists(PathPython64))
                    {
                        Assert.Fail("The Python tests below will not work unless you have a x64 version of Python installed");
                    }
                    else
                    {
                        m_testPythonHome = PathPython64;
                    }
                }
                else  // x86
                {
                    if (!Directory.Exists(PathPython86))
                    {
                        Assert.Fail("The Python tests below will not work unless you have a x86 version of Python installed");
                    }
                    else
                    {
                        m_testPythonHome = PathPython86;
                    }
                }

                InitializePythonHomePath(m_testPythonHome);
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

        private void InitializePythonHomePath(string pythonHomePath)
        {
            // set the environment variable to the proper installation
            if (!String.IsNullOrEmpty(pythonHomePath) && (m_originalPythonHome != pythonHomePath))
            {
                Environment.SetEnvironmentVariable(EnvironmentVariableNamePythonHome, pythonHomePath);
            }
            var path = Environment.GetEnvironmentVariable(EnvironmentVariableNamePythonHome);
            Assert.True(String.IsNullOrEmpty(pythonHomePath) || (path == pythonHomePath));
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
            if (m_encConverters != null)
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
            if (m_encConverters.ContainsKey("UnitTesting-Python3-ReverseString-Unicode"))
                m_encConverters.Remove("UnitTesting-Python3-ReverseString-Unicode");
        }

        /// --------------------------------------------------------------------
        /// <summary>
        /// Global cleanup, called once after all tests in this class
        /// ("fixture") have been run.
        /// </summary>
        /// --------------------------------------------------------------------
        [OneTimeTearDown]
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
        /// This is a test for Python 3 via the Py3ScriptEncConverter and requires a Python 3.9 to be installed in the D:\Python\Python39 folder
        /// So that the file, D:\Python\Python39\python39.dll will exist
        /// Note that unless someone complains, this transducer will only support Unicode encoding
        /// </summary>
        [Test]
        public void TestPy3ScriptEncConverter()
        {
            var converterName = "UnitTesting-Python3-ReverseString-Unicode";
            var dir = Path.Combine(TestEncConverters.GetMapsTablesFolder(), "PythonExamples");
            var converterIdentifierForPython3 = @$"{Path.Combine(dir, "ReverseString.py")};{m_testPythonHome}\python39.dll";
            m_encConverters.AddConversionMap(converterName, converterIdentifierForPython3,
                                             ConvType.Unicode_to_from_Unicode, EncConverters.strTypeSILPy3Script,
                                             "UNICODE", "UNICODE", ProcessTypeFlags.PythonScript);

            var theEc = m_encConverters[converterName];
            var strOutput = theEc.Convert("abcde");
            Assert.AreEqual("edcba", strOutput);

            m_encConverters.Remove(converterName);    // to hurry the release of the environment
        }
    }
}
