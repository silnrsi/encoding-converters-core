// created by Steve McConnel, Feb 7, 2012.

using System;
using System.IO;
using Microsoft.Win32;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;

namespace SilEncConvertersTests
{
	/// ------------------------------------------------------------------------
	/// <summary>
	/// Set of tests to excercise the methods in the EncConverters class.
	/// </summary>
	/// ------------------------------------------------------------------------
	[TestFixture]
	public class TestEncConverters
	{
		EncConverters m_encConverters;
		bool m_fWroteRepoFile;
		string m_repoFile;
		
		/// --------------------------------------------------------------------
		/// <summary>
		/// Global initialization, called once before any test in this class
		/// ("fixture") is run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TestFixtureSetUp]
		public void ClassInit()
		{
			// Make sure we get set up to be able to access Registry.LocalMachine.
			string machine_store = Environment.GetEnvironmentVariable("MONO_REGISTRY_PATH");
			if (machine_store == null)
			{
				Console.WriteLine("First, make sure that /var/lib/fieldworks exists and is writeable by everyone.");
				Console.WriteLine("eg, sudo mkdir -p /var/lib/fieldworks && sudo chmod +wt /var/lib/fieldworks");
				Console.WriteLine("Then add the following line to ~/.profile, logout, and login again.");
				Console.WriteLine("MONO_REGISTRY_PATH=/var/lib/fieldworks/registry; export MONO_REGISTRY_PATH");
				// doesn't work on Maverick, but try it anyway...
				Environment.SetEnvironmentVariable("MONO_REGISTRY_PATH", "/var/lib/fieldworks/registry");
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
				// Now we need to write a good file to that location.
				// REVIEW: use EncConverters.strTypeSILtec etc. to fill in the type values in the strings below?
				StreamWriter writer = new StreamWriter(m_repoFile);
				writer.WriteLine("<?xml version=\"1.0\" standalone=\"yes\"?>");
				writer.WriteLine("<mappingRegistry xmlns=\"http://www.sil.org/computing/schemas/SILMappingRegistry.xsd\">");
				writer.WriteLine("  <mappings />");
				writer.WriteLine("  <fonts />");
				writer.WriteLine("  <implementations>");
				writer.WriteLine("    <platform name=\"COM\">");
				writer.WriteLine("      <implement type=\"SIL.AdaptItKB\" use=\"SilEncConverters40.AdaptItEncConverter\" priority=\"0\" />");
				writer.WriteLine("      <implement type=\"SIL.AdaptItKBGuesser\" use=\"SilEncConverters40.AdaptItGuesserEncConverter\" priority=\"0\" />");
				writer.WriteLine("      <implement type=\"SIL.cc\" use=\"SilEncConverters40.CcEncConverter\" priority=\"2\" />");
				writer.WriteLine("      <implement type=\"cp\" use=\"SilEncConverters40.CpEncConverter\" priority=\"6\" />");
				writer.WriteLine("      <implement type=\"SIL.comp\" use=\"SilEncConverters40.CmpdEncConverter\" priority=\"0\" />");
				writer.WriteLine("      <implement type=\"SIL.tec\" use=\"SilEncConverters40.TecEncConverter\" priority=\"3\" />");
				writer.WriteLine("      <implement type=\"SIL.map\" use=\"SilEncConverters40.TecEncConverter\" priority=\"4\" />");
				writer.WriteLine("      <implement type=\"ICU.conv\" use=\"SilEncConverters40.IcuConvEncConverter\" priority=\"1\" />");
				writer.WriteLine("      <implement type=\"ICU.regex\" use=\"SilEncConverters40.IcuRegexEncConverter\" priority=\"1\" />");
				writer.WriteLine("      <implement type=\"ICU.trans\" use=\"SilEncConverters40.IcuTranslitEncConverter\" priority=\"1\" />");
				writer.WriteLine("      <implement type=\"SIL.PerlExpression\" use=\"SilEncConverters40.PerlExpressionEncConverter\" priority=\"3\" />");
				writer.WriteLine("      <implement type=\"SIL.PyScript\" use=\"SilEncConverters40.PyScriptEncConverter\" priority=\"3\" />");
				//TechHindiSiteEncConverter
				//ITransEncConverter
				//UTransEncConverter
				//TecFormEncConverter
				//FallbackEncConverter
				//TechHindiSiteEncConverter
				writer.WriteLine("    </platform>");
				writer.WriteLine("  </implementations>");
				writer.WriteLine("</mappingRegistry>");
				writer.Close();
				writer.Dispose();
				writer = null;
				m_fWroteRepoFile = true;
			}
			if (key != null)
			{
				key.Close();
				key = null;
			}
			try
			{
				m_encConverters = new EncConverters();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
		}
		
		/// --------------------------------------------------------------------
		/// <summary>
		/// Initialization called before each test in this class ("fixture") is
		/// run.
		/// </summary>
		/// --------------------------------------------------------------------
		[SetUp]
		public void TestInit()
		{
			if (m_encConverters != null)
				m_encConverters.Reinitialize();
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Cleanup called after each test in this class ("fixture") is run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TearDown]
		public void TestCleanup()
		{
		}

		/// --------------------------------------------------------------------
		/// <summary>
		/// Global cleanup, called once after all tests in this class
		/// ("fixture") have been run.
		/// </summary>
		/// --------------------------------------------------------------------
		[TestFixtureTearDown]
		public void ClassCleanup()
		{
			m_encConverters = null;
			if (m_fWroteRepoFile)
			{
				File.Delete(m_repoFile);
				RegistryKey key = Registry.CurrentUser.OpenSubKey(EncConverters.HKLM_PATH_TO_XML_FILE, true);
				key.DeleteValue(EncConverters.strRegKeyForStorePath);
				key.Flush();
				key.Close();
				m_fWroteRepoFile = false;
			}
			m_repoFile = null;
		}
		
		[Test]
		public void TestCase ()
		{
		}
	}
}

