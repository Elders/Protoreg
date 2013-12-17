param(
	$config='stage'
	)
.\psake.ps1 .\release.ps1 -properties @{ config=$config }
