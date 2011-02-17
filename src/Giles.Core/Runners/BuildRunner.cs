using System;
using System.Diagnostics;
using Giles.Core.Configuration;

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
            var watch = new Stopwatch();

            Console.WriteLine("Building...");

            watch.Start();
            config.Executor.Execute(settings.MsBuild, config.SolutionPath);
            watch.Stop();
            
            Console.WriteLine("Building complete in {0} seconds.", watch.Elapsed.TotalSeconds);

            //Console.WriteLine("\n\n======= BUILD RESULTS =======");
            //Console.WriteLine(output);
        }
    }
}