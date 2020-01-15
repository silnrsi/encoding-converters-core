# Encoding Converters Core

See also ReadMe_linux.txt and ReadMe_windows.txt.

These are the main encoding converters used by Fieldworks and SIL Converters to transform language data.
The converters are mainly used to change character encoding.
Encoding converters in this package include IcuRegEx, IcuTransliterator, TecKit, Perl, Python, and CC

The NuGet package https://www.nuget.org/packages/Encoding-Converters-Core/ is built from this repository.

## Build Encoding Converters Core

In Ubuntu Linux,

### Setup

Add packages.sil.org repository by following instructions at https://packages.sil.org/ .

Install build dependencies.

    sudo apt-get install automake g++ python-dev libicu-dev mono-sil icu-dev-fw

### First build

    (. environ && ./autogen.sh && make)

### Subsequent builds

    (. environ && make)

### Test

    (. environ && mono output/Debug/RunTests.exe)

In Windows,

1. Open EncConverters 2015.sln in Visual Studio Community 2015.
* Choose Build > Build Solution.

## Build NuGet packages

To build new packages:
1. Run a release build for both 32 and 64bit
2. Increase the version in Package.nuspec
3. Run 'nuget.exe pack' from the root directory of the repository