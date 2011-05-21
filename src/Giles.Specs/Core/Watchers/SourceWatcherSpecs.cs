using System.IO;
using System.Linq;
using Giles.Core.Configuration;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.Watchers;
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
        protected static FileSystemWatcher fileSystemWatcher;
        protected static IFileWatcherFactory fileWatcherFactory;
        protected static string solutionfolder;
        protected static ITestRunner testRunner;
        static GilesConfig config;
        static TestRunnerResolver resolver;

        Establish context = () =>
            {
                fileSystem = Substitute.For<IFileSystem>();
                buildRunner = Substitute.For<IBuildRunner>();
                testRunner = Substitute.For<ITestRunner>();
                fileWatcherFactory = Substitute.For<IFileWatcherFactory>();

                config = new GilesConfig();

                watcher = new SourceWatcher(buildRunner, testRunner, fileSystem, fileWatcherFactory, config);
                                    
                path = @"c:\solutionFolder\mySolution.sln";
                filter = "*.cs";

                fileSystem.FileExists(path).Returns(false);
                fileSystemWatcher = new FileSystemWatcher(".");

                fileWatcherFactory.Build(path, filter, null, null, null).ReturnsForAnyArgs(fileSystemWatcher);
                solutionfolder = @"c:\solutionFolder";
                fileSystem.GetDirectoryName(path)
                            .Returns(solutionfolder);
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

        [Ignore("build is called on a delay, need a better way to test this")]
        It calls_the_solution_builder = () =>
            buildRunner.Received().Run();
    }

    public class when_disposing : with_a_source_watcher
    {
        Establish context = () =>
            watcher.Watch(path, filter);

        Because of = () =>
            watcher.Dispose();

        It should_call_dispose_on_each_file_watcher = () =>
            watcher.FileWatchers.All(x => x == null);
    }
}