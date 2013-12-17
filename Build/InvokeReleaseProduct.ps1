param(
	$config="release"
	)
.\psake.ps1 .\release.ps1 -properties @{ config=$config }
