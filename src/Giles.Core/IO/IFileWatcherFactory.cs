using System.IO;

namespace Giles.Core.IO
{
    public interface IFileWatcherFactory
    {
        FileSystemWatcher Build(string Path, string Filter, FileSystemEventHandler ChangedAction, FileSystemEventHandler CreatedAction, ErrorEventHandler ErrorAction);
    }
}