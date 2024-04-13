using ECInterfaces;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SilEncConverters40.EcTranslators
{
	public abstract class TranslatorConverter : EncConverter
	{
		public const int WarnEveryXRequests = 100;

		public int RequestCount { get; set; }

		public abstract bool HasUserOverriddenCredentials { get; }

		protected TranslatorConverter(string sProgId, string sImplementType)
			: base(sProgId, sImplementType)
		{
		}

		protected void CheckOverusage()
		{
			if (!HasUserOverriddenCredentials && (++RequestCount % WarnEveryXRequests) == 0)
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

		public static string LogExceptionMessage(string className, Exception ex)
		{
			var message = ex.Message;
			var msg = "Error occurred: " + message;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
				if (message.Contains(ex.Message))
					continue;   // skip identical msgs
				message = ex.Message;
				msg += $"{Environment.NewLine}because: (InnerException): {message}";
			}

			Util.DebugWriteLine(className, msg);
			return msg;
		}

		public static string LoadEmbeddedResourceFileAsStringExecutingAssembly(string strResourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			strResourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.Contains(strResourceName));
			if (string.IsNullOrEmpty(strResourceName))
				return null;

			var resourceAsStream = assembly.GetManifestResourceStream(strResourceName);
			StreamReader reader = new StreamReader(resourceAsStream);
			string text = reader.ReadToEnd();
			return text;
		}
	}
}
