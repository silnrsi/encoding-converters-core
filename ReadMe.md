Encoding Converters Core

These are the main encoding converters used by Fieldworks and SIL Converters to transform language data.
The converters are mainly used to change character encoding.
Encoding converters in this package include IcuRegEx, IcuTransliterator, TecKit, Perl, Python, and CC

The NuGet package https://www.nuget.org/packages/Encoding-Converters-Core/ is built from this repository.

To build new packages:
1. Run a release build for both 32 and 64bit
2. Increase the version in Package.nuspec
3. Run 'NuGet.exe pack' from the root directory of the repository