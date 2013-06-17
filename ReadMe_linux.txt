# vim: set filetype=cfg : 
################################################################################
#
# ReadMe_linux.txt
#
# A collection of notes about building and running EncConverters on Linux. 
#
# Created May 17 2013 by Jim Kornelsen
#
################################################################################

Contents:
    Building
    Running
    Paths and Libraries
    Solutions to common error messages

#-------------------------------------------------------------------------------
# Building
#-------------------------------------------------------------------------------

## To build using makefiles.

from project root, sudo ./autogen.sh        # calls configure
make                                        # for testing
make release
sudo make install prefix=/usr
http://www.gnu.org/prep/standards/html_node/Directory-Variables.html

## xbuild

Generate msbuild from VStudio and port to Mono xbuild.
http://www.mono-project.com/Microsoft.Build

## Mono

sudo apt-get install mono-complete   # includes xbuild and mono-2.0-dev

## MonoDevelop

It often says a project file will not be built in active configuration.
I don't know of a conveninent solution to this, but it's not too hard to fix
either. Just remove and then add each project file.
Editing the solution file itself is possible but a little tricky.
Lines containing .Build.0 are needed for every project file.

## compile a single file with Mono

gmcs TestEncConverters.cs /r:../../output/Debug/ECInterfaces.dll /r:../../output/Debug/SilEncConverters40.dll
gmcs ECFileConverter.cs -r:bin/x86/Debug/ECInterfaces.dll -r:bin/x86/Debug/SilEncConverters40.dll

#-------------------------------------------------------------------------------
# Running
#-------------------------------------------------------------------------------

cd output/Debug
./ECFileConverter.exe /i in.txt /o out.txt /n askMe

## Running tests

./RunTests.exe
or use sudo for possibly better results.

Instructions given by the program:
First, make sure that /var/lib/fieldworks exists and is writeable by everyone.
eg, sudo mkdir -p /var/lib/fieldworks && sudo chmod +wt /var/lib/fieldworks
Then add the following line to ~/.profile, logout, and login again.
MONO_REGISTRY_PATH=/var/lib/fieldworks/registry; export MONO_REGISTRY_PATH

Cleanup() often complains about the RESTOREME file at:
/var/lib/fieldworks/SIL/Repository
Fix this by renaming manually.

## Debugging

    Make sure the project is getting built with the debug flag.
    Then run it like this:
    mono --debug --trace=N:TestEncCnvtrs,RunTests,SilEncConverters40,ECInterfaces ./RunTests.exe > out.txt

    Does this set a breakpoint?
    gdb mono
    run --debug --break "RunTests:Main" ./RunTests.exe

## Root Privileges

    in shell script, gksu mono ECFileConverter.exe

#-------------------------------------------------------------------------------
# Paths and Libraries
#-------------------------------------------------------------------------------

## Paths

/usr/lib/encConverters has to be set with LD_LIBRARY_PATH in order for
    OOoLT to find ECDriver.
    To do this, add a file to ld.so.conf.d with the path.
    Or we could specify the path in OOoLT.
There seems to be a problem when the TECkit library is in a different
    place from the running SilEncConverters40.dll.
    Setting LD_LIBRARY_PATH makes this work, but ldconfig does not.
    Anyway in a production environment this shouldn't be a problem because
    they will be in the same directory.
Set MONO_PATH to load assemblies outside the current directory.

locate
updatedb

## ICU

    sudo apt-get install libicu50   # probably already done by default
    sudo apt-get install libicu-dev
    installs header files at /usr/include/unicode
    library files are in /usr/lib or /user/lib/i386-linux-gnu
    in /usr/lib/fieldworks:
        libicuuc.so
            contains ucnv_openPackage used by IcuConverter.cpp
        libicui18n.so
            contains uregex_replaceAll used by IcuRegexEncConverter.cpp
    readelf -Ws          # look for numbers in the 7th column
    ldd libIcuConvEC.so  # view dependences and any problems
    A possibly easier way than adding -licuuc in makefiles:
    icu-config --ldflags
    Note: On the ICU website it seems to suggest downloading and building
    manually. When I did this, make put files in various places under
    /usr/local, which makes sense, but these overrode files elsewhere.
    Anyway what I really wanted to do was probably apt-get install libicu-dev.

## ldconfig

/usr/lib/encConverters
    Create file /etc/ld.so.conf.d/encConverters.conf containing this text:
        /usr/lib/encConverters
    Then sudo ldconfig

## NUnit

    Copy this file to /output/Debug:
    /usr/lib/mono/gac/nunit.framework/2.5.10.0__96d09a1eb7f44a77/nunit.framework.dll 
    To run tests directly:
        cd output/Debug
        nunit-console TestEncCnvtrs.dll
    NAnt runs NUnit tests conveniently, but csproj files don't do this without
    installing extra libraries.

Assembly DLLs can be viewed in MonoDevelop with the Assembly browser.
Easier is to edit references and browse to a file.
When selected (not added), it will show the version info at the bottom.

## Registry

    There are several places where a registry structure is stored.
    echo $MONO_REGISTRY_PATH
    /etc/mono/registry              # HKCR - system wide shared settings
    ~/.mono/registry                # HKCU - user settings
    # fieldworks settings
    /var/lib/fieldworks/registry
    ~/.config/fieldworks/registry/
    chmod 777 /etc/mono/registry/ClassesRoot -R

## Repository

    /var/lib/fieldworks/SIL/Repository/mappingRegistry.xml
    rm *RESTOREME   # may be needed when running tests

## Gecko

Suspicion of the main problem: Which version of mono?
We need to build and run with a version of mono that has the correct libs.
- Does the fieldworks version?
- What about mono-2.0 libs?

System.TypeInitializationException: An exception was thrown by the type
initializer for Gtk.Application ---> System.EntryPointNotFoundException:
glibsharp_g_thread_supported

Download:
    geckofx 14 source code
    xulrunner libraries
    xulrunner sdk (just for header files)

Need to do on Linux before initializing xulrunner:
LD_LIBRARY_PATH=/media/winD/Jim/computing/SEC_on_linux/ec-main/lib/xulrunner
LD_PRELOAD=../../DistFiles/linux/geckofix.so

Fixes for building GeckoFx 14:
    In Geckofx-Core/Linux/Makefile, set -I to dir containing HashFunctions.h,
        and use g++ instead of gcc.
    In Geckofx-Core/Xpcom.cs:
        comment out 2 lines with AddRef
    In Makefile:
        change LD_PRELOAD to simply ./geckofix.so
        Add under clean:
        cd Geckofx-Core/Linux && make clean
    In GeckoFxTest/GeckoFxTest.sh:
        Instead of /usr/lib/firefox/, change LD_LIBRARY_PATH to
        xulrunner path in PutXulRunnerFolderHere directory.
        Change MONO_PREFIX to /usr
    In all .csproj files:
        <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
Additional fixes for building GeckoFx 18:
    In Geckofx-Core/DOM, rename one file xxDOMyy to xxDomyy.
    In solution file, delete two WPF projects (not supported on Mono).
    In unit test class, add "ref" as the compiler requests.

To build Skybound.Gecko.dll, in MonoDevelop:
    go to Solution tab
    right-click on Test project
    Options-> Run -> General
    change Environment Variable LD_LIBRARY_PATH to .17 instead of .13

## Fieldworks paths

/usr/lib/fieldworks/mono/lib/mono/2.0/gmcs.exe
/usr/lib/fieldworks/mono/bin/mono

/home/jkornelsen/p4repo/Calgary/FW_7.0/Bin/src/MonoRegistryTools/WriteKey
/home/jkornelsen/p4repo/Calgary/FW_7.0/Src/Common/FieldWorks/FieldWorks.cs
    main entry point

/usr/share/fieldworks/setup-user

## Embedding Mono

http://stackoverflow.com/questions/3672532/use-mono-net-library-in-linux

#-------------------------------------------------------------------------------
# Solutions to common error messages
#-------------------------------------------------------------------------------

Runtime error: undefined symbol: sem_init
Solution: link with -lpthread (the second g++ step).

ERROR: ld.so: object '../../../Geckofx-Core/Linux/geckofix.so' from
    LD_PRELOAD cannot be preloaded: ignored.
    /bin/sh: symbol lookup error: ./geckofix.so:
    undefined symbol: __gxx_personality_v0
Solution: build with g++ instead of gcc

usr/bin/cli: symbol lookup error: libIcuEC.so: undefined symbol: ucnv_open_44
Solution: link with -licuuc (actual solution was to fix libicu48 package by
          removing libicu44 items from /usr/local).
          Two things to check:
          - is the correct library found at link time
          - is the correct library found at runtime

Missing method
System.Reflection.MethodInfo::op_Inequality(MethodInfo,MethodInfo)
in assembly /usr/lib/mono/2.0/mscorlib.dll,
referenced in assembly
/media/winD/Jim/computing/SEC_on_linux/ec-main/output/Debug/nunit.framework.dll
Solution: build this assembly with mono 4.0 libraries instead of 2.0
    (details: It was using wrong csproj file, the one that still had
     <TargetFrameworkVersion>v3.5</TargetFrameworkVersion>)
    Use dmcs instead of gmcs.
    Thing to check:
    - are we building each assembly to the expected version of mono
    - are we running with the expected version of mono

If creating the converter fails (for example if TECkit is not found),
then it crashes because the name is null.

