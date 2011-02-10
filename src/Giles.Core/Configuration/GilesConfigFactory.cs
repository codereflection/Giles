using System.IO;
using System.Linq;
using Giles.Core.IO;
using Machine.Specifications.Utility;

namespace Giles.Core.Configuration
{
    public class GilesConfigFactory
    {
        readonly GilesConfig config;
        readonly IFileSystem fileSystem;
        readonly string solutionPath;
        readonly string testAssemblyPath;
        readonly string solutionFolder;
        readonly string[] supportedRunners;

        public GilesConfigFactory(GilesConfig config, IFileSystem fileSystem, string solutionPath, string testAssemblyPath)
        {
            this.config = config;
            this.fileSystem = fileSystem;
            this.solutionPath = solutionPath;
            this.testAssemblyPath = testAssemblyPath;
            solutionFolder = fileSystem.GetDirectoryName(solutionPath);
            supportedRunners = new[] {"mspec.exe", "nunit-console.exe"};
        }

        public GilesConfig Build()
        {
            LocateTestRunners();
            config.TestAssemblyPath = testAssemblyPath;
            config.SolutionPath = solutionPath;
            return config;
        }

        void LocateTestRunners()
        {
            supportedRunners.Each(x =>
                                      {
                                          var files = fileSystem.GetFiles(solutionFolder, x,
                                                                               SearchOption.AllDirectories);
                                          
                                          config.TestRunners.Add(x,
                                                                 new RunnerAssembly
                                                                     {
                                                                         Path = files.FirstOrDefault(),
                                                                         Enabled = !string.IsNullOrWhiteSpace(files.FirstOrDefault())
                                                                     });
                                      });
        }
    }
}