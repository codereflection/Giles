using System;

namespace Giles.Core.Runners
{
    public interface IBuildRunner : IRunner
    {
    }

    public class BuildRunner : IBuildRunner
    {
        readonly string solutionPath;

        public BuildRunner(string solutionPath)
        {
            this.solutionPath = solutionPath;
        }

        public void Run()
        {
            var buildProcess = new System.Diagnostics.Process
                                   {
                                       StartInfo =
                                           {
                                               FileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe",
                                               Arguments = solutionPath,
                                               UseShellExecute = false,
                                               RedirectStandardOutput = true
                                           }
                                   };
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