using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.UI;
using Giles.Core.Utility;

namespace Giles.Core.Configuration
{
    public class GilesConfigFactory
    {
        readonly GilesConfig config;
        readonly IFileSystem fileSystem;
        readonly string solutionPath;
        readonly string testAssemblyPath;
        readonly string projectRoot;
        readonly string[] supportedRunners;

        public GilesConfigFactory(GilesConfig config, IFileSystem fileSystem, string solutionPath, string testAssemblyPath, string projectRoot)
        {
            this.config = config;
            this.fileSystem = fileSystem;
            this.solutionPath = solutionPath;
            this.testAssemblyPath = testAssemblyPath;
            this.projectRoot = projectRoot;
         
            supportedRunners = new[] {"mspec.exe", "nunit-console.exe:/nologo"};
        }

        public GilesConfig Build()
        {
            LocateTestRunners();
            config.TestAssemblyPath = testAssemblyPath;
            config.TestAssembly = Assembly.LoadFrom(config.TestAssemblyPath);
            config.SolutionPath = solutionPath;

            config.UserDisplay = new List<IUserDisplay> {new ConsoleUserDisplay(), new GrowlUserDisplay()};
            config.Executor = new CommandProcessExecutor();
            return config;
        }

        void LocateTestRunners()
        {
            supportedRunners.Each(x =>
                {
                    var executeable = x.Split(':').First();
                    var options = x.Split(':').Where(opt => opt != executeable).ToList();

                    var files = fileSystem.GetFiles(projectRoot, executeable,
                                                        SearchOption.AllDirectories);

                    config.TestRunners.Add(executeable,
                                            new RunnerAssembly
                                                {
                                                    Path = files.FirstOrDefault(),
                                                    Enabled = !string.IsNullOrWhiteSpace(files.FirstOrDefault()),
                                                    Options = options
                                                });
                });
        }
    }
}