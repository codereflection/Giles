using System;
using CommandLine;
using Giles.Core.Configuration;
using Giles.Core.Watchers;
using Giles.Options;
using Ninject;

namespace Giles
{
    class Program
    {
        static SourceWatcher sourceWatcher;
        static GilesConfig config;

        static void Main(string[] args)
        {

            var options = new CLOptions();

            var parser = new CommandLineParser(
                new CommandLineParserSettings(false,Console.Error));

            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);

            Console.Clear();
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine("Giles - your own personal watcher");

            var solutionPath = options.SolutionPath;
            var testAssemblyPath = options.TestAssemblyPath;
            var projectRoot = options.ProjectRoot;

            var kernel = new StandardKernel(new SlayerModule(solutionPath, testAssemblyPath, projectRoot));

            var configFactory = kernel.Get<GilesConfigFactory>();
            config = configFactory.Build();
            
            sourceWatcher = kernel.Get<SourceWatcher>();

            sourceWatcher.Watch(solutionPath, @"*.cs");

            DisplayOptions();
            
            MainFeedbackLoop();

            Console.WriteLine("See you next time...");

        }

        static void MainFeedbackLoop()
        {
            while (true)
            {
                
                var key = Console.ReadKey(true);

                var keyValue = key.KeyChar.ToString().ToLower();

                if (keyValue == "?")
                    DisplayOptions();

                if (keyValue == "i")
                    DisplayConfig();

                if (keyValue == "c")
                    Console.Clear();

                if (keyValue == "r")
                    sourceWatcher.RunNow();

                if (keyValue == "b")
                    config.BuildDelay = GetUserValue(config.BuildDelay);

                if (keyValue == "q")
                    break;
            }
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

        static void DisplayOptions()
        {
            Console.WriteLine("Interactive Console Options:");
            Console.WriteLine("  ? = Display options");
            Console.WriteLine("  C = Clear the window");
            Console.WriteLine("  I = Show current configuration");
            Console.WriteLine("  R = Run build & tests now");
            Console.WriteLine("  B = Set Build Delay");
            Console.WriteLine("  Q = Quit");
            Console.WriteLine();
        }
    }
}
