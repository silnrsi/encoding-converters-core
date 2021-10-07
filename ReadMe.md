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

    sudo apt-get install automake g++ python-dev libicu-dev mono5-sil icu-dev-fw libteckit-dev

### First build

    (. environ && ./autogen.sh && make)

### Subsequent builds

    (. environ && make)

### Test

    # Optionally use non-global common dir
    export EC_COMMON_APPLICATION_DATA_PATH="$(pwd)/ec-common"
    export MONO_REGISTRY_PATH="${EC_COMMON_APPLICATION_DATA_PATH}/registry"

    # Copy RunTests.exe to directory with the other DLLs to be found.
    (cp -a src/RunTests/bin/x64/Debug/RunTests.exe output/x64/Debug/ &&
      . environ && mono output/x64/Debug/RunTests.exe)

In Windows,

1. Open EncConverters 2015.sln in Visual Studio Community 2015.
2. Choose Build > Build Solution.

## Build NuGet packages

To build new packages:
1. Run a release build for both 32 and 64bit
2. Increase the version in Package.nuspec
3. Run 'nuget.exe pack' from the root directory of the repository