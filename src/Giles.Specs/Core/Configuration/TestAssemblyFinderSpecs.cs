using System.Collections.Generic;
using System.IO;
using System.Linq;
using Giles.Core.Configuration;
using Giles.Core.IO;
using Machine.Specifications;
using NSubstitute;

namespace Giles.Specs.Core.Configuration
{
    [Subject(typeof(TestAssemblyFinder))]
    public class a_solution_with_a_test_project
    {
        protected static TestAssemblyFinder testFinder;
        protected static IEnumerable<string> configuration;
        protected static string solutionFilePath;
        protected static IFileSystem fileSystem;

        Establish context = () => {
            solutionFilePath = @"c:\app\src\solution.sln";
            fileSystem = Substitute.For<IFileSystem>();
            fileSystem.ReadAllText(solutionFilePath).Returns(
                TestResources.ReadAllText(
                    "Giles.Specs.Core.Configuration.Resources.Giles.sln"));

            fileSystem.OpenFile(@"c:\app\src\Giles\Giles.csproj",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                .Returns(TestResources.Read(
                    "Giles.Specs.Core.Configuration.Resources.Giles.csproj"));

            fileSystem.OpenFile(@"c:\app\src\Giles.Core\Giles.Core.csproj",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                .Returns(TestResources.Read(
                    "Giles.Specs.Core.Configuration.Resources.Giles.Core.csproj"));

            fileSystem.OpenFile(@"c:\app\src\Giles.Specs\Giles.Specs.csproj",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                .Returns(TestResources.Read(
                    "Giles.Specs.Core.Configuration.Resources.Giles.Specs.csproj"));

            testFinder = new TestAssemblyFinder
                             {
                                 GetFileSystem = () => fileSystem
                             };
        };
    }

    [Subject(typeof(TestAssemblyFinder))]
    public class when_loading_configuration_settings_from_a_solution_with_a_test_project : a_solution_with_a_test_project
    {
        Because of = () =>
            configuration = testFinder.FindTestAssembliesIn(solutionFilePath);

        It reads_the_solution_file = () =>
            fileSystem.Received().ReadAllText(solutionFilePath);

        It reads_the_csharp_project_files = () =>
        {
            fileSystem.Received().OpenFile(@"c:\app\src\Giles\Giles.csproj",
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileSystem.Received().OpenFile(@"c:\app\src\Giles.Core\Giles.Core.csproj",
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileSystem.Received().OpenFile(@"c:\app\src\Giles.Specs\Giles.Specs.csproj",
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        };

        It determines_the_test_assembly_file_path = () =>
            configuration.Single().ShouldEqual(
                @"c:\app\src\Giles.Specs\bin\Debug\Giles.Specs.dll");
    }

    [Subject(typeof(TestAssemblyFinder))]
    public class a_solution_with_two_projects_referencing_test_frameworks : a_solution_with_a_test_project
    {
        // switch call for Core project to return core project with MSpec reference
        Establish context = () => 
            fileSystem.OpenFile(@"c:\app\src\Giles.Core\Giles.Core.csproj",
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                .Returns(TestResources.Read(
                    "Giles.Specs.Core.Configuration.Resources.Giles.Core.WithMSpec.csproj"));
    }

    [Subject(typeof(TestAssemblyFinder))]
    public class when_a_solution_contains_two_projects_referencing_test_frameworks : a_solution_with_two_projects_referencing_test_frameworks
    {
        private static IEnumerable<string> assemblies;

        Because of = () =>
            assemblies = testFinder.FindTestAssembliesIn(solutionFilePath);

        It should_find_both_test_projects = () =>
            assemblies.Count().ShouldEqual(2);

        // HERE: implement this
        It should_list_the_project_with_Spec_in_the_name_first = () =>
            assemblies.First().ShouldEqual(@"c:\app\src\Giles.Specs\bin\Debug\Giles.Specs.dll");
    }
}