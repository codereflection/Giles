using System.IO;

namespace Giles.Core.IO
{
    public class FileWatcherFactory : IFileWatcherFactory
    {
        readonly IFileSystem fileSystem;

        public FileWatcherFactory(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public FileSystemWatcher Build(string path, string filter, FileSystemEventHandler changedAction,
                                       FileSystemEventHandler createdAction, ErrorEventHandler errorAction)
        {
            return fileSystem.SetupFileWatcher(path, filter, changedAction, createdAction, null, null, errorAction);
        }
    }
}