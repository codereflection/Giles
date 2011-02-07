function install_to
{
	param(
		[Parameter(Position=0,Mandatory=1)]
		[string] $path = $null
		)
	if(!(test-path $path)){new-item -path . -name $path -itemtype directory}
	push-location $path -stackname 'chewie_nuget'
}

function nuget 
{
	[CmdletBinding()]
	param(
		[Parameter(Position=0,Mandatory=1)]
		[string] $name = $null,
		
		[Parameter(Position=1,Mandatory=0)]
		[string] $version = ""
		)
		
		$command = "nuget.exe install $name"
		if($version -ne ""){$command += " -v $version"}
		
	invoke-expression $command

}

gc $pwd\.NugetFile | Foreach-Object { $block = [scriptblock]::Create($_.ToString()); % $block;}
if((get-location -stackname 'chewie_nuget').count -gt 0) {pop-location -stackname 'chewie_nuget'}