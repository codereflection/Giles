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

        public FileSystemWatcher Build(string Path, string Filter, FileSystemEventHandler ChangedAction, FileSystemEventHandler CreatedAction, ErrorEventHandler ErrorAction)
        {
            return fileSystem.SetupFileWatcher(Path, Filter, ChangedAction, CreatedAction, null, null, ErrorAction);
        }
    }
}