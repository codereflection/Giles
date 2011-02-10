using System;

namespace Giles.Core.Runners
{
    public interface IBuildRunner : IRunner
    {
    }

    public class BuildRunner : RunnerBase, IBuildRunner
    {
        readonly string solutionPath;

        public BuildRunner(string solutionPath)
        {
            this.solutionPath = solutionPath;
        }

        public void Run()
        {
            var buildProcess = SetupProcess(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe", solutionPath);

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