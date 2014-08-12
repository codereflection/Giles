using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Giles.Core.AppDomains;
using Giles.Core.Configuration;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.Utility;

namespace Giles.Core.Watchers
{
    public class SourceWatcher : IDisposable
    {
        readonly IBuildRunner buildRunner;
        readonly Timer buildDelayTimer;
        readonly IFileSystem fileSystem;
        readonly IFileWatcherFactory fileWatcherFactory;
        readonly GilesConfig config;

        public List<FileSystemWatcher> FileWatchers { get; set; }
        public bool Pause { get; set; }

        public SourceWatcher(IBuildRunner buildRunner, IFileSystem fileSystem,
                             IFileWatcherFactory fileWatcherFactory, GilesConfig config)
        {
            FileWatchers = new List<FileSystemWatcher>();
            this.fileSystem = fileSystem;
            this.buildRunner = buildRunner;
            this.fileWatcherFactory = fileWatcherFactory;
            this.config = config;
            buildDelayTimer = new Timer { AutoReset = false, Enabled = false, Interval = config.BuildDelay };
            config.PropertyChanged += config_PropertyChanged;
            buildDelayTimer.Elapsed += (sender, e) => RunNow();
        }

        void config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "BuildDelay") return;
            
            var buildDelay = ((GilesConfig) sender).BuildDelay;

            buildDelayTimer.Interval = buildDelay;
        }

        public void Dispose()
        {
            FileWatchers.ToList().ForEach(x => x.Dispose());
        }

        public void Watch(string solutionPath, string filter)
        {
            var solutionFolder = fileSystem.GetDirectoryName(solutionPath);
            var fileSystemWatcher = fileWatcherFactory.Build(new FileSystemWatcherOptions(solutionFolder, filter, ChangeAction, null, ErrorAction, DeletedAction, RenamedAction));
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fileSystemWatcher.IncludeSubdirectories = true;

            FileWatchers.Add(fileSystemWatcher);
        }

        private void RenamedAction(object sender, RenamedEventArgs e)
        {
            TriggerBuildTimer();
        }

        private void DeletedAction(object sender, FileSystemEventArgs e)
        {
            // noop
        }

        public void ErrorAction(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ChangeAction(object sender, FileSystemEventArgs e)
        {
            TriggerBuildTimer();
        }

        private void TriggerBuildTimer()
        {
            if (buildDelayTimer.Enabled)
                ResetBuildTimer();
            else
                buildDelayTimer.Enabled = true;
        }

        void ResetBuildTimer()
        {
            buildDelayTimer.Enabled = false;
            buildDelayTimer.Enabled = true;
        }

        public Func<GilesConfig, GilesTestListener> GetListener = config => new GilesTestListener(config);

        public void RunNow()
        {
            if (Pause || !buildRunner.Run())
                return;

            var listener = GetListener.Invoke(config);

            var manager = new GilesAppDomainManager();

            var runResults = new List<SessionResults>();

            var testAssembliesToRun = config.TestAssemblies.ToList();

            foreach (var filter in config.Filters.Where(f => f.Type == FilterType.Exclusive))
            {
                testAssembliesToRun.RemoveAll(a => a.Contains(filter.NameDll));
            }

            var watch = new Stopwatch();
            watch.Start();

            testAssembliesToRun.Each(assm => runResults.AddRange(manager.Run(assm, config.Filters.Where(f => f.Type != FilterType.Exclusive).Select(f => f.Name).ToList())));

            watch.Stop();

            Console.WriteLine("Test run completed in {0} seconds", watch.Elapsed.TotalSeconds);

            runResults.Each(result =>
                               {
                                   result.Messages.Each(m => listener.WriteLine(m, "Output"));
                                   result.TestResults.Each(listener.AddTestSummary);
                               });

            listener.DisplayResults();

            LastRunResults.GilesTestListener = listener;
        }
    }
}