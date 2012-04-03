using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Giles.Core.Configuration;
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
        static bool quitRequested;
        static IList<InteractiveMenuOption> menuOptions;
        static StandardKernel kernel;

        static void Main(string[] args)
        {
            var options = new CLOptions();

            Console.WriteLine(GetGilesFunnyLine());

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var parser = new CommandLineParser(
                new CommandLineParserSettings(false, Console.Error));

            if (!parser.ParseArguments(args, options))
            {
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

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Critical Error: {0}\nTerminating: {1}", e.ExceptionObject, e.IsTerminating);
        }

        static GilesConfig GetGilesConfigFor(CLOptions options)
        {
            var solutionPath = options.SolutionPath.Replace("\"", string.Empty);
            var testAssemblies = GetTestAssemblies(options);

            solutionPath = Path.GetFullPath(solutionPath);
            for (var i = 0; i < testAssemblies.Count() - 1; i++)
            {
                testAssemblies[i] = Path.GetFullPath(testAssemblies[i]);
            }

            return SetupGilesConfig(solutionPath, testAssemblies);
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

        static SourceWatcher StartSourceWatcher()
        {
            var watcher = kernel.Get<SourceWatcher>();

            // HACK: Only *.cs files? Really? 
            watcher.Watch(config.SolutionPath, @"*.cs");
            return watcher;
        }

        static List<string> GetTestAssemblies(CLOptions options)
        {
            var testAssemblies = options.GetTestAssemblies();

            testAssemblies = testAssemblies.Count > 0 ?
                testAssemblies :
                FindTestAssembly(options.SolutionPath);

            if (testAssemblies == null)
            {
                Console.Error.Write(options.GetUsage());
                Console.Error.WriteLine("No test assemblies detected. Please specify"
                    + " the TestAssemblyPaths command line option.");
                Console.Error.WriteLine();
                Environment.Exit(1);
            }
            return testAssemblies;
        }

        private static List<string> FindTestAssembly(string solutionPath)
        {
            var finder = new TestAssemblyFinder();
            var assemblies = finder.FindTestAssembliesIn(solutionPath);

            return assemblies;
        }

        static GilesConfig SetupGilesConfig(string solutionPath, List<string> testAssemblies)
        {

            var builder = new GilesConfigBuilder(solutionPath, testAssemblies);
            return builder.Build();
        }

        static void ConsoleSetup()
        {
            Console.Clear();
            Console.Title = GetGilesFunnyLine();
            Console.WriteLine("Giles - your own personal watcher");
            Console.WriteLine("\t\"I'd like to test that theory...\"\n\n");
        }

        static InteractiveMenuOption[] GetInteractiveMenuOptions()
        {
            return new[]
                  {
                      new InteractiveMenuOption { HandlesKey = key => key == "?", Task = DisplayInteractiveMenuOptions },
                      new InteractiveMenuOption { HandlesKey = key => key == "i", Task = DisplayConfig },
                      new InteractiveMenuOption { HandlesKey = key => key == "c", Task = Console.Clear },
                      new InteractiveMenuOption { HandlesKey = key => key == "r", Task = sourceWatcher.RunNow },
                      new InteractiveMenuOption { HandlesKey = key => key == "b", Task = SetBuildDelay },
                      new InteractiveMenuOption { HandlesKey = key => key == "q", Task = RequestQuit },
                      new InteractiveMenuOption { HandlesKey = key => key == "v", Task = DisplayVerboseResults },
                      new InteractiveMenuOption { HandlesKey = key => key == "e", Task = DisplayErrors },
                      new InteractiveMenuOption { HandlesKey = key => key == "f", Task = SetTestFilters },
                      new InteractiveMenuOption { HandlesKey = key => key == "h", Task = ClearTestFilters },
                  };
        }


        static void MainFeedbackLoop()
        {
            while (!quitRequested)
            {
                try
                {
                    var keyValue = Console.ReadKey(true).KeyChar.ToString().ToLower();

                    menuOptions
                        .Where(option => option.HandlesKey(keyValue))
                        .Each(option => option.Task());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e);
                }
            }
            Console.WriteLine("Until next time...");
        }

        static void RequestQuit()
        {
            quitRequested = true;
        }

        static void SetTestFilters()
        {
            config.Filters = UserInputHandler.GetUserValuesFor(config.Filters, "Filters: Enter a namespace and type (MyNamespace.FooTests).");
            Console.WriteLine("Filters set to:");
            config.Filters.Each(x => Console.WriteLine("\t{0}", x));
        }

        static void ClearTestFilters()
        {
            config.Filters = new List<string>();
            Console.WriteLine("Test filters cleared");
        }

        static void SetBuildDelay()
        {
            config.BuildDelay = GetUserValue("Build Delay", config.BuildDelay);
        }

        static void DisplayErrors()
        {
            if (LastRunResults.GilesTestListener != null)
                LastRunResults.GilesTestListener.DisplayErrors();
            else
                Console.WriteLine("Please run some tests first...");
        }

        static void DisplayVerboseResults()
        {
            if (LastRunResults.GilesTestListener != null)
                LastRunResults.GilesTestListener.DisplayVerboseResults();
            else
                Console.WriteLine("Please run some tests first...");
        }


        static T GetUserValue<T>(string description, T defaultValue)
        {
            Console.Write("Enter new value for {0} ({1}): ", description, defaultValue);
            var newValue = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newValue))
                return defaultValue;

            return (T)Convert.ChangeType(newValue, typeof(T));
        }

        static void DisplayConfig()
        {
            Console.WriteLine("\nCurrent Configuration");
            Console.WriteLine("  Build Delay: {0}", config.BuildDelay);
            Console.WriteLine("  Solution: {0}", config.SolutionPath);
            Console.WriteLine("  Test Assemblies: \n\t{0}", GetTestAssemblyListAsString());
            config.TestRunners.Each(r => Console.WriteLine("  {0} Has been enabled", r.Key));
            Console.WriteLine("  Test Filters: \n\t{0}", GetTestFilterListAsString());
            Console.WriteLine();
        }

        static string GetTestAssemblyListAsString()
        {
            return !config.TestAssemblies.Any() ? 
                "No test assemblies detected." : 
                config.TestAssemblies.Aggregate((next, working) => working + Environment.NewLine + "\t" + next);
        }

        static string GetTestFilterListAsString()
        {
            return !config.Filters.Any()
                ? "<All Classes>" 
                : config.Filters.Aggregate((next, working) => working + Environment.NewLine + "\t" + next);
        }

        static void DisplayInteractiveMenuOptions()
        {
            Console.WriteLine("Interactive Console Options:");
            Console.WriteLine("   ? = Display options");
            Console.WriteLine("   C = Clear the window");
            Console.WriteLine("   I = Show current configuration");
            Console.WriteLine("   R = Run build & tests now");
            Console.WriteLine("   V = Display all messages from last test run");
            Console.WriteLine("   E = Display errors from last test run");
            Console.WriteLine("   B = Set Build Delay");
            Console.WriteLine("   F = Set Test Filters");
            Console.WriteLine("   H = Clear Test Filters");
            Console.WriteLine("   Q = Quit");
            Console.WriteLine();
        }
    }

    public class InteractiveMenuOption
    {
        public Func<string, bool> HandlesKey;
        public Action Task;
    }
}
