param($installPath, $toolsPath, $package)

# find out where to put the files, we're going to create a deploy directory
# at the same level as the solution.

$rootDir = (Get-Item $installPath).parent.parent.fullname
$deployTarget = "$rootDir\EcDistFiles\"

# create our deploy support directory if it doesn't exist yet

$deploySource = join-path $installPath 'runtimes/EcDistFiles'

if (!(test-path $deployTarget)) {
    mkdir $deployTarget
}

# copy everything in there

Copy-Item "$deploySource/*" $deployTarget -Recurse -Force

# the following would add all the EcDistFiles to the VS project in a solution level folder (named EcDistFiles)
# but it's not working, presumably, because it wasn't written expecting multiple levels. It could probably
# be made to work with recursion, but I'm not sure we even need these bits in the VS solution, so ... commenting out
# get the active solution
# $solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])

# create a deploy solution folder if it doesn't exist

# $deployFolder = $solution.Projects | where-object { $_.ProjectName -eq "EcDistFiles" } | select -first 1

# if(!$deployFolder) {
#     $deployFolder = $solution.AddSolutionFolder("EcDistFiles")
# }

# add all our support deploy scripts to our Support solution folder

# $folderItems = Get-Interface $deployFolder.ProjectItems ([EnvDTE.ProjectItems])

# ls $deployTarget | foreach-object {
# 	Write-Output $_.FullName
#     $folderItems.AddFromFile($_.FullName) > $null
# } > $null