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
        readonly string solutionFolder;
        readonly string[] supportedRunners;

        public GilesConfigFactory(GilesConfig config, IFileSystem fileSystem, string solutionPath)
        {
            this.config = config;
            this.fileSystem = fileSystem;
            solutionFolder = fileSystem.GetDirectoryName(solutionPath);
            supportedRunners = new[] { "mspec.exe", "nunit-console.exe" };
        }

        public void Build()
        {
            LocateTestRunners();
        }

        void LocateTestRunners()
        {
            supportedRunners.Each(x =>
                {
                    var files = fileSystem.GetFiles(solutionFolder, x, SearchOption.AllDirectories);
                    config.TestRunners.Add(x, files.FirstOrDefault());
                });
        }

    }
}