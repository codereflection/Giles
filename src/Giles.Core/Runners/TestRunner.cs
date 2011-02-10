using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Giles.Core.IO;
using Machine.Specifications.Utility;

namespace Giles.Core.Runners
{
    public interface ITestRunner : IRunner
    {
    }

    public class TestRunner : RunnerBase, ITestRunner
    {
        readonly IFileSystem fileSystem;
        readonly string solutionPath;
        readonly string testAssemblyPath;
        IList<string> supportedRunners;
        IDictionary<string,string> testRunners = new Dictionary<string, string>();
        string solutionFolder;

        public TestRunner(IFileSystem fileSystem, string solutionPath, string testAssemblyPath)
        {
            this.fileSystem = fileSystem;
            this.solutionPath = solutionPath;
            this.testAssemblyPath = testAssemblyPath;
            supportedRunners = new[] { "mspec.exe", "nunit-console.exe" };

            solutionFolder = fileSystem.GetDirectoryName(solutionPath);
            LocateTestRunner();
        }

        void LocateTestRunner()
        {
            supportedRunners.Each(x =>
                                 {
                                     var files = fileSystem.GetFiles(solutionFolder, x, SearchOption.AllDirectories);
                                     testRunners.Add(x, files.FirstOrDefault());
                                 });
        }

        public void Run()
        {
            var testProcess = SetupProcess(testRunners.FirstOrDefault().Value, testAssemblyPath);
            testProcess.Start();
            var output = testProcess.StandardOutput.ReadToEnd();

            testProcess.WaitForExit();
            testProcess.Close();
            testProcess.Dispose();

            Console.WriteLine("\n\n======= TEST RESULTS {0} =======", DateTime.Now.ToLongTimeString());
            Console.WriteLine(output);
        }
    }
}