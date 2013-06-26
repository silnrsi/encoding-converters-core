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

cd to project root
./autogen.sh        # calls configure
make                # for testing and debugging
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
export MONO_REGISTRY_PATH=/var/lib/encConverters/registry
./ECFileConverter.exe /i in.txt /o out.txt /n askMe

Running like this was necessary on Fedora:
sudo mono ECFileConverter.exe /i in.txt /o out.txt /n askMe


## Running tests

nunit-console TestEncCnvtrs.dll
./RunTests.exe  # does the same thing but works in MonoDevelop

Instructions given by the program:
First, make sure that /var/lib/fieldworks exists and is writeable by everyone.
eg, sudo mkdir -p /var/lib/fieldworks && sudo chmod +wt /var/lib/fieldworks
Then add the following line to ~/.profile, logout, and login again.
MONO_REGISTRY_PATH=/var/lib/fieldworks/registry; export MONO_REGISTRY_PATH

Cleanup() may complain about the RESTOREME file at:
/var/lib/fieldworks/SIL/Repository
Fix this by renaming manually.

May need to copy this file to /output/Debug:
/usr/lib/mono/gac/nunit.framework/2.5.10.0__96d09a1eb7f44a77/nunit.framework.dll

NAnt runs NUnit tests conveniently, but csproj files don't do this without
installing extra libraries.


## Debugging

Make sure the project is getting built with the debug flag.
Then run it like this:
mono --debug --trace=N:TestEncCnvtrs,RunTests,SilEncConverters40,ECInterfaces ./RunTests.exe > out.txt

Does this set a breakpoint?
gdb mono
run --debug --break "RunTests:Main" ./RunTests.exe


#-------------------------------------------------------------------------------
# Paths and Libraries
#-------------------------------------------------------------------------------

## Paths

/usr/lib/encConverters is searched by OOoLT to find libecdriver.so
There seems to be a problem when the TECkit library is in a different
    place from the running SilEncConverters40.dll.
    Setting LD_LIBRARY_PATH makes this work, but ldconfig does not.
    Anyway in a production environment this shouldn't be a problem because
    they will be in the same directory.
Set MONO_PATH to load assemblies outside the current directory.
If there are several versions of mono, then it may be necessary to set the
    path so that the appropriate version is found.
    which mono

Some commands to find things:
    which
    whereis
    locate
    updatedb (no arguments)  # update database used by locate


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


## Registry

Mono uses folders and XML files to simulate the Windows registry.

#mkdir /var/lib/fieldworks
#chmod a+rwx /var/lib/fieldworks
sudo mkdir -p /var/lib/encConverters && sudo chmod +wt /var/lib/encConverters

There are several places where a registry structure can be stored.
    /etc/mono/registry              # HKCR - system wide shared settings
    ~/.mono/registry                # HKCU - user settings
    /var/lib/fieldworks/registry    # system wide fieldworks settings
    ~/.config/fieldworks/registry/  # user fieldworks settings
        chmod 777 /etc/mono/registry/ClassesRoot -R
    echo $MONO_REGISTRY_PATH

Two registry key folders:
    SOFTWARE\SIL\EncodingConverterRepository
        Created by EncConverters.WriteStorePath().
        Gets read to find the repository file when initializing the list.
    SOFTWARE\SIL\SilEncConverters40
        Gets created on Windows during installation by wxs script.
        Gets created on Linux during "sudo make install prefix=/usr"
        A message recommendeds to create it manually on developer machines.
        Some subkeys apparently get created by the Converters Installer.
        Gets read to open help file in AutoConfigDialog.cs.


## Mapping Repository

/var/lib/fieldworks/SIL/Repository/mappingRegistry.xml
rm *RESTOREME   # may be needed when running tests


## Viewing compiled files

file abc            # technical overview of what abc is
nm -g abc.so        # list symbols in abc.so
readelf -Ws abc.so  # output all symbols defined in abc.so
ldd abc.so          # list dependencies of abc.so

Assembly DLLs can be viewed in MonoDevelop with the Assembly browser.
Easier is to edit references and browse to a file.
When selected (not added), it will show the version info at the bottom.


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


## ldconfig

/usr/lib/encConverters
Create file /etc/ld.so.conf.d/encConverters.conf containing this text:
    /usr/lib/encConverters
Then sudo ldconfig
However none of this is currently needed.


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

Type load error
    in AutoConfigDialog.ctor when calling InitializeComponent().
Solution: WebBrowserAdaptor.cs requires Gecko DLLs (notice "using Gecko" line)
    even if there is no xulrunner folder.

If creating the converter fails (for example if TECkit is not found),
then it crashes because the name is null.

