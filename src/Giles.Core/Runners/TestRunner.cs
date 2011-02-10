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
        readonly string solutionFolder;
        readonly string testAssemblyPath;
        readonly IList<string> supportedRunners;
        
        IDictionary<string,string> testRunners = new Dictionary<string, string>();

        public TestRunner(IFileSystem fileSystem, string solutionPath, string testAssemblyPath)
        {
            this.fileSystem = fileSystem;
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
            testRunners.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Each(x =>
                        {
                            var testProcess = SetupProcess(x.Value, testAssemblyPath);
                            testProcess.Start();
                            var output = testProcess.StandardOutput.ReadToEnd();

                            testProcess.WaitForExit();
                            testProcess.Close();
                            testProcess.Dispose();

                            Console.WriteLine("\n\n======= TEST RESULTS {0} =======", DateTime.Now.ToLongTimeString());
                            Console.WriteLine(output);
                        });
        }
    }
}