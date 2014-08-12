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

        public FileSystemWatcher Build(FileSystemWatcherOptions fileSystemWatcherOptions)
        {
            return fileSystem.SetupFileWatcher(fileSystemWatcherOptions.Path, 
                                               fileSystemWatcherOptions.Filter, 
                                               fileSystemWatcherOptions.ChangedAction, 
                                               fileSystemWatcherOptions.CreatedAction, 
                                               fileSystemWatcherOptions.DeletedAction, 
                                               fileSystemWatcherOptions.RenamedAction, 
                                               fileSystemWatcherOptions.ErrorAction);
        }
    }
}