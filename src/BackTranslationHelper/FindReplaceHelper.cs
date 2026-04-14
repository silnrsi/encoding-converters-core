using Microsoft.Win32;
using System;
using System.Windows.Forms;


#if !UseReflection
using SpellingFixer30;
#endif

namespace BackTranslationHelper
{
	public class FindReplaceHelper
	{
#if !UseReflection
		public const string cstrSpellFixerProgID = "SpellingFixer30.SpellingFixer";
		public const string Caption = "FindReplaceHelper";

		protected CscProject m_cscProject = null;
		protected SpellingFixer m_aSpellFixerLegacy = null;

		public FindReplaceHelper()
		{
		}

		public FindReplaceHelper(string projectName)
		{
			if (projectName.StartsWith(SpellingFixer.SFConverterPrefix))
			{
				// remove the prefix to get the project name
				projectName = projectName.Substring(SpellingFixer.SFConverterPrefix.Length);
				m_aSpellFixerLegacy ??= new SpellingFixer(projectName);
			}
			else if (projectName.StartsWith(SpellingFixer.SFConverterPrefixCsc))
			{
				// remove the prefix to get the project name
				projectName = projectName.Substring(SpellingFixer.SFConverterPrefixCsc.Length);
				m_cscProject = CscProject.LoadProject(projectName);
			}
			else
			{
				throw new ArgumentException("Substitution Projects must have a CC table EncConverter that start with either '" + SpellingFixer.SFConverterPrefixCsc + "' (think: only whole word substitutions) or '" + SpellingFixer.SFConverterPrefix + "' (can be configured to allow within word changes).");
			}
		}

		protected bool IsCscProject
		{
			get { return (m_cscProject?.SpellFixerEncConverterName != null); }
		}

		protected bool IsSpellFixerLegacyProject
		{
			get { return (m_aSpellFixerLegacy?.SpellFixerEncConverterName != null); }
		}

		protected bool IsSpellFixerProject
		{
			get { return IsSpellFixerLegacyProject || IsCscProject; }
		}

		/// <summary>
		/// Use this method to query the user for which flavor of SpellFixer they want to use:
		/// Either the Consistent Spelling Checker (which does full word replacements), or the
		/// legacy SpellFixer (which can do full word or partial word replacements, but is more
		/// dangerous for novice users, since a non-full-word replacement of a word like 'bath'
		/// can also change 'bloodbath').
		/// </summary>
		public void QuerySpellFixProjectType()
		{
			try
			{
				m_aSpellFixerLegacy ??= new SpellingFixer();

				if (m_aSpellFixerLegacy.QuerySpellFixProject() == SpellFixerMode.eConsistentSpellingChecker)
					TrySelectProject();
				else
					TryLoginProject();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Caption);
			}
		}


		protected void TrySelectProject()
		{
			try
			{
				m_cscProject = CscProject.SelectProject();
			}
			finally
			{
				m_aSpellFixerLegacy = null;   // just in case
			}
		}

		protected void TryLoginProject()
		{
			try
			{
				if (m_aSpellFixerLegacy == null)
					m_aSpellFixerLegacy = new SpellingFixer();
				m_aSpellFixerLegacy.LoginProject();
			}
			finally
			{
				m_cscProject = null;   // just in case
			}
		}

		public static FindReplaceHelper GetFindReplaceHelper()
		{
			var helper = new FindReplaceHelper();
			if (!helper.IsCscProject && !helper.IsSpellFixerLegacyProject)
				helper.QuerySpellFixProjectType();

			return (!helper.IsCscProject && !helper.IsSpellFixerLegacyProject)
					? null
					: helper;
		}

		public string SpellFixerEncConverterName
		{
			get
			{
				if (IsCscProject)
					return m_cscProject.SpellFixerEncConverterName;
				else if (IsSpellFixerLegacyProject)
					return m_aSpellFixerLegacy.SpellFixerEncConverterName;
				else
					throw new InvalidOperationException("No SpellFixer project is currently selected.");
			}
		}

		public void AssignCorrectSpelling(string strInput)
		{
			try
			{
				if (IsCscProject)
					m_cscProject.AssignCorrectSpelling(strInput);
				else if (IsSpellFixerLegacyProject)
					m_aSpellFixerLegacy.AssignCorrectSpelling(strInput);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Caption);
			}
		}

		public void EditSpellingFixes()
		{
			try
			{
				if (IsCscProject)
					m_cscProject.EditSpellingFixes();
				else if (IsSpellFixerLegacyProject)
					m_aSpellFixerLegacy.EditSpellingFixes();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Caption);
			}
		}

		public void FindReplacementRule(string strWord)
		{
			try
			{
				if (IsCscProject)
					m_cscProject.FindReplacementRule(strWord);
				else if (IsSpellFixerLegacyProject)
					m_aSpellFixerLegacy.FindReplacementRule(strWord);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Caption);
			}
		}
#else
		// this is directly from SpellFixerEC.cs, but needed to avoid the dependency
		public const string cstrSpellFixerProgID = "SpellingFixerEC.SpellingFixerEC";

		protected Object m_aSpellFixer = null;
		protected MethodInfo m_fnAssignCorrectSpelling = null;
		protected MethodInfo m_fnFindReplacementRule = null;
		protected MethodInfo m_fnQueryForSpellingCorrectionIfTableEmpty = null;
		protected MethodInfo m_fnLoginProject = null;
		protected MethodInfo m_fnEditSpellingFixes = null;
		protected PropertyInfo m_propSpellFixerEncConverter = null;
		protected PropertyInfo m_propSpellFixerEncConverterName = null;
		protected PropertyInfo m_propProjectFont = null;

		public static FindReplaceHelper GetFindReplaceHelper()
		{
			var theSpellFixerByReflection = new SpellFixerByReflection();
			theSpellFixerByReflection.LoginProject();
			var projectName = theSpellFixerByReflection.SpellFixerEncConverterName;
			return new FindReplaceHelper(projectName);
		}

		public FindReplaceHelper(string projectName)
		{
			// double-check that this should work...
			Type typeSpellFixer = Type.GetTypeFromProgID(cstrSpellFixerProgID);
			if (typeSpellFixer != null)
			{
				Type[] aTypeParams = new Type[] { typeof(string) };
				m_fnAssignCorrectSpelling = typeSpellFixer.GetMethod("AssignCorrectSpelling", aTypeParams);
				m_fnFindReplacementRule = typeSpellFixer.GetMethod("FindReplacementRule", aTypeParams);
				m_fnQueryForSpellingCorrectionIfTableEmpty = typeSpellFixer.GetMethod("QueryForSpellingCorrectionIfTableEmpty", aTypeParams);
				m_fnLoginProject = typeSpellFixer.GetMethod("LoginProject");
				m_fnEditSpellingFixes = typeSpellFixer.GetMethod("EditSpellingFixes");
				m_propSpellFixerEncConverter = typeSpellFixer.GetProperty("SpellFixerEncConverter");
				m_propSpellFixerEncConverterName = typeSpellFixer.GetProperty("SpellFixerEncConverterName");
				m_propProjectFont = typeSpellFixer.GetProperty("ProjectFont");
				m_aSpellFixer = Activator.CreateInstance(typeSpellFixer, new[] { projectName });
			}
		}

		public string SpellFixerEncConverterName
		{
			get
			{
				string strName = null;
				if (m_propSpellFixerEncConverterName != null)
					strName = (string)m_propSpellFixerEncConverterName.GetValue(m_aSpellFixer, null);
				return strName;
			}
		}

		public void AssignCorrectSpelling(string strInput)
		{
			if (m_fnAssignCorrectSpelling != null)
			{
				object[] oParams = new object[] { strInput };
				m_fnAssignCorrectSpelling.Invoke(m_aSpellFixer, oParams);
			}
		}

		public void QueryForSpellingCorrectionIfTableEmpty(string strBadWord)
		{
			if (m_fnQueryForSpellingCorrectionIfTableEmpty != null)
			{
				object[] oParams = new object[] { strBadWord };
				m_fnQueryForSpellingCorrectionIfTableEmpty.Invoke(m_aSpellFixer, oParams);
			}
		}

		public void EditSpellingFixes()
		{
			m_fnEditSpellingFixes?.Invoke(m_aSpellFixer, null);
		}

		public void FindReplacementRule(string strWord)
		{
			if (m_fnFindReplacementRule != null)
			{
				object[] oParams = new object[] { strWord };
				m_fnFindReplacementRule.Invoke(m_aSpellFixer, oParams);
			}
		}
#endif

		public static bool IsSpellFixerAvailable
		{
			get
			{
				//Console.WriteLine("Looking for key " + cstrSpellFixerProgID);
				RegistryKey keySF = Registry.ClassesRoot.OpenSubKey(cstrSpellFixerProgID, false);
				if (keySF != null)
				{
					try
					{
						// if we can get the type, then we should be able to instantiate it (don't waste
						//  the time actually trying to instantiate it, a) because it takes a long time, and
						//  b) unless we *really* want to load it, we don't want the user to have to choose
						//  a project (the default constructor of SF puts up a choose project dialog).
						Type typeSpellFixer = Type.GetTypeFromProgID(cstrSpellFixerProgID);
						if (typeSpellFixer != null)
							return true;
					}
					catch { }
				}

				return false;
			}
		}
	}
}
