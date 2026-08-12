using System.Reflection;
using System.Runtime.CompilerServices;
using System.Resources;
using System.Runtime.InteropServices;	// For type library.
using System;

// JohnT added the following on suggestion from Randy Regneir. Also the InteropServices using clause.
// This was in hopes of correcting a problem where a new GUID is generated for each build, also
// a problem where we get warnings on #import (on a clean build or where the tlb changed only)
// saying we need to import mscorlib.
[assembly: GuidAttribute("06F10557-EE67-4dad-B3F5-031568E4DB31")]	// Type library guid.

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Encoding Converters")]
[assembly: AssemblyDescription("Encoding Converters Repository and basic converter engine wrappers")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SIL")]
[assembly: AssemblyProduct("Encoding Converters")]
[assembly: AssemblyCopyright("Copyright © 2003-2021 SIL. All rights reserved.")]
[assembly: AssemblyTrademark("Copyright © 2003-2021 SIL. All rights reserved.")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguageAttribute("en")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// NOTE: if you change this value, then you have to update the function:
//  DirectableEncConverter.cs:DirectableEncConverterDeserializationBinder to 
//  support the new version number and allow for the old one when serializing in
//  the data.
[assembly: AssemblyVersion("4.0.0.0")]
[assembly: AssemblyFileVersionAttribute("4.1.0.0")]

[assembly: CLSCompliantAttribute(true)]

//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: AssemblyDelaySign(false)]
// done in project settings now (was causing a warning)
// [assembly: AssemblyKeyFile("..\\..\\..\\..\\..\\..\\src\\FieldWorks.snk")]
[assembly: AssemblyKeyName("")]

// grants TestEncCnvtrs access to this assembly's `internal` types (NativeModuleConflictDetector,
// PersistentEncConverterHostSession, SubprocessEncConverter -- see Handover.md, "Generic subprocess
// fallback for host-process conflicts") so they can be unit-tested directly rather than only through
// the full EncConverters/AddEx round trip. Because this assembly is strongly named (see
// AssemblyKeyName/project SignAssembly above), the friend assembly's public key must be given
// explicitly -- TestEncCnvtrs is signed with the same FieldWorks.snk key pair used throughout this
// solution (see its .csproj), and this is that key pair's public key (extracted via
// `sn -tp FieldWorks.snk`), not a new/different key.
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("TestEncCnvtrs, PublicKey=00240000048000009400000006020000002400005253413100040000010001005f4452c387d979e3cba05fd73bb9aebe8f8830874663d66a7869f614a8f5e8def658d5c5920fae609d28aa005d5a9af5bd758ca8f19ad0347b7aa76e1f723f8994792136f5ceff9fb6f719d4337f65da2e1d66a85cc5e28e4656a1a30c2ff513440393177625c725d3fb156dc3c11610ea5936b9404ab9d51f7eb71ac0aa27bd")]
