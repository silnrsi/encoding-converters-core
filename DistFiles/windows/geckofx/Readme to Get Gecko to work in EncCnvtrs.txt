The DLLs in this folder (i.e. geckofx-core-18.dll and Geckofx-Winforms-18.dll) are now part of the compile-time requirement for EncConverters core and they should be redistributed with whatever product uses it.

For a Windows release, however, the code is set to default to using the WebBrowser control, since it releases us from having to redistribute all the files in the xulRunner (Mozilla) folder as well. AND it allows us to be able to use simple 'mht' files for the help (those are IE-specific help files that enclose the entire page -- text and images in a single archive file with a *.mht extension. Firefox can't read these.

So to build the EncCnvrs project on/for Linux, you need to download and unzip the xulRunner stuff from http://ftp.mozilla.org/pub/mozilla.org/xulrunner/releases/18.0/runtimes/ and unzip it so that xulRunner becomes a sub-folder of <SolutionDir>/lib/xulrunner. On installation, the xulRunner folder should become a sub-folder of the install folder (where the SilEncConverters40.dll is installed).

Then, don't forget to edit the *.mht files in IE (or Word) and re-save them as *.htm (or whatever extension Mozilla supports).
