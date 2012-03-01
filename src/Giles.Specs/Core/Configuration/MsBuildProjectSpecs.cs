using System;
using System.Linq;
using Giles.Core.Configuration;
using Machine.Specifications;

namespace Giles.Specs.Core.Configuration
{
    [Subject(typeof(MsBuildProject))]
    public class when_loading
    {
        protected static string projectPath;
        protected static MsBuildProject project;

        Establish context = () => 
            projectPath = "Giles.Specs.Core.Configuration.Resources.Giles.Specs.csproj";

        Because of = () =>
            project = MsBuildProject.Load(TestResources.Read(projectPath));

        It returns_referenced_assemblies_with_relative_paths = () => {
            project.GetLocalAssemblyRefs()
                .Count(x => x == @"..\..\lib\NSubstitute.1.0.0.0\lib\35\NSubstitute.dll")
                .ShouldEqual(1);
            project.GetLocalAssemblyRefs()
                .Count(x => x == @"..\..\tools\mspec\Machine.Specifications.dll")
                .ShouldEqual(1);
        };

        It returns_the_default_platform_configuration = () =>
            project.GetDefaultPlatformConfig().ShouldEqual("Debug|AnyCPU");

        It returns_property_values_based_on_platform_configuration = () => {
            project.GetPropertyValue("Release|AnyCPU", "OutputPath").ShouldEqual(@"bin\Release\");
            project.GetPropertyValue("Debug|AnyCPU", "OutputPath").ShouldEqual(@"bin\Debug\");
            project.GetPropertyValue("Debug|AnyCPU", "DefineConstants").ShouldEqual("DEBUG;TRACE");
        };

        It returns_the_default_property_value_if_platform_specific_not_found = () =>
            project.GetPropertyValue("Release|AnyCPU", "AssemblyName").ShouldEqual(@"Giles.Specs");

        It should_not_return_property_value_for_other_platform_configurations = () =>
            typeof(InvalidOperationException).ShouldBeThrownBy(
                () => project.GetPropertyValue("Release|AnyCPU", "DebugSymbols"));

        It returns_the_output_assembly_path = () =>
            project.GetAssemblyFilePath(@"C:\projects\Giles\src\Giles.Specs\Giles.Specs.csproj")
                .ShouldEqual(@"C:\projects\Giles\src\Giles.Specs\bin\Debug\Giles.Specs.dll");

        It returns_the_assembly_name = () =>
            project.GetAssemblyName().ShouldEqual(@"Giles.Specs");
    }

    [Subject(typeof(MsBuildProject))]
    public class when_loading_a_project_with_an_invalid_default_platform
    {
        protected static string projectPath;
        protected static MsBuildProject project;
        static string defaultPlatformConfig;

        Establish context = () =>
            {
                projectPath = "Giles.Specs.Core.Configuration.Resources.Giles.Specs.WithInvalidPlatform.csproj";
                project = MsBuildProject.Load(TestResources.Read(projectPath));
            };

        Because of = () =>
            defaultPlatformConfig = project.GetDefaultPlatformConfig();

        It should_default_to_the_next_available_platform_in_the_project_file = () =>
            defaultPlatformConfig.ShouldEqual("Debug|x86");
    }
}
