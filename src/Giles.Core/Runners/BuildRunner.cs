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

        public BuildRunner(GilesConfig config)
        {
            this.config = config;
        }

        public void Run()
        {
            var buildProcess = SetupProcess(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe", config.SolutionPath);

            var watch = new Stopwatch();

            Console.WriteLine("Building...");

            watch.Start();
            buildProcess.Start();
            var output = buildProcess.StandardOutput.ReadToEnd();

            buildProcess.WaitForExit();
            buildProcess.Close();
            buildProcess.Dispose();
            watch.Stop();
            
            Console.WriteLine("Building complete in {0} seconds.", watch.Elapsed.TotalSeconds);

            //Console.WriteLine("\n\n======= BUILD RESULTS =======");
            //Console.WriteLine(output);
        }
    }
}