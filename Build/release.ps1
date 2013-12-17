############################################################################################
#
#    Version information for an assembly consists of the following four values:
#       Major.Minor.Stage.Build
#
#	 Stage Numbers
#		Alpha	- 7100
#		Beta	- 7200
#		RC		- 7300
#		GA    - 7400
#		HF		- 7500
#		SP		- 7600
#
#
############################################################################################
#
#    $assemblyInformationalVersion
#	     The version of the product which is released
#        [assembly: AssemblyInformationalVersion("1.0.Alpha")]
#
#    $versionAssemblyFile
#	     Describes file version of the assembly. If you increment Major Version you probably want to change the AssemblyVersion as well
#        [assembly: AssemblyFileVersion("1.0.7100.0")]
#
#    $versionAssembly
#	     Careful! The CLR uses this version when loading assemblies. Change this only when you introduce breaking changes.
#        [assembly: AssemblyVersion("1.0.0.0")]
#
############################################################################################

properties {
	$config='stage' #release or stage

	$assemblyInformationalVersion = "LaCore.Hyperion-1.0.GA"
	$assemblyFileVersion = "1.0.7400.?"
	$assemblyVersion = "1.0.0.0"
	$assemblyRevision = "0"
	
	$base_directory = Resolve-Path ..
	$msiProject = "$base_directory\src\LaCore.Hyperion.Setup.sln"
	$msiSource = "$base_directory\bin\$config\LaCore.Hyperion.Setup\LaCore.Hyperion.Setup.msi"
	$msiDeployDir = "$base_directory\bin\Deploy"
	$msiDeploy = "$msiDeployDir\$assemblyInformationalVersion.msi"
}

task default -depends Release;

task -name ValidateConfig -action {
    assert ( 'release', 'stage' -contains $config) ` "Invalid config: $config; valid values are release or stage";
}

task -name Release -depends ValidateConfig -action {
	Invoke-psake .\build.ps1 Build -properties @{ config=$config; assemblyInformationalVersion=$assemblyInformationalVersion; assemblyFileVersion=$assemblyFileVersion; assemblyVersion=$assemblyVersion;  assemblyRevision=$assemblyRevision; }
	exec { msbuild /nologo /verbosity:minimal $msiProject /t:Clean /p:Configuration=$config /m }
    exec { msbuild /nologo /verbosity:minimal $msiProject /p:Configuration=$config /m }
    Create-Directory $msiDeployDir
    Copy-Item $msiSource $msiDeploy
	write-host $msiDeploy
}

function Create-Directory($dir){
	if (!(Test-Path -path $dir)) { new-item $dir -force -type directory}
}