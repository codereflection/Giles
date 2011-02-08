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
	    static IBuildRunner buildRunner;
	    static FileSystemWatcher fileSystemWatcher;
	    protected static IFileWatcherFactory fileWatcherFactory;
        protected static string solutionfolder;

	    Establish context = () =>
								{
									fileSystem = Substitute.For<IFileSystem>();
									buildRunner = Substitute.For<IBuildRunner>();
								    fileWatcherFactory = Substitute.For<IFileWatcherFactory>();
								    
                                    watcher = new SourceWatcher(fileSystem, buildRunner, fileWatcherFactory);
								    
                                    path = @"c:\solutionFolder\mySolution.sln";
								    filter = "*.cs";

								    fileSystem.FileExists(path).Returns(false);
                                    fileSystemWatcher = new FileSystemWatcher(".");

                                    fileWatcherFactory.Build(path, filter, null, null, null).ReturnsForAnyArgs(fileSystemWatcher);
								    solutionfolder = @"c:\solutionFolder\";
								    fileSystem.GetDirectoryName(path)
                                              .Returns(solutionfolder);
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
            fileWatcherFactory.Received().Build(solutionfolder, filter, Arg.Any<FileSystemEventHandler>(), Arg.Any<FileSystemEventHandler>(), Arg.Any<ErrorEventHandler>());

		It should_store_a_reference_to_the_watcher = () =>
			watcher.FileWatchers.ShouldNotBeEmpty();

        It should_get_the_solution_base_path = () =>
            fileSystem.Received().GetDirectoryName(path);
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