using System;
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

        public TestRunner(GilesConfig config)
        {
            this.config = config;
        }

        public void Run()
        {
            config.TestRunners.Where(x => x.Value.Enabled).Each(x =>
                        {
                            var testProcess = SetupProcess(x.Value.Path, config.TestAssemblyPath);
                            testProcess.Start();
                            var output = testProcess.StandardOutput.ReadToEnd();

                            testProcess.WaitForExit();
                            testProcess.Close();
                            testProcess.Dispose();

                            Console.WriteLine("\n\n======= TEST RESULTS {0} =======", DateTime.Now.ToLongTimeString());
                            Console.WriteLine(output);
                        });
            Console.WriteLine("Test run complete.");
        }
    }
}