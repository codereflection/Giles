using System;
using System.Collections.Generic;
using Giles.Core.Configuration;
using Giles.Core.Runners;
using Machine.Specifications;
using NSubstitute;

namespace Giles.Specs.Core.Runners
{
    public class with_a_test_runner
    {
        protected static TestRunner runner;
        protected static string solutionPath;
        protected static string solutionFolder;
        protected static string testAssemblyPath;
        protected static GilesConfig config;
        static Dictionary<string, RunnerAssembly> runners;

        Establish context = () =>
                                {
                                    solutionFolder = @"c:\solutionFolder";
                                    solutionPath = @"c:\solutionFolder\mySolution.sln";
                                    testAssemblyPath = @"c:\solutionFolder\testProject\bin\debug\testAssembly.dll";
                                    runners = new Dictionary<string, RunnerAssembly>();
                                    runners.Add("foo", new RunnerAssembly { Enabled = true, Options = new List<string> { "bar" }, Path = "baz" });

                                    config = new GilesConfig()
                                                 {
                                                     BuildDelay = 1,
                                                     ProjectRoot = solutionFolder,
                                                     SolutionPath = solutionPath,
                                                     TestAssemblyPath = testAssemblyPath,
                                                     TestRunners = runners
                                                 };
                                    runner = new TestRunner(config);
                                };
    }

    public class when_running_a_test_runner : with_a_test_runner
    {
        static bool executeReceived;

        Establish context = () =>
            CommandProcessExecutor.Execute = (filename, args) => ExecuteReceiver(filename, args);

        static ExecutionResult ExecuteReceiver(string filename, string args)
        {
            executeReceived = true;
            return new ExecutionResult { ExitCode = 0 };
        }

        Because of = () =>
            runner.Run();

        It should_execute_the_runner = () =>
           executeReceived.ShouldBeTrue();
    }
}