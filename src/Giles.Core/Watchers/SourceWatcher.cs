using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Giles.Core.IO;

namespace Giles.Core.Watchers
{
    public class SourceWatcher : IDisposable
    {
        readonly IFileSystem fileSystem;
        public List<FileSystemWatcher> FileWatchers { get; set; }

        public SourceWatcher(IFileSystem fileSystem)
        {
            this.FileWatchers = new List<FileSystemWatcher>();
            this.fileSystem = fileSystem;
        }

        public void Watch(string path, string filter)
        {
            FileWatchers.Add(fileSystem.SetupFileWatcher(path, filter, null, null, null, null, null));  
        }

        public void Dispose()
        {
            FileWatchers.ToList().ForEach(x => x.Dispose());
        }
    }
}