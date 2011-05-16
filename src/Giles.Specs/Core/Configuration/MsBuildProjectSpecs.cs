using System;
using System.Linq;
using Giles.Core.Configuration;
using Machine.Specifications;

namespace Giles.Specs.Core.Configuration
{
    public class an_msbuild_project
    {
        protected static string projectPath;
        protected static MsBuildProject project;

        private Establish context = () => {
            projectPath = "Giles.Specs.Core.Configuration.Resources.Giles.Specs.csproj";
        };
    }

    public class when_an_msbuild_project_is_loaded : an_msbuild_project
    {
        Because of = () => {
            project = MsBuildProject.Load(TestResources.Read(projectPath));
        };

        It returns_referenced_assemblies_with_local_paths = () => {
            project.GetLocalAssemblyRefs()
                .Any(x => x == @"..\..\lib\NSubstitute.1.0.0.0\lib\35\NSubstitute.dll")
                .ShouldEqual(true);
            project.GetLocalAssemblyRefs()
                .Any(x => x == @"..\..\tools\mspec\Machine.Specifications.dll")
                .ShouldEqual(true);
            project.GetLocalAssemblyRefs().Count().ShouldEqual(2);
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
    }
}
