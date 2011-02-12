using System;
using System.Diagnostics;
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
        readonly ConsoleColor defaultConsoleColor;

        public TestRunner(GilesConfig config)
        {
            this.config = config;
            defaultConsoleColor = Console.ForegroundColor;
        }

        public void Run()
        {
            var watch = new Stopwatch();
            watch.Start();

            config.TestRunners.Where(x => x.Value.Enabled).Each(x =>
                        {
                            var args = config.TestAssemblyPath;

                            if (x.Value.Options.Count > 0)
                                args += " " + x.Value.Options.Aggregate((working, next) => working + next);

                            var testProcess = SetupProcess(x.Value.Path, args);
                            testProcess.Start();
                            var output = testProcess.StandardOutput.ReadToEnd();

                            testProcess.WaitForExit();
                            
                            var exitCode = testProcess.ExitCode;

                            testProcess.Close();
                            testProcess.Dispose();

                            Console.WriteLine("\n\n======= {0} TEST RUNNER RESULTS =======", x.Key.ToUpper().Replace(".EXE", string.Empty));
                            Console.ForegroundColor = exitCode != 0 ? 
                                ConsoleColor.Red : defaultConsoleColor;

                            Console.WriteLine(output);

                            Console.ForegroundColor = defaultConsoleColor;
                        });
            watch.Stop();
            Console.WriteLine("All test runners completed in {0} seconds.", watch.Elapsed.TotalSeconds);
        }
    }
}