using System.IO;
using System.Linq;
using Giles.Core.Configuration;
using Giles.Core.IO;
using Machine.Specifications;
using Machine.Specifications.Utility;
using NSubstitute;

namespace Giles.Specs.Core.Configuration
{
    public class a_giles_config
    {
        protected static GilesConfigFactory factory;
        protected static GilesConfig config;
        protected static IFileSystem fileSystem;
        protected static string solutionPath;
        protected static string solutionFolder;
        protected static string testRunnerExe;
        protected static string testAssemblyPath;
        protected static string projectRoot;

        Establish context = () =>
                                {
                                    solutionFolder = @"c:\solutionRoot\solution";
                                    solutionPath = @"c:\solutionRoot\solution\mySolution.sln";
                                    testAssemblyPath = @"c:\solutionRoot\solution\tests\bin\debug\tests.dll";
                                    fileSystem = Substitute.For<IFileSystem>();
                                    testRunnerExe = @"c:\testAssembly.exe";
                                    fileSystem.GetFiles(Arg.Any<string>(), Arg.Any<string>(), SearchOption.AllDirectories)
                                        .Returns(new[] { testRunnerExe });
                                    fileSystem.GetDirectoryName(solutionPath).Returns(solutionFolder);
                                    projectRoot = @"c:\solutionRoot";

                                    config = new GilesConfig();
                                    factory = new GilesConfigFactory(config, fileSystem, solutionPath, testAssemblyPath, projectRoot);
                                };

    }

    public class when_building : a_giles_config
    {
        Because of = () =>
            factory.Build();

        It locates_the_mspec_test_runner = () =>
            fileSystem.Received().GetFiles(projectRoot, "mspec.exe", SearchOption.AllDirectories);

        It locates_the_nunit_test_runner = () =>
            fileSystem.Received().GetFiles(projectRoot, "nunit-console.exe", SearchOption.AllDirectories);

        It built_the_correct_config_test_runners = () =>
            config.TestRunners.All(x => x.Value.Path == testRunnerExe).ShouldBeTrue();

        It assigned_the_test_assembly_path = () =>
            config.TestAssemblyPath.ShouldEqual(testAssemblyPath);

        It assigned_the_solution_path = () =>
            config.SolutionPath.ShouldEqual(solutionPath);

        It recognized_the_nunit_test_runner_options = () =>
            config.TestRunners.First(x => x.Key == "nunit-console.exe").Value.Options.First().ShouldEqual("/nologo");

        It recognized_the_mspec_test_runner_options = () =>
            config.TestRunners.First(x => x.Key == "mspec.exe").Value.Options.ShouldBeEmpty();

        It should_configure_the_console_user_display = () =>
                                                       config.UserDisplay.Count().ShouldBeGreaterThan(0);

        It should_configure_a_command_process_executor = () =>
                                                         config.Executor.ShouldNotBeNull();

    }

}