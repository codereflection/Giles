using System.IO;
using System.Linq;
using Giles.Core.IO;
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

        Establish context = () =>
                                {
                                    fileSystem = Substitute.For<IFileSystem>();
                                    watcher = new SourceWatcher(fileSystem);
                                    path = ".";
                                    filter = "*.cs";

                                    var fileSystemWatcher = new FileSystemWatcher
                                    {
                                        Path = path,
                                        Filter = filter
                                    };
                                    
                                    fileSystem.FileExists(path).Returns(false);
                                    fileSystem.SetupFileWatcher(path, filter, null, null, null, null, null)
                                              .Returns(fileSystemWatcher);
                                };

    }

    //public static void CreatedAction(object sender, FileSystemEventArgs e)
    //{

    //}

    public class when_starting_to_watch_files : with_a_source_watcher
    {
        Because of = () =>
            watcher.Watch(path, filter);

        It should_setup_a_file_watcher = () =>
            fileSystem.Received().SetupFileWatcher(path, filter, null, null, null, null, null);

        It should_store_a_reference_to_the_watcher = () =>
            watcher.FileWatchers.ShouldNotBeEmpty();

        It should_have_a_file_watcher_watching_the_correct_path = () =>
            watcher.FileWatchers.First().Path.ShouldEqual(path);

        It should_have_a_file_watcher_watching_the_correct_filter = () =>
            watcher.FileWatchers.First().Filter.ShouldEqual(filter);
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