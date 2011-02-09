using Giles.Core.IO;
using Giles.Core.Watchers;
using Ninject.Modules;

namespace Giles
{
    public class SlayerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileSystem>().To<FileSystem>();
            Bind<IBuildRunner>().To<BuildRunner>();
            Bind<IFileWatcherFactory>().To<FileWatcherFactory>();
            Bind<SourceWatcher>().ToSelf();
        }
    }
}