#rakefile.rb

require 'rake'
require 'albacore'
require 'lib/FileSystem/filesystem'

task :default => [:full]

@GilesVersion = "0.1.3.0"

task :full => [:clean,:assemblyInfo,:build,:buildx86,:copyx86,:specifications,:createSpec,:createPackage]

task :clean do
	FileUtils.rm_rf 'build'	
end


msbuild :build do |msb|
	msb.properties :configuration => :AutomatedRelease
	msb.solution = "src/Giles.sln"
end

msbuild :buildx86 do |msb|
	msb.properties :configuration => :AutomatedRelease_x86
	msb.solution = "src/Giles.sln"
end

desc "Copy x86 Giles to build directory"
task :copyx86 do
	FileSystem.CopyFiles("build/x86/Giles.exe", "build/Giles-x86.exe")
	FileSystem.CopyFiles("build/x86/Giles.exe.config", "build/Giles-x86.exe.config")
end

mspec :specifications do |mspec|
	mspec.command = "lib/manual/mspec/mspec.exe"
	mspec.assemblies = "build/Giles.Specs.dll"
	mspec.html_output = "report/Specs"
end


desc "Assembly Version Info Generator"
assemblyinfo :assemblyInfo do |asm|
	asm.output_file ="src/ProjectVersion.cs"
	asm.title = "Giles, Rupert Giles, at your service!"
	asm.company_name = "codereflection"
	asm.product_name = "Giles - auto test runner"
	asm.version = @GilesVersion
	asm.file_version = @GilesVersion
	asm.copyright = "Copyright (c)2012 Jeff Schumacher (@codereflection). Rupert Giles and all other Buffy The Vampire Slayer references are copyrights of their respective owners."
end

desc "Prep the package folder"
task :prepPackage do
	FileSystem.DeleteDirectory("deploy")
	FileSystem.EnsurePath("deploy/package")
	FileSystem.EnsurePath("deploy/package/tools")
	FileSystem.DeleteDirectory("build/x86")
	FileSystem.CopyFiles("build/*", "deploy/package")
	FileSystem.CopyFiles("giles.ps1", "deploy/package/tools")
	FileSystem.CopyFiles("giles-x86.ps1", "deploy/package/tools")
	FileSystem.CopyFiles("init.ps1", "deploy/package/tools")
	FileSystem.CopyFiles("ReleaseNotes.txt", "deploy/package")
	FileSystem.CopyFiles("License.txt", "deploy/package")
end

desc "Create the nuspec"
nuspec :createSpec => :prepPackage do |nuspec|
	nuspec.id = "Giles"
	nuspec.version = @GilesVersion
	nuspec.authors = "Jeff Schumacher (@codereflection)"
	nuspec.owners = "Jeff Schumacher (@codereflection)"
	nuspec.description = "Giles - continuous test runner for .NET applications."
	nuspec.summary = "Currently supports Machine.Specifications (mspec), NUnit, xUnit.net, and NSpec"
	nuspec.language = "en-US"
	nuspec.projectUrl = "http://testergiles.herokuapp.com/"
	nuspec.title = "Giles, Rupert Giles, at your service!"
	nuspec.tags = "testrunner test unittest giles"
	nuspec.output_file = "Giles.nuspec"
	nuspec.working_directory = "deploy/package"
	nuspec.licenseUrl = "https://github.com/codereflection/Giles/blob/master/License.txt"
end

desc "Create the nuspec package"
nugetpack :createPackage do |nugetpack|
	nugetpack.nuspec = "deploy/package/Giles.nuspec"
	nugetpack.base_folder = "deploy/package"
	nugetpack.output = "deploy"
end
