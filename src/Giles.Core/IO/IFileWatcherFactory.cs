using System.IO;

namespace Giles.Core.IO
{
    public interface IFileWatcherFactory
    {
        FileSystemWatcher Build(FileSystemWatcherOptions fileSystemWatcherOptions);
    }
}