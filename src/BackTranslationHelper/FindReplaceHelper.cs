using Microsoft.Win32;
using SilEncConverters40;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackTranslationHelper
{
	public class FindReplaceHelper
	{
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
			if (m_fnEditSpellingFixes != null)
			{
				m_fnEditSpellingFixes.Invoke(m_aSpellFixer, null);
			}
		}

		public void FindReplacementRule(string strWord)
		{
			if (m_fnFindReplacementRule != null)
			{
				object[] oParams = new object[] { strWord };
				m_fnFindReplacementRule.Invoke(m_aSpellFixer, oParams);
			}
		}
	}
}
