#rakefile.rb

require 'rake'
require 'albacore'

task :default => [:full]


task :full => [:clean,:assemblyInfo,:build,:specifications]

task :clean do
	FileUtils.rm_rf 'build'	
end


msbuild :build do |msb|
	msb.properties :configuration => :AutomatedRelease
	msb.solution = "src/Giles.sln"
end


mspec :specifications do |mspec|
	mspec.command = "tools/mspec/mspec.exe"
	mspec.assemblies = "build/Giles.Specs.dll"
	mspec.html_output = "report/Specs"
end


desc "Assembly Version Info Generator"
assemblyinfo :assemblyInfo do |asm|
	asm.output_file ="src/ProjectVersion.cs"
	asm.title = "Giles, Rupert Giles, at your service!"
	asm.company_name = "codereflection"
	asm.product_name = "Giles - auto test runner"
	asm.version = "0.0.2.0"
	asm.file_version = "0.0.2.0"
	asm.copyright = "Copyright (c)2011 Jeff 'codereflection' Schumacher"
end