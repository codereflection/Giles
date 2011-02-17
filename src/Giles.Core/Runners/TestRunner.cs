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

        public TestRunner(GilesConfig config)
        {
            this.config = config;
        }

        public void Run()
        {
            var watch = new Stopwatch();
            watch.Start();

            config.TestRunners.Where(x => x.Value.Enabled).Each(ExecuteRunner);

            watch.Stop();

            config.UserDisplay.Each(display => display.DisplayMessage("All test runners completed in {0} seconds.", watch.Elapsed.TotalSeconds));
        }

        void ExecuteRunner(KeyValuePair<string, RunnerAssembly> x)
        {
            var args = config.TestAssemblyPath;

            if (x.Value.Options.Count > 0)
                args += " " + x.Value.Options.Aggregate((working, next) => working + next);

           
            var result = config.Executor.Execute(x.Value.Path, args);

            config.UserDisplay.Each(display => display.DisplayResult(result));
        }
    }
}