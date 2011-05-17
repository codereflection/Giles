using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Giles.Core.Configuration;
using Giles.Core.IO;
using Giles.Core.Utility;
using Giles.Core.Watchers;
using Giles.Options;
using Ninject;

namespace Giles
{
    class Program
    {
        static SourceWatcher sourceWatcher;
        static GilesConfig config;
        static bool QuitRequested = false;

        static void Main(string[] args)
        {
            var options = new CLOptions();

            var parser = new CommandLineParser(
                new CommandLineParserSettings(false,Console.Error));

            if (!parser.ParseArguments(args, options))
            {
                Console.WriteLine("Unknown command line arguments");
                Environment.Exit(1);
            }

            ConsoleSetup();

            SetupSourceWatcher(options);

            SetupInteractiveMenuOptions();

            DisplayInteractiveMenuOptions();
            
            MainFeedbackLoop();

            Console.WriteLine("Grr, argh...");

        }

        static void SetupSourceWatcher(CLOptions options)
        {
            var solutionPath = options.SolutionPath.Replace("\"", string.Empty);
            var testAssemblyPath = GetTestAssemblyPath(options);

            GetSourceWatcher(solutionPath, testAssemblyPath);

            sourceWatcher.Watch(solutionPath, @"*.cs");
        }

        private static string GetTestAssemblyPath(CLOptions options)
        {
            var testAssemblyPath = options.TestAssemblyPath != null
                ? options.TestAssemblyPath.Replace("\"", string.Empty)
                : FindTestAssembly(options);

            if (testAssemblyPath == null)
            {
                Console.Error.Write(options.GetUsage());
                Console.Error.WriteLine("No test assemblies detected. Please specify"
                    + " the TestAssemblyPath command line option.");
                Console.Error.WriteLine();
                Environment.Exit(1);
            }
            return testAssemblyPath;
        }

        private static string FindTestAssembly(CLOptions options)
        {
            var testAssemblyFinder = new TestAssemblyFinder(new FileSystem());
            var testAssemblies = testAssemblyFinder.FindTestAssembliesIn(options.SolutionPath);

            if (testAssemblies.Count() == 0)
                return null;

            return testAssemblies.First();
        }

        static void GetSourceWatcher(string solutionPath, string testAssemblyPath)
        {
            var kernel = SetupGilesKernelAndConfig(solutionPath, testAssemblyPath);

            sourceWatcher = kernel.Get<SourceWatcher>();
        }

        static StandardKernel SetupGilesKernelAndConfig(string solutionPath, string testAssemblyPath)
        {
            var kernel = new StandardKernel(new SlayerModule(solutionPath, testAssemblyPath));

            var configFactory = kernel.Get<GilesConfigFactory>();
            config = configFactory.Build();
            return kernel;
        }

        static void ConsoleSetup()
        {
            Console.Clear();
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine("Giles - your own personal watcher");
            Console.WriteLine("\t\"I'd like to test that theory...\"\n\n");
        }


        static IList<InteractiveMenuOption> InteractiveMenuOptions;

        static void SetupInteractiveMenuOptions()
        {
            InteractiveMenuOptions = new[]
                  {
                      new InteractiveMenuOption { HandlesKey = key => key == "?", Task = DisplayInteractiveMenuOptions },
                      new InteractiveMenuOption { HandlesKey = key => key == "i", Task = DisplayConfig },
                      new InteractiveMenuOption { HandlesKey = key => key == "c", Task = Console.Clear },
                      new InteractiveMenuOption { HandlesKey = key => key == "r", Task = sourceWatcher.RunNow },
                      new InteractiveMenuOption { HandlesKey = key => key == "b", Task = SetBuildDelay },
                      new InteractiveMenuOption { HandlesKey = key => key == "q", Task = RequestQuit },
                      new InteractiveMenuOption { HandlesKey = key => key == "v", Task = DisplayVerboseResults }
                  };
        }


        static void MainFeedbackLoop()
        {
            while (!QuitRequested)
            {
                var keyValue = Console.ReadKey(true).KeyChar.ToString().ToLower();

                InteractiveMenuOptions
                    .Where(option => option.HandlesKey(keyValue))
                    .Each(option => option.Task());                
            }
        }

        static void RequestQuit()
        {
            QuitRequested = true;
        }

        static void SetBuildDelay()
        {
            config.BuildDelay = GetUserValue(config.BuildDelay);
        }

        static void DisplayVerboseResults()
        {
            if (LastRunResults.GilesTestListener != null)
                LastRunResults.GilesTestListener.DisplayVerboseResults();
            else
                Console.WriteLine("Please run some tests first...");
        }

        static T GetUserValue<T>(T defaultValue)
        {
            Console.Write("Enter new value ({0}): ", defaultValue);
            var newValue = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newValue))
                return defaultValue;
            
            return (T)Convert.ChangeType(newValue, typeof(T));
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // eating the ctrl-c for breakfast...
            e.Cancel = true;
        }

        static void DisplayConfig()
        {
            Console.WriteLine("\nCurrent Configuration");
            Console.WriteLine("  Build Delay: " + config.BuildDelay);
            Console.WriteLine("  Solution: " + config.SolutionPath);
            Console.WriteLine("  Project Root: " + config.ProjectRoot);
            Console.WriteLine("  Test Assembly: " + config.TestAssemblyPath);
            Console.WriteLine();
        }

        static void DisplayInteractiveMenuOptions()
        {
            Console.WriteLine("Interactive Console Options:");
            Console.WriteLine("  ? = Display options");
            Console.WriteLine("  C = Clear the window");
            Console.WriteLine("  I = Show current configuration");
            Console.WriteLine("  R = Run build & tests now");
            Console.WriteLine("  V = Display last test run messages");
            Console.WriteLine("  B = Set Build Delay");
            Console.WriteLine("  Q = Quit");
            Console.WriteLine();
        }
    }

    public class InteractiveMenuOption
    {
        public Func<string, bool> HandlesKey;
        public Action Task;
    }
}
