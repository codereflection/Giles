using Giles.Core.Configuration;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.Watchers;
using Ninject.Modules;

namespace Giles
{
    public class SlayerModule : NinjectModule
    {
        readonly GilesConfig config;

        public SlayerModule(GilesConfig config)
        {
            this.config = config;
        }

        public override void Load()
        {
            Bind<IFileSystem>().To<FileSystem>();
            Bind<IBuildRunner>().To<BuildRunner>().WithConstructorArgument("config", config);
            Bind<IFileWatcherFactory>().To<FileWatcherFactory>();
            Bind<SourceWatcher>().ToSelf().InSingletonScope().WithConstructorArgument("config", config);
			Bind<ISolutionParser>().To<RegexSolutionParser>().InSingletonScope();
        }
    }
}