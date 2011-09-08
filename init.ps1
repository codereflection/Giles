param($rootPath, $toolsPath, $package, $project)

$gilesRunnerFrom = $toolsPath + "\giles.ps1"
$gilesRunnerTo = "giles.ps1"

if(!(Test-Path $gilesRunnerTo))
{
  Copy-Item $gilesRunnerFrom $gilesRunnerTo
}