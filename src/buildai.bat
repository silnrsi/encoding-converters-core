devenv C:\src\EC\src\AIGuesserEC\AIGuesserEC.csproj /rebuild Debug
devenv C:\src\EC\src\AIGuesserEC\AIGuesserEC.csproj /rebuild Release
; devenv C:\src\ww\Lib\src\EC\Installer\AIGuesserMM\AIGuesserMM.vdproj /rebuild Release
copy C:\src\EC\output\release\AIGuesserEC.dll C:\src\StoryEditor\DepDLLs
copy C:\src\EC\output\release\AIGuesserEC.tlb C:\src\StoryEditor\DepDLLs
; copy C:\src\WW\Installer\ExternalEncodingConverters\AIGuesserMM.msm C:\src\StoryEditor\DepDLLs
copy C:\src\EC\output\release\AIGuesserEC.dll C:\src\StoryEditor\output\debug
copy C:\src\EC\output\release\AIGuesserEC.dll C:\src\StoryEditor\output\release
copy C:\src\EC\output\release\AIGuesserEC.dll "C:\src\StoryEditor\OneStory Releases\OSE2.7\client"
