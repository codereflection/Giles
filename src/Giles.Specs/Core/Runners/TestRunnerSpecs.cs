using System.IO;
using Giles.Core.IO;
using Giles.Core.Runners;
using Machine.Specifications;
using NSubstitute;

namespace Giles.Specs.Core.Runners
{
    public class with_a_test_runner
    {
        protected static TestRunner runner;
        protected static IFileSystem fileSystem;
        protected static string solutionPath;
        protected static string solutionFolder;
        protected static string testAssemblyPath;

        Establish context = () =>
                                {
                                    solutionPath = @"c:\solutionFolder\mySolution.sln";
                                    testAssemblyPath = @"c:\solutionFolder\testProject\bin\debug\testAssembly.dll";
                                    fileSystem = Substitute.For<IFileSystem>();
                                    solutionFolder = @"c:\solutionFolder";
                                    fileSystem.GetDirectoryName(solutionPath).Returns(solutionFolder);
                                };

    }

    public class when_setting_up_a_new_test_runner : with_a_test_runner
    {
        Because of = () =>
            runner = new TestRunner(fileSystem, solutionPath, testAssemblyPath);

        It should_get_the_solution_folder = () =>
            fileSystem.Received().GetDirectoryName(solutionPath);

        It should_locate_the_test_runner = () =>
            fileSystem.Received().GetFiles(solutionFolder, "mspec.exe", SearchOption.AllDirectories);
    }

    public class when_running_a_test_runner : with_a_test_runner
    {
        Because of = () =>
            runner.Run();

        [Ignore("Process is not abstracted out of the runner yet")]
        It should_execute_the_runner = () =>
            true.ShouldBeTrue();
    }
}