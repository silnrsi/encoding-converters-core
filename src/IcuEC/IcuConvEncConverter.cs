// Created by Steve McConnel Feb 2, 2012 by copying and editing IcuTranslitEncConverter.cs

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ECInterfaces;                     // for IEncConverter

namespace SilEncConverters40
{
	/// <summary>
	/// Managed ICU Converter EncConverter
	/// </summary>
	public class IcuConvEncConverter : EncConverter
	{
		#region DLLImport Statements
		// On Linux looks for libIcuConvEC.so (adds lib- and -.so)
		[DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_Initialize", CallingConvention = CallingConvention.Cdecl)]
		static extern int CppInitialize (
			[MarshalAs(UnmanagedType.LPStr)] string strConverterSpec);

		[DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_PreConvert", CallingConvention = CallingConvention.Cdecl)]
		static extern int CppPreconvert(
			int eInEncodingForm, ref int eInFormEngine,
			int eOutEncodingForm, ref int eOutFormEngine,
			ref int eNormalizeOutput, bool bForward, int nInactivityWarningTimeOut);

		[DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_DoConvert", CallingConvention = CallingConvention.Cdecl)]
		static extern unsafe int CppDoConvert(
			byte* lpInputBuffer, int nInBufLen,
			byte* lpOutputBuffer, int *npOutBufLen);

		[DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_ConverterNameList_start", CallingConvention = CallingConvention.Cdecl)]
		static extern int CppConverterNameList_start();

        static string CppConverterNameList_next()
        {
            if (Util.IsUnix)
            {
                return CppConverterNameList_next_Linux();
            }
            else
            {
                return CppConverterNameList_next_Windows();
            }
        }

        static string CppGetDisplayName(string strID)
        {
            if (Util.IsUnix)
            { return CppGetDisplayName_Linux(strID); }
            else
            {
                return CppGetDisplayName_Windows(strID);
            }
        }

        [DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
		[return : MarshalAs(UnmanagedType.LPStr)]
		static extern string CppConverterNameList_next_Linux();

		[DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		static extern string CppGetDisplayName_Linux(
			[MarshalAs(UnmanagedType.LPStr)] string strID);

        [DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_ConverterNameList_next", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppConverterNameList_next_Windows();

        [DllImport("IcuConvEC.dll", EntryPoint = "IcuConvEC_GetDisplayName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string CppGetDisplayName_Windows([MarshalAs(UnmanagedType.LPStr)] string strID);

		#endregion DLLImport Statements

		#region Member Variable Definitions
		public const string strDisplayName = "ICU Converter";
		public const string strHtmlFilename = "ICU Converters Plug-in About box.htm";
		#endregion Member Variable Definitions

		#region Initialization
		/// <summary>
		/// The class constructor. </summary>
		public IcuConvEncConverter()
			: base (
				typeof(IcuConvEncConverter).FullName,
				EncConverters.strTypeSILicuConv)
		{
		}

		/// <summary>
		/// The class destructor. </summary>
		~IcuConvEncConverter()
		{
			Unload();
		}

		public override void Initialize(
			string converterName,
			string converterSpec,
			ref string lhsEncodingID,
			ref string rhsEncodingID,
			ref ConvType conversionType,
			ref Int32 processTypeFlags,
			Int32 codePageInput,
			Int32 codePageOutput,
			bool bAdding)
		{
			Util.DebugWriteLine(this, "BEGIN");
			// let the base class have first stab at it
			base.Initialize(converterName, converterSpec, ref lhsEncodingID, ref rhsEncodingID,
				ref conversionType, ref processTypeFlags, codePageInput, codePageOutput,
				bAdding);

			// the only thing we want to add (now that the convType can be less than accurate)
			//  is to make sure it's unidirectional
			switch (conversionType)
			{
				case ConvType.Legacy_to_from_Legacy:
					conversionType = ConvType.Legacy_to_Legacy;
					break;
				case ConvType.Legacy_to_from_Unicode:
					conversionType = ConvType.Legacy_to_Unicode;
					break;
				case ConvType.Unicode_to_from_Legacy:
					conversionType = ConvType.Unicode_to_Legacy;
					break;
				case ConvType.Unicode_to_from_Unicode:
				    conversionType = ConvType.Unicode_to_Unicode;
				    break;
				default:
					break;
			}
			Util.DebugWriteLine(this, "END");
		}
		#endregion Initialization

		#region Misc helpers
		protected bool IsFileLoaded()
		{
			return false;
		}

		protected void Unload()
		{
		}

		protected override EncodingForm DefaultUnicodeEncForm(bool bForward, bool bLHS)
		{
			// if it's unspecified, then we want UTF-16
			return EncodingForm.UTF16;
		}

		protected unsafe void Load(string strConvID)
		{
			Util.DebugWriteLine(this, "BEGIN");
			Util.DebugWriteLine(this, "Calling CppInitialize");
			int status = 0;

            status = CppInitialize(strConvID);
			if (status != 0)
			{
				throw new Exception("CppInitialize failed.");
			}
			Util.DebugWriteLine(this, "END");
		}
		#endregion Misc helpers

		#region Abstract Base Class Overrides
		protected override unsafe void PreConvert(
			EncodingForm       eInEncodingForm,
			ref EncodingForm   eInFormEngine,
			EncodingForm       eOutEncodingForm,
			ref EncodingForm   eOutFormEngine,
			ref NormalizeFlags eNormalizeOutput,
			bool               bForward)
		{
			// let the base class do it's thing first
			base.PreConvert(eInEncodingForm, ref eInFormEngine,
				eOutEncodingForm, ref eOutFormEngine,
				ref eNormalizeOutput, bForward);

			if (NormalizeLhsConversionType(ConversionType) == NormConversionType.eUnicode)
            {
                if (Util.IsUnix)
                {
                    // returning this value will cause the input Unicode data (of any form,
                    // UTF16, BE, etc.) to be converted to UTF8 narrow bytes before calling
                    // DoConvert.
                    eInFormEngine = EncodingForm.UTF8Bytes;
                }
                else
                {
                    eInFormEngine = EncodingForm.UTF16;
                }
            }
            else
            {
                // legacy
                eInFormEngine = EncodingForm.LegacyBytes;
            }

            if (NormalizeRhsConversionType(ConversionType) == NormConversionType.eUnicode)
            {
                if (Util.IsUnix)
                {
                    eOutFormEngine = EncodingForm.UTF8Bytes;
                }
                else
                {
                    eOutFormEngine = EncodingForm.UTF16;
                }
            }
			else
			{
				eOutFormEngine = EncodingForm.LegacyBytes;
			}

			// do the load at this point.
			Load(ConverterIdentifier);
			
			// Finally, let the C++ code do its thing.
			int encInForm = (int)eInEncodingForm;
			int encInEngine = (int)eInFormEngine;
			int encOutForm = (int)eOutEncodingForm;
			int encOutEngine = (int)eOutFormEngine;
			int normOutput = (int)eNormalizeOutput;
			CppPreconvert(encInForm, ref encInEngine, encOutForm, ref encOutEngine,
				ref normOutput, bForward, 0);
			eInFormEngine = (EncodingForm)encInEngine;
			eOutFormEngine = (EncodingForm)encOutEngine;
			eNormalizeOutput = (NormalizeFlags)normOutput;
		}

		protected override unsafe void DoConvert(
			byte*   lpInBuffer,
			int     nInLen,
			byte*   lpOutBuffer,
			ref int rnOutLen)
		{
			Util.DebugWriteLine(this, "BEGIN");
			int status = 0;
			fixed(int* pnOut = &rnOutLen)
			{
				status = CppDoConvert(lpInBuffer, nInLen, lpOutBuffer, pnOut);
			}
			if (status != 0)
			{
				EncConverters.ThrowError(ErrStatus.Exception, "CppDoConvert() failed.");
			}
            Util.DebugWriteLine(this, "END");
		}

		protected override string GetConfigTypeName
		{
			get { return typeof(IcuConvEncConverterConfig).AssemblyQualifiedName; }
		}

		#endregion Abstract Base Class Overrides
		
		#region Additional public methods to access the C++ DLL.
		/// <summary>
		/// Gets the available ICU converter specifications.
		/// </summary>
		public static List<string> GetAvailableConverterSpecs()
		{
			var count = CppConverterNameList_start();
			var specs = new List<string>(count);
			for (var i = 0; i < count; ++i)
			{
				var name = CppConverterNameList_next();
				specs.Add(name);
			}
			return specs;
		}

	    protected override void GetConverterNameEnum(out string [] rSa)
        {
            rSa = GetAvailableConverterSpecs().ToArray();
        }

		/// <summary>
		/// Gets the display name of the given ICU converter specification.
		/// In practice, the output may be the same as the input.
		/// </summary>
		public static string GetDisplayName(string spec)
		{
			try
			{
			var name = CppGetDisplayName(spec);
			return name;
			}
			catch (Exception e)
			{
				// ReSharper disable LocalizableElement
				Console.WriteLine("Exception caught: message = '{0}'", e.Message);
				Console.WriteLine("Exception stack:\n{0}", e.StackTrace);
				// ReSharper restore LocalizableElement
				throw;
			}
		}
		#endregion
	}
}
