powershell -ExecutionPolicy Unrestricted -Command "& {Import-Module .\psake.psm1; Invoke-psake .\build.ps1 nuget -properties @{ config='Release';assemblyFileVersion='1.0.?.7400';assemblyRevision='5' }} "
PAUSE