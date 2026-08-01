# EncConverter Modernization Guide

## Build & Test Commands
- Build x86 Solution: `msbuild EncConverters.sln /p:Configuration=Debug /p:Platform=x86`
- Build x64 Solution: `msbuild EncConverters.sln /p:Configuration=Debug /p:Platform=x64`
- Restore NuGet Packages: `nuget restore EncConverters.sln` or `dotnet restore` (SDK projects only)

## Architecture Constraints
- **Strict Bit-Width Separation:** The solution *cannot* be AnyCPU. Native transducer DLLs (CC, TECkit, ICU) require distinct x86 and x64 build configurations.
- **Project Styles:** 
  - C# projects must be converted to multi-targeting SDK-style formats.
  - C++ (`.vcxproj`) projects must remain standard MSBuild C++ structures but configured to support target platforms cleanly.
- **Target Framework Matrix:**
  - Support legacy boundaries: `.NET Framework 4.6.2` and `.NET Framework 4.8`
  - Cross-platform & Modern boundaries: `.NET Standard 2.0` (where UI-free) and `.NET 8.0 / .NET 10.0` (using modern UI abstractions).
- **Target Namespace:** Transition legacy namespaces toward `SIL.Transduction`.

## UI Requirements
- Deprecate Internet Explorer engine and the obsolete GeckoFX package.
- Move configuration interfaces toward a unified cross-platform browser engine interface or native modern UI abstraction.
