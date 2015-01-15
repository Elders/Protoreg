properties {
	$base_directory = Resolve-Path ..
	$src_directory = "$base_directory\src"
 	
	$sln="Elders.Protoreg.sln"
	
	$config = "debug"; #debug or release or stage
	
	$company="Elders"
	$product="Protoreg"
 
	$assemblyInformationalVersion = "dev";
	$assemblyFileVersion = "1.0.?";
	$assemblyVersion = "1.0.0.0";
	$assemblyRevision = "11";

	$nugetSourceDir = "Elders.Protoreg"
	$nugetSourceFiles = @("Elders.Protoreg.dll", "Elders.Protoreg.pdb")
}

Framework "4.0"
. ".\nyx.ps1"

task build -depends Init, AssemblyInfo, ValidateConfig, BuildProtoreg
task nuget -depends Init, AssemblyInfo, ValidateConfig, BuildProtoreg, PublishNugetPackage

task ValidateConfig {
    assert ( "debug", "release", "stage" -contains $config) ` "Invalid config: $config; valid values are debug or release or stage";
}

task Init {
	Delete-Directory("$root\bin")
}

task AssemblyInfo {
    Create-AssemblyVersionInfo `
		-assemblyVersion $assemblyVersion `
		-assemblyFileVersion $assemblyFileVersion.Replace("?",$assemblyRevision) `
		-file $src_directory\AssemblyVersionInfo.cs

    Create-AssemblyCompanyProductInfo `
		-company $company `
		-product $product `
		-file $src_directory\AssemblyCompanyProductInfo.cs
}

task BuildProtoreg -depends ValidateConfig, AssemblyInfo -description "Builds outdated source files" {
    Build "$src_directory\$sln" $config
}

task PublishNugetPackage {
    $version = $assemblyFileVersion.Replace("?",$assemblyRevision)

    Nuget-CreateNuspec `
        -authors "Nikolai Mynkow, Simeon Dimov" `
        -owners "Nikolai Mynkow, Simeon Dimov" `
        -copyright Elders `
        -requireLicenseAcceptance false `
		-licenseUrl https://github.com/Elders/Protoreg/blob/master/LICENSE `
        -projectUrl https://github.com/Elders/Protoreg `
        -product $product `
        -version  $version `
        -description "Protobuf-net wrapper" `
		-dependencies @(
						,@("protobuf-net", "[2.0.0.668, 2.1)")
					   ) `
        -file "$nugetDeploy\Product.nuspec" `

    Nuget-BuildPackage $nugetSourceDir $nugetSourceFiles

    Nuget-PushPackage $version
}