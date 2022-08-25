using System;
using System.IO;
using Microsoft.Win32;

using NUnit.Framework;

using ECInterfaces;
using SilEncConverters40;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace TestEncCnvtrs
{
	/// <summary>
	/// Utility methods useful for testing encoding converters.
	/// </summary>
	public static class TestUtil
	{
		/// <summary>
		/// Convert a byte array to a string by zero-padding all the bytes in the array.
		/// </summary>
		public static string GetPseudoStringFromBytes(byte[] bytes)
		{
			char[] rgch = new char[bytes.Length];
			for (int i = 0; i < bytes.Length; ++i)
				rgch[i] = (char)bytes[i];
			return new string(rgch);
		}

		/// <summary>
		/// Convert the string to a byte array by stripping the top byte from each character in
		/// the string.
		/// </summary>
		public static byte[] GetBytesFromPseudoString(string str)
		{
			byte[] bytes = new byte[str.Length];
			var mask = (str[0] & 0xFF00);
			for (int i = 0; i < str.Length; ++i)
			{
				bytes[i] = (byte)(str[i] & 0xFF);
				var newmask = (str[i] & 0xFF00);
				Assert.AreEqual(mask, newmask,
					String.Format("char[{0}] has a different upper byte ({1:x4}) than the first char in the string ({2:x4})", i, newmask, mask));
			}
			return bytes;
		}

		public static void WriteBytesForDebugging(byte[] bytes, string header)
		{
			if (!String.IsNullOrEmpty(header))
				Console.WriteLine(header);
			if (bytes != null)
			{
				Console.WriteLine("bytes.Length = {0}", bytes.Length);
				for (int i = 0; i < bytes.Length; ++i)
				{
					if (i > 0 && (i % 16) == 0)
						Console.WriteLine();
					Console.Write(" {0:x2}", bytes[i]);
				}
			}
			else
			{
				Console.WriteLine("bytes = null");
			}
			Console.WriteLine();
		}

		public static void WriteCharsForDebugging(string raw, string header)
		{
			if (!String.IsNullOrEmpty(header))
				Console.WriteLine(header);
			if (raw != null)
			{
				Console.WriteLine("raw.Length = {0}", raw.Length);
				for (int i = 0; i < raw.Length; ++i)
				{
					if (i > 0 && (i % 16) == 0)
						Console.WriteLine();
					Console.Write(" {0:x4}", (int)raw[i]);
				}
			}
			else
			{
				Console.WriteLine("raw = null");
			}
			Console.WriteLine();
		}
	}

}