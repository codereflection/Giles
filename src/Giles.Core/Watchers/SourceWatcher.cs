using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Giles.Core.IO;
using Giles.Core.Runners;

namespace Giles.Core.Watchers
{
    public class SourceWatcher : IDisposable
    {
        readonly IBuildRunner buildRunner;
        readonly Timer buildTimer;
        readonly IFileSystem fileSystem;
        readonly IFileWatcherFactory fileWatcherFactory;
        readonly ITestRunner testRunner;
        string solutionPath;
        public List<FileSystemWatcher> FileWatchers { get; set; }


        public SourceWatcher(IBuildRunner buildRunner, ITestRunner testRunner, IFileSystem fileSystem, IFileWatcherFactory fileWatcherFactory)
        {
            FileWatchers = new List<FileSystemWatcher>();
            this.fileSystem = fileSystem;
            this.buildRunner = buildRunner;
            this.fileWatcherFactory = fileWatcherFactory;
            this.testRunner = testRunner;
            buildTimer = new Timer(2000) {AutoReset = false, Enabled = false, Interval = 500};
            buildTimer.Elapsed += new ElapsedEventHandler(buildTimer_Elapsed);
        }

        void buildTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunNow();
        }


        public void Dispose()
        {
            FileWatchers.ToList().ForEach(x => x.Dispose());
        }

        public void Watch(string solutionPath, string filter)
        {
            solutionPath = solutionPath;
            var solutionFolder = fileSystem.GetDirectoryName(solutionPath);
            var fileSystemWatcher = fileWatcherFactory.Build(solutionFolder, filter, ChangeAction, null,
                                                                           ErrorAction);
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.IncludeSubdirectories = true;

            FileWatchers.Add(fileSystemWatcher);
        }

        public void ErrorAction(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ChangeAction(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine("Detected a change in:\n\t" + e.FullPath);

            if (buildTimer.Enabled)
            {
                buildTimer.Enabled = false;
                buildTimer.Enabled = true;
            }
            else
                buildTimer.Enabled = true;
        }

        public void RunNow()
        {
            buildRunner.Run();
            testRunner.Run();
        }
    }
}