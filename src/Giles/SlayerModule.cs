using Giles.Core.IO;
using Ninject.Modules;

namespace Giles
{
    public class SlayerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileSystem>().To<FileSystem>();
        }
    }
}