using ECInterfaces;
using SilEncConverters40.EcTranslators.AzureOpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilEncConverters40.EcTranslators
{
	public abstract class PromptExeTranslator : ExeEncConverter
	{
		public List<string> ExamplesInputString { get; set; } = new List<string>();
		public List<string> ExamplesOutputString { get; set; } = new List<string>();


		public PromptExeTranslator
			(
				string strProgramID,									// usually "typeof(<classname>).FullName", e.g. typeof(AzureOpenAiEncConverter).FullName
				string strImplType,										// e.g. (cf. SIL.AzureOpenAi)
				Int32 lProcessType = (int)ProcessTypeFlags.Translation,	// e.g. ProcessTypeFlags.Translation
				string strWorkingDirSuffix = ""							// e.g. @"\SIL\Indic\ITrans" (if installed there)
			)
			: base(strProgramID, strImplType, ConvType.Unicode_to_Unicode, "UNICODE", "UNICODE", lProcessType, strWorkingDirSuffix)
		{
		}

		public virtual void AddExample(string inputString, string outputString)
		{
			ExamplesInputString.Add(inputString);
			ExamplesOutputString.Add(outputString);
			m_psi = null;	// to force it to re-pull the Arguments, including these examples
		}

		public virtual void PurgeExamples()
		{
			ExamplesInputString.Clear();
			ExamplesOutputString.Clear();
			m_psi = null;   // to force it to re-pull the Arguments, including these examples
		}
	}
}
