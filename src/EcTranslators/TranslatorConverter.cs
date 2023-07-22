using ECInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SilEncConverters40.EcTranslators
{
	public abstract class TranslatorConverter : EncConverter
	{
		public const int WarnEveryXRequests = 100;

		public int RequestCount { get; set; }

		protected TranslatorConverter(string sProgId, string sImplementType)
			: base(sProgId, sImplementType)
		{
		}

		protected void CheckOverusage()
		{
			if ((++RequestCount % WarnEveryXRequests) == 0)
			{
				throw new ApplicationException($"The {ImplementType} converter is a metered and limited connection that is shared by all users of SILConverters/EncConverters. Please avoid using it to translate large quantities of text without getting your own resource key (as described in the About tab)");
			}
		}

		[CLSCompliant(false)]
		internal static unsafe void StringToProperByteStar(string strOutput, byte* lpOutBuffer, ref int rnOutLen)
		{
			int nLen = strOutput.Length * 2;
			if (nLen > (int)rnOutLen)
				EncConverters.ThrowError(ErrStatus.OutputBufferFull);
			rnOutLen = nLen;
			ECNormalizeData.StringToByteStar(strOutput, lpOutBuffer, rnOutLen, false);
		}

		internal static string LogExceptionMessage(string className, Exception ex)
		{
			string msg = "Error occurred: " + ex.Message;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
				msg += $"{Environment.NewLine}because: (InnerException): {ex.Message}";
			}

			Util.DebugWriteLine(className, msg);
			return msg;
		}
	}
}
