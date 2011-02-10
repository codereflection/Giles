using System;
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

            buildProcess.Start();
            var output = buildProcess.StandardOutput.ReadToEnd();

            buildProcess.WaitForExit();
            buildProcess.Close();
            buildProcess.Dispose();

            //Console.WriteLine("\n\n======= BUILD RESULTS =======");
            //Console.WriteLine(output);
        }
    }
}