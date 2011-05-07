using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Giles.Core.Configuration;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.Utility;

namespace Giles.Core.Watchers
{
    public class SourceWatcher : IDisposable
    {
        readonly IBuildRunner buildRunner;
        readonly Timer buildTimer;
        readonly IFileSystem fileSystem;
        readonly IFileWatcherFactory fileWatcherFactory;
        readonly GilesConfig config;
        readonly ITestRunnerResolver testRunnerResolver;
        readonly ITestRunner testRunner;


        public SourceWatcher(IBuildRunner buildRunner, ITestRunner testRunner, IFileSystem fileSystem,
                             IFileWatcherFactory fileWatcherFactory, GilesConfig config, ITestRunnerResolver testRunnerResolver)
        {
            FileWatchers = new List<FileSystemWatcher>();
            this.fileSystem = fileSystem;
            this.buildRunner = buildRunner;
            this.fileWatcherFactory = fileWatcherFactory;
            this.config = config;
            this.testRunnerResolver = testRunnerResolver;
            this.testRunner = testRunner;
            buildTimer = new Timer {AutoReset = false, Enabled = false, Interval = config.BuildDelay};
            config.PropertyChanged += config_PropertyChanged;
            buildTimer.Elapsed += buildTimer_Elapsed;
        }

        void config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BuildDelay")
                buildTimer.Interval = (sender as GilesConfig).BuildDelay;
        }

        public List<FileSystemWatcher> FileWatchers { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            FileWatchers.ToList().ForEach(x => x.Dispose());
        }

        #endregion

        void buildTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunNow();
        }

        public void Watch(string solutionPath, string filter)
        {
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
            if (!buildRunner.Run())
                return;
            var testFrameworkRunner = testRunnerResolver.Resolve(config.TestAssembly).ToList();

            var listener = new GilesTestListener(config);

            testFrameworkRunner.ForEach(x =>
                                            {
                                                var sessionResult = x.RunAssembly(config.TestAssembly);
                                                sessionResult.Messages.Each(m => listener.WriteLine(m, "Output"));
                                                sessionResult.TestResults.Each(listener.AddTestSummary);
                                            });


            //var outputs = listener.GetOutput().ToList();

            //outputs.ForEach(x => Console.WriteLine("Category {0}\n{1}", x.Key, x.Value.ToString()));
            listener.DisplayResults();
        }
    }
}