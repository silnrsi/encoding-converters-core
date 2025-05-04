param(
    # Solution directory path. Often passed by MSBuild as $(SolutionDir)
    [string]$SolutionDir = "C:\Users\pete_\source\repos\encoding-converters-core", #silconverters",

    # Relative path to the NuGet packages folder from SolutionDir
    [string]$PackagesDirRelative = "packages",

    # Relative path to the root output folder from SolutionDir
    [string]$OutputDirRootRelative = "output",

    # Configurations to check
    [string[]]$Configurations = @("Debug", "Release"),

    # Platforms to check
    [string[]]$Platforms = @("x86", "x64"),

    # Path to the JSON config file, relative to SolutionDir
    [string]$DllMapConfigFile = "build\dll_versions.json"
)

# --- Script Setup ---
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop" # Exit script on first error unless caught
$ProgressPreference = 'SilentlyContinue' # Suppress progress bars for cleaner logs

# --- DLL path configuration ---
$DllMapConfigPath = Join-Path -Path $SolutionDir -ChildPath $DllMapConfigFile
if (-not (Test-Path $DllMapConfigPath)) {
    Write-Error "DLL Version Map config file not found at '$DllMapConfigPath'"
    exit 1
}
$DllVersionMap = Get-Content -Raw -Path $DllMapConfigPath | ConvertFrom-Json -AsHashtable

$PackagesDir = Join-Path -Path $SolutionDir -ChildPath $PackagesDirRelative
$OutputDirRoot = Join-Path -Path $SolutionDir -ChildPath $OutputDirRootRelative
$Script:ErrorOccurred = $false # Use Script scope to modify from functions

# --- Helper Functions ---

# Safely gets the Assembly Version from a DLL path
function Get-SafeAssemblyVersion {
    param([string]$DllPath)
    try {
        if (Test-Path -Path $DllPath -PathType Leaf) {
            $assemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($DllPath)
            return $assemblyName.Version
        } else {
            return $null # File doesn't exist
        }
    } catch {
        Write-Warning "Could not get AssemblyName for '$DllPath'. Error: $($_.Exception.Message)"
        return $null # Error reading assembly
    }
}

# Writes an error message in a format MSBuild/VS might recognize
function Write-BuildError {
    param([string]$Message)
    Write-Error $Message # Writes to stderr, usually picked up by build systems
    $Script:ErrorOccurred = $true
}

# --- Main Logic ---

# 1. Pre-calculate Expected Versions from the Packages folder
Write-Host "Pre-calculating expected DLL versions from '$PackagesDir'..."
$ExpectedVersions = @{}
foreach ($dllName in $DllVersionMap.Keys) {
    $relativeDllPath = $DllVersionMap[$dllName]
    $canonicalDllPath = Join-Path -Path $PackagesDir -ChildPath $relativeDllPath
    $version = Get-SafeAssemblyVersion -DllPath $canonicalDllPath
    if ($version) {
        $ExpectedVersions[$dllName] = $version
        Write-Host "  - $dllName : Expected Version $($version.ToString()) (from $canonicalDllPath)"
    } else {
        Write-BuildError "FATAL: Could not determine expected version for '$dllName' from '$canonicalDllPath'. Please check the DllVersionMap and ensure the file exists and is valid."
        # Exit early if a canonical DLL can't be read - fundamental problem
        exit 1
    }
}

# 2. Check DLL versions in Output Folders
Write-Host "Checking DLL versions in output directories..."
foreach ($config in $Configurations) {
    foreach ($platform in $Platforms) {
        $currentOutputDir = Join-Path -Path $OutputDirRoot -ChildPath $platform | Join-Path -ChildPath $config
        Write-Host "  Checking Output: '$currentOutputDir'"

        if (-not (Test-Path -Path $currentOutputDir -PathType Container)) {
            Write-Host "    Skipping: Output directory does not exist."
            continue
        }

        foreach ($dllName in $ExpectedVersions.Keys) {
            $expectedVersion = $ExpectedVersions[$dllName]
            $outputDllPath = Join-Path -Path $currentOutputDir -ChildPath $dllName

            $actualVersion = Get-SafeAssemblyVersion -DllPath $outputDllPath

            if ($actualVersion -eq $null) {
                # DLL not found in output - this might be okay or not, depending on requirements.
                # For this script's purpose (checking version *if present*), we just note it.
                # Write-Host "    - $dllName : Not found in output (optional)."
                continue # Skip version comparison if file isn't there
            }

            # Compare versions
            if ($actualVersion -ne $expectedVersion) {
                Write-BuildError "Version Mismatch in '$outputDllPath': Expected '$($expectedVersion.ToString())' but found '$($actualVersion.ToString())'."
            } else {
                # Write-Host "    - $dllName : Version OK ($($actualVersion.ToString()))" # Optional success message
            }
        }
    }
}

# 3. Check Binding Redirects in app.config files
Write-Host "Checking binding redirects in app.config files..."
# Search for app.config files, excluding common non-source folders
$appConfigFiles = Get-ChildItem -Path "$SolutionDir\src" -Filter app.config -Recurse -ErrorAction SilentlyContinue | Where-Object {
    $_.FullName -notmatch '\\(packages|output|bin|obj|\\.vs|\\.git|node_modules)\\'
}

if ($appConfigFiles.Count -eq 0) {
    Write-Host "  No app.config files found to check (excluding common build/tool folders)."
}

foreach ($configFile in $appConfigFiles) {
    Write-Host "  Checking Config: '$($configFile.FullName)'"
    try {
        [xml]$xmlContent = Get-Content -Path $configFile.FullName -Raw
        # Define the namespace manager if needed (often required for assemblyBinding)
        $nsMgr = New-Object System.Xml.XmlNamespaceManager($xmlContent.NameTable)
        $nsMgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1") # Standard assembly binding namespace

        foreach ($dllName in $ExpectedVersions.Keys) {
            $expectedVersion = $ExpectedVersions[$dllName]
            $expectedVersionString = $expectedVersion.ToString()
            # Assembly name in config usually doesn't have .dll
            $assemblyIdentityName = [System.IO.Path]::GetFileNameWithoutExtension($dllName)

            # XPath to find the specific bindingRedirect
            $xpath = "/configuration/runtime/asm:assemblyBinding/asm:dependentAssembly[asm:assemblyIdentity/@name='$assemblyIdentityName']/asm:bindingRedirect"
            $bindingRedirectNode = $xmlContent.SelectSingleNode($xpath, $nsMgr)

            if ($bindingRedirectNode) {
                $newVersionString = $bindingRedirectNode.GetAttribute("newVersion")
                if ($newVersionString -ne $expectedVersionString) {
                    Write-BuildError "Binding Redirect Mismatch in '$($configFile.FullName)' for '$assemblyIdentityName': Expected newVersion='$expectedVersionString' but found '$newVersionString'."
                } else {
                    # Write-Host "    - $assemblyIdentityName : Binding Redirect OK ($newVersionString)" # Optional success message
                }

                # Optional: Check oldVersion format too?
                $oldVersionString = $bindingRedirectNode.GetAttribute("oldVersion")
                $expectedOldVersionPattern = "0.0.0.0-$expectedVersionString"
                if ($oldVersionString -ne $expectedOldVersionPattern) {
                   Write-BuildError "Binding Redirect Format Warning in '$($configFile.FullName)' for '$assemblyIdentityName': Expected oldVersion='$expectedOldVersionPattern' but found '$oldVersionString'."
                }
            } else {
                # No binding redirect found for this assembly in this config file. This is usually fine.
                Write-Host "    - $assemblyIdentityName : No binding redirect found."
            }
        }
    } catch {
        Write-BuildError "Failed to process '$($configFile.FullName)'. Error: $($_.Exception.Message)"
    }
}

# --- Final Result ---
Write-Host "Verification complete."
if ($Script:ErrorOccurred) {
    Write-Host "Errors were detected during verification."
    exit 1 # Signal failure to the build process
} else {
    Write-Host "All checked DLL versions and binding redirects are consistent."
    exit 0 # Signal success
}

