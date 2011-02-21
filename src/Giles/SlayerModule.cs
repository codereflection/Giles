using Giles.Core.Configuration;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.Watchers;
using Ninject.Modules;

namespace Giles
{
    public class SlayerModule : NinjectModule
    {
        readonly string solutionPath;
        readonly string testAssemblyPath;
        readonly string projectRoot;

        public SlayerModule(string solutionPath, string testAssemblyPath, string projectRoot)
        {
            this.solutionPath = solutionPath;
            this.testAssemblyPath = testAssemblyPath;
            this.projectRoot = projectRoot;
        }

        public override void Load()
        {
            Bind<IFileSystem>().To<FileSystem>();
            Bind<IBuildRunner>().To<BuildRunner>();
            Bind<ITestRunner>().To<TestRunner>();
            Bind<IFileWatcherFactory>().To<FileWatcherFactory>();
            Bind<SourceWatcher>().ToSelf().InSingletonScope();
            Bind<GilesConfig>().ToSelf().InSingletonScope();
            Bind<GilesConfigFactory>().ToSelf()
                .WithConstructorArgument("solutionPath", solutionPath)
                .WithConstructorArgument("testAssemblyPath", testAssemblyPath)
                .WithConstructorArgument("projectRoot", projectRoot);
            Bind<ITestRunnerResolver>().To<TestRunnerResolver>().InSingletonScope();
        }
    }
}