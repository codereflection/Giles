using System;
using System.IO;
using System.Linq;
using Giles.Core.Configuration;
using Machine.Specifications.Utility;

namespace Giles.Core.Runners
{
    public interface ITestRunner : IRunner
    {
    }

    public class TestRunner : RunnerBase, ITestRunner
    {
        readonly GilesConfig config;
        readonly string testAssemblyPath;

        public TestRunner(GilesConfig config, string testAssemblyPath)
        {
            this.config = config;
            this.testAssemblyPath = testAssemblyPath;
        }

        public void Run()
        {
            config.TestRunners.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Each(x =>
                        {
                            var testProcess = SetupProcess(x.Value, testAssemblyPath);
                            testProcess.Start();
                            var output = testProcess.StandardOutput.ReadToEnd();

                            testProcess.WaitForExit();
                            testProcess.Close();
                            testProcess.Dispose();

                            Console.WriteLine("\n\n======= TEST RESULTS {0} =======", DateTime.Now.ToLongTimeString());
                            Console.WriteLine(output);
                        });
        }
    }
}