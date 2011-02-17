using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Giles.Core.Configuration;
using Machine.Specifications.Utility;

namespace Giles.Core.Runners
{
    public interface ITestRunner : IRunner
    {
    }

    public class TestRunner : ITestRunner
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

            config.TestRunners.Where(x => x.Value.Enabled).Each(ExecuteRunner);

            watch.Stop();
            Console.WriteLine("All test runners completed in {0} seconds.", watch.Elapsed.TotalSeconds);
        }

        void ExecuteRunner(KeyValuePair<string, RunnerAssembly> x)
        {
            var args = config.TestAssemblyPath;

            if (x.Value.Options.Count > 0)
                args += " " + x.Value.Options.Aggregate((working, next) => working + next);

           
            var result = config.Executor.Execute(x.Value.Path, args);

            Console.WriteLine("\n\n======= {0} TEST RUNNER RESULTS =======", x.Key.ToUpper().Replace(".EXE", string.Empty));
            Console.ForegroundColor = result.ExitCode != 0 ? 
                                                        ConsoleColor.Red : defaultConsoleColor;

            Console.WriteLine(result.Output);

            Console.ForegroundColor = defaultConsoleColor;
        }
    }
}