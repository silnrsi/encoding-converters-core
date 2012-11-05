This file is only the beginnings of a ReadMe. This initial version was created by John Thomson to document the
things I've discovered that I wished someone had documented for me. I am not the author of any of this code.
I may have some of this wrong. Please add to it or correct it as you are able.

Building EncodingConverters: Windows:
- open the main solution, EC/EncConverters 2010.sln if you are using Visual Studio 2010.
- build solution.

In my experience this succeeds in a release build (sometimes after a few tries or restarting VS). It generally builds
all the projects except the AIGuesserEC in a debug build.

It is helpful to install python 2.7 (e.g., from here: http://www.python.org/getit/, choose the Python 2.7.3 Windows Installer).
I think this was necessary even to build the Python EncConverter, but it is certainly necessary in order to use it or run its tests (perhaps to run any tests).

Running unit tests: Windows:

ICU needs to be able to find its EC/Plugins directory (e.g., EC/output/release/EC/Plugins). Depending on exactly how the test runner configures
things, it may automatically find them; by default it expects to find EC/Plugins starting from the folder where the EXE lives. If this fails,
you can set a registry key, HKLM/SOFTWARE/SIL/SilEncConverters40, and set the value of DeveloperPluginDir to the appropriate folder.

As noted above, to run the Python unit test requires installing Python. I found version 2.7.3 worked; I didn't try others.
I did not get the Perl unit test to run. Probably it just requires installing some version of Perl.

I had some trouble with three tests that involve IcuTransliterators. I found that these tests
- always pass in a debug build
- always pass in a Release build using Nunit-86 2.6.0.12124
- usually fail in a Release build using Resharper's test runner (set to use NUnit 2.6)
- sometimes one passes when run in isolation
- sometimes one passes when breakpoints are set in various places
- There seems to be some dependency between tests, failures are much more likely when these tests are run in a group, or following another test
(expecially from the TestIcuConvEncConverter group).

Linux versus Windows:
You should be aware that (as of November 2012) things seem to be a bit unstable. Bob Eaton recently did an incomplete version of upgrading to
ICU version 48. Jim Kornelsen recently replaced the C# to C++ linkage using ATL with new wrappers, but this work seems to be
incompletely tested (and maybe incompletely done) on Windows. The current solution is set up to use Jim's work. I think at least one module
(EncCnvtrs 2010?) may be obsolete.

ICU version:
Note that the version number in the EncConverters DLLs (e.g., SilEncConverters40.dll) refers to the version of the EncConverters interface.
The version number in the various ICU DLLs (e.g., icuin48.dll) refers to the version of ICU.
Confusion is easy especially because for several years they were exactly in sync, SilEncConverters40.dll shipping with version 4.0 of ICU.
Now however we are still at version 4.0 of the Enc Converters interfaces, but are up to 48 (soon to be 50) of ICU.

Upgrading ICU to a new version
- obtain or build the appropriate icudtVV.dll, icuinVV.dll, and icuucVV.dll modules from icu/bin in the ICU download package and put them in ICU/DistFiles/icu/bin
- get the appropriate header files (from the ICU download package include/unicode folder) and put them in EC/include/unicode.
Remove any obsolete ones. (Note that we probably don't need them all. But it does little harm to have them.)
- copy all the files from the download package icu/lib folder to EC/lib/windows (some are probably not needed...)
- modify the icuTranslitEC.vcsproj, IcuConvEC.vcsproj, IcuRegexEC.vcsproj to copy the appropriate DLLs to the output. In Build Events/Post Build Event, change the version
numbers of the DLLs. Alsoin IcuEC 2010 (if this is not obsolete) similarly modify the post-build event. Unfortunately this needs to be
done in both debug and release versions.
- see what breaks and fix it.
