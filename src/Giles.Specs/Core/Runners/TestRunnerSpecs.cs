using Giles.Core.Configuration;
using Giles.Core.Runners;
using Machine.Specifications;

namespace Giles.Specs.Core.Runners
{
    public class with_a_test_runner
    {
        protected static TestRunner runner;
        protected static string solutionPath;
        protected static string solutionFolder;
        protected static string testAssemblyPath;
        static GilesConfig config;

        Establish context = () =>
                                {
                                    solutionFolder = @"c:\solutionFolder";
                                    solutionPath = @"c:\solutionFolder\mySolution.sln";
                                    testAssemblyPath = @"c:\solutionFolder\testProject\bin\debug\testAssembly.dll";

                                    config = new GilesConfig();
                                };

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