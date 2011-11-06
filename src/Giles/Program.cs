using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;
using Giles.Core.Configuration;
using Giles.Core.Utility;
using Giles.Core.Watchers;
using Giles.Options;
using Ninject;

namespace Giles {
    class Program {
        static SourceWatcher sourceWatcher;
        static GilesConfig config;
        static bool quitRequested;
        static IList<InteractiveMenuOption> menuOptions;
        static StandardKernel kernel;

        static void Main(string[] args) {
            var options = new CLOptions();

            Console.WriteLine(GetGilesFunnyLine());

            var parser = new CommandLineParser(
                new CommandLineParserSettings(false, Console.Error));

            if (!parser.ParseArguments(args, options)) {
                Console.WriteLine("Unable to determine what command lines arguments were used, check the help above!\nThe minimum needed is the -s [solution file path].");
                Environment.Exit(1);
            }

            config = GetGilesConfigFor(options);

            kernel = new StandardKernel(new SlayerModule(config));

            ConsoleSetup();

            sourceWatcher = StartSourceWatcher();

            menuOptions = GetInteractiveMenuOptions();

            DisplayInteractiveMenuOptions();

            MainFeedbackLoop();
        }

        static GilesConfig GetGilesConfigFor(CLOptions options)
        {
            var solutionPath = options.SolutionPath.Replace("\"", string.Empty);
            var testAssemblyPath = GetTestAssemblyPath(options);
            return SetupGilesConfig(solutionPath, testAssemblyPath);
        }

        static string GetGilesFunnyLine()
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            return string.Format(@"Grr, argh... v{0}.{1}.{2}.{3}",
                name.Version.Major,
                name.Version.Minor,
                name.Version.Revision,
                name.Version.Build);
        }

        static SourceWatcher StartSourceWatcher() {
            var watcher = kernel.Get<SourceWatcher>();

            // HACK: Only *.cs files? Really? 
            watcher.Watch(config.SolutionPath, @"*.cs");
            return watcher;
        }

        private static string GetTestAssemblyPath(CLOptions options)
        {
            var path = options.TestAssemblyPath != null
                ? options.TestAssemblyPath.Replace("\"", string.Empty)
                : FindTestAssembly(options.SolutionPath);

            if (path == null)
            {
                Console.Error.Write(options.GetUsage());
                Console.Error.WriteLine("No test assemblies detected. Please specify"
                    + " the TestAssemblyPath command line option.");
                Console.Error.WriteLine();
                Environment.Exit(1);
            }
            return path;
        }

        private static string FindTestAssembly(string solutionPath)
        {
            var finder = new TestAssemblyFinder();
            var assemblies = finder.FindTestAssembliesIn(solutionPath);

            return assemblies.Count() == 0 ? null : assemblies.First();
        }

        static GilesConfig SetupGilesConfig(string solutionPath, string testAssemblyPath) {

            var builder = new GilesConfigBuilder(solutionPath, testAssemblyPath);
            return builder.Build();
        }

        private static int Height {
            get {
                return (69 > Console.LargestWindowHeight) ? Console.LargestWindowHeight : 69;
            }
        }

        private static int Width {
            get {
                return (150 > Console.LargestWindowWidth) ? Console.LargestWindowWidth : 150; ;
            }
        }

        static void ConsoleSetup() {
            Console.Clear();
            Console.Title = GetGilesFunnyLine();
            //GilesConsoleWindowControls.SetConsoleWindowPosition(0, 75);
            //Console.SetBufferSize(1024, 5000);
            //Console.SetWindowSize(Width, Height);
            Console.WriteLine("Giles - your own personal watcher");
            Console.WriteLine("\t\"I'd like to test that theory...\"\n\n");
        }

        static InteractiveMenuOption[] GetInteractiveMenuOptions() {
            return new[]
                  {
                      new InteractiveMenuOption { HandlesKey = key => key == "?", Task = DisplayInteractiveMenuOptions },
                      new InteractiveMenuOption { HandlesKey = key => key == "i", Task = DisplayConfig },
                      new InteractiveMenuOption { HandlesKey = key => key == "c", Task = Console.Clear },
                      new InteractiveMenuOption { HandlesKey = key => key == "r", Task = sourceWatcher.RunNow },
                      new InteractiveMenuOption { HandlesKey = key => key == "b", Task = SetBuildDelay },
                      new InteractiveMenuOption { HandlesKey = key => key == "q", Task = RequestQuit },
                      new InteractiveMenuOption { HandlesKey = key => key == "v", Task = DisplayVerboseResults },
                      new InteractiveMenuOption { HandlesKey = key => key == "e", Task = DisplayErrors }
                  };
        }


        static void MainFeedbackLoop() {
            while (!quitRequested) {
                var keyValue = Console.ReadKey(true).KeyChar.ToString().ToLower();

                menuOptions
                    .Where(option => option.HandlesKey(keyValue))
                    .Each(option => option.Task());
            }
            Console.WriteLine("Until next time...");
        }

        static void RequestQuit() {
            quitRequested = true;
        }

        static void SetBuildDelay() {
            config.BuildDelay = GetUserValue(config.BuildDelay);
        }

        static void DisplayErrors()
        {
            if (LastRunResults.GilesTestListener != null)
                LastRunResults.GilesTestListener.DisplayErrors();
            else
                Console.WriteLine("Please run some tests first...");
        }

        static void DisplayVerboseResults() {
            if (LastRunResults.GilesTestListener != null)
                LastRunResults.GilesTestListener.DisplayVerboseResults();
            else
                Console.WriteLine("Please run some tests first...");
        }

        static T GetUserValue<T>(T defaultValue) {
            Console.Write("Enter new value ({0}): ", defaultValue);
            var newValue = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newValue))
                return defaultValue;

            return (T)Convert.ChangeType(newValue, typeof(T));
        }

        static void DisplayConfig() {
            Console.WriteLine("\nCurrent Configuration");
            Console.WriteLine("  Build Delay: " + config.BuildDelay);
            Console.WriteLine("  Solution: " + config.SolutionPath);
            Console.WriteLine("  Test Assembly: " + config.TestAssemblyPath);
            config.TestRunners.Each(r => Console.WriteLine("  " + r.Key + " Has been enabled"));
            Console.WriteLine();
        }

        static void DisplayInteractiveMenuOptions() {
            Console.WriteLine("Interactive Console Options:");
            Console.WriteLine("   ? = Display options");
            Console.WriteLine("   C = Clear the window");
            Console.WriteLine("   I = Show current configuration");
            Console.WriteLine("   R = Run build & tests now");
            Console.WriteLine("   V = Display all messages from last test run");
            Console.WriteLine("   E = Display errors from last test run");
            Console.WriteLine("   B = Set Build Delay");
            Console.WriteLine("   Q = Quit");
            Console.WriteLine();
        }
    }

    public class InteractiveMenuOption {
        public Func<string, bool> HandlesKey;
        public Action Task;
    }
}
