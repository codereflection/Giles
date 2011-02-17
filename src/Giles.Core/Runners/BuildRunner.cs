using System;
using System.Diagnostics;
using Giles.Core.Configuration;

namespace Giles.Core.Runners
{
    public interface IBuildRunner : IRunner
    {
    }

    public class BuildRunner : RunnerBase, IBuildRunner
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
            var buildProcess = SetupProcess(settings.MsBuild, config.SolutionPath);

            var watch = new Stopwatch();

            Console.WriteLine("Building...");

            watch.Start();
            config.Executor.Execute(settings.MsBuild, config.SolutionPath);
            watch.Stop();
            

            Console.WriteLine("Build complete in {0} seconds. Result: {1}", 
                watch.Elapsed.TotalSeconds,
                exitCode == 0 ? "Success" : "Failure");

            //Console.WriteLine("\n\n======= BUILD RESULTS =======");
            //Console.WriteLine(output);
        }
    }
}