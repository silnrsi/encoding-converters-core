ICU4NET 0.0.3
-------------

HOW TO USE
-------------
1. Add references to ICU4NET.dll and ICU4NETExtension.dll.

2. Make sure that ICU4C's DLLs, including icudt42.dll, icuin42.dll, icuio42.dll, icule42.dll, iculx42.dll, icutu41.dll, and icuuc42.dll, are in the %PATH% or in the working directory of your exe program.

DEPLOYMENT ON TARGET MACHINE
-------------
You may face problem related to DLL dependency when using program built with ICU4NET on machine without these prerequisites:

1. Make sure .NET Framework 3.5 is installed on target machine - 
http://www.microsoft.com/downloads/details.aspx?FamilyId=333325fd-ae52-4e35-b531-508d977d32a6&displaylang=en

2. Make sure Visual C++ 2008 Run-time is installed on target machine (required by ICU4C) -
 http://www.microsoft.com/downloads/details.aspx?FamilyID=9b2da534-3e03-4391-8a4d-074b9f2bc1bf&displaylang=en

DEMO
-------------
Run ThaiWordBreak.exe for demo!