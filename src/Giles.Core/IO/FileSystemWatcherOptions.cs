using System.IO;

namespace Giles.Core.IO
{
    public class FileSystemWatcherOptions
    {
        public FileSystemWatcherOptions(string path, string filter, FileSystemEventHandler changedAction, FileSystemEventHandler createdAction, ErrorEventHandler errorAction, FileSystemEventHandler deletedAction, RenamedEventHandler renamedAction)
        {
            Path = path;
            Filter = filter;
            ChangedAction = changedAction;
            CreatedAction = createdAction;
            ErrorAction = errorAction;
            DeletedAction = deletedAction;
            RenamedAction = renamedAction;
        }

        public string Path { get; private set; }

        public string Filter { get; private set; }

        public FileSystemEventHandler ChangedAction { get; private set; }

        public FileSystemEventHandler CreatedAction { get; private set; }

        public ErrorEventHandler ErrorAction { get; private set; }

        public FileSystemEventHandler DeletedAction { get; private set; }

        public RenamedEventHandler RenamedAction { get; private set; }
    }
}