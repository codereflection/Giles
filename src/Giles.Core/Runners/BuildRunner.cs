using System;
using System.Diagnostics;
using Giles.Core.Configuration;
using Machine.Specifications.Utility;

namespace Giles.Core.Runners
{
    public interface IBuildRunner : IRunner
    {
    }

    public class BuildRunner : IBuildRunner
    {
        readonly GilesConfig config;
        readonly Settings settings;

        public BuildRunner(GilesConfig config, Settings settings)
        {
            this.config = config;
            this.settings = settings;
        }

        public void Run()
        {
            Debugger.Break();
            var watch = new Stopwatch();
            config.UserDisplay.Each(display => display.DisplayMessage("Building..."));

            watch.Start();
            config.Executor.Execute(settings.MsBuild, config.SolutionPath);
            watch.Stop();
            
            config.UserDisplay.Each(display => display.DisplayMessage("Building complete in {0} seconds.", watch.Elapsed.TotalSeconds));
        }
    }
}