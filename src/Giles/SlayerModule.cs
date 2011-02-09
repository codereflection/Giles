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

        public SlayerModule(string solutionPath, string testAssemblyPath)
        {
            this.solutionPath = solutionPath;
            this.testAssemblyPath = testAssemblyPath;
        }

        public override void Load()
        {
            Bind<IFileSystem>().To<FileSystem>();
            Bind<IBuildRunner>().To<BuildRunner>()
                .WithConstructorArgument("solutionPath", solutionPath);
            Bind<ITestRunner>().To<TestRunner>()
                .WithConstructorArgument("solutionPath", solutionPath)
                .WithConstructorArgument("testAssemblyPath", testAssemblyPath);
            Bind<IFileWatcherFactory>().To<FileWatcherFactory>();
            Bind<SourceWatcher>().ToSelf().InSingletonScope();
        }
    }
}