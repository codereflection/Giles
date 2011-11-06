using System.IO;
using System.Threading;
using Giles.Core.Configuration;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.Watchers;
using Giles.Specs.Core.Utility;
using Machine.Specifications;
using NSubstitute;

namespace Giles.Specs.Core.Watchers
{
    public class with_a_source_watcher
    {
        protected static SourceWatcher watcher;
        protected static IFileSystem fileSystem;
        protected static string path;
        protected static string filter;
        protected static IBuildRunner buildRunner;
        protected static FakeFileSystemWatcher fileSystemWatcher;
        protected static IFileWatcherFactory fileWatcherFactory;
        protected static string solutionfolder;
        protected static GilesConfig config;
        protected static GilesTestListener listener;

        Establish context = () =>
            {
                fileSystem = Substitute.For<IFileSystem>();
                buildRunner = Substitute.For<IBuildRunner>();
                fileWatcherFactory = Substitute.For<IFileWatcherFactory>();

                config = new GilesConfig();

                watcher = new SourceWatcher(buildRunner, fileSystem, fileWatcherFactory, config);

                path = @"c:\solutionFolder\mySolution.sln";
                filter = "*.cs";

                fileSystem.FileExists(path).Returns(false);
                fileSystemWatcher = new FakeFileSystemWatcher(".");

                fileWatcherFactory.Build(path, filter, null, null, null).ReturnsForAnyArgs(fileSystemWatcher);
                solutionfolder = @"c:\solutionFolder";
                fileSystem.GetDirectoryName(path)
                            .Returns(solutionfolder);

                listener = Substitute.For<GilesTestListener>(config);
                watcher.GetListener = c => listener;
            };
    }

    public class when_starting_to_watch_files : with_a_source_watcher
    {
        Because of = () =>
            watcher.Watch(path, filter);

        It setups_a_file_watcher = () =>
            fileWatcherFactory.Received().Build(solutionfolder, filter, Arg.Any<FileSystemEventHandler>(), Arg.Any<FileSystemEventHandler>(), Arg.Any<ErrorEventHandler>());

        It stores_a_reference_to_the_watcher = () =>
            watcher.FileWatchers.ShouldNotBeEmpty();

        It gets_the_solution_base_path = () =>
            fileSystem.Received().GetDirectoryName(path);
    }


    public class when_a_file_has_changed : with_a_source_watcher
    {
        Establish context = () => 
            watcher.Watch(path, filter);

        Because of = () =>
            watcher.ChangeAction(null, null);

        It calls_the_solution_builder = () =>
            {
                Thread.Sleep((int) config.BuildDelay + 100);
                buildRunner.Received().Run();
            };

        It should_display_the_results = () => 
            listener.Received().DisplayResults();

    }   

    public class when_disposing : with_a_source_watcher
    {
        Establish context = () =>
            watcher.Watch(path, filter);

        Because of = () =>
            watcher.Dispose();

        It should_call_dispose_on_each_file_watcher = () =>
            fileSystemWatcher.WasDisposed.ShouldBeTrue();
    }
}