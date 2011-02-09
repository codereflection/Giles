using System;
using Giles.Core.Watchers;
using Ninject;

namespace Giles
{
    class Program
    {
        static string solutionPath;
        static string testAssemblyPath;

        static void Main(string[] args)
        {
            Console.Clear();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            Console.WriteLine("Giles - your own personal watcher");

            solutionPath = args[0];
            testAssemblyPath = args[1];
            //solutionPath = @"D:\Dev\Prototypes\TestableTestStuff\TestableTestStuff.sln";
            //testAssemblyPath = @"D:\Dev\Prototypes\TestableTestStuff\ClassThatDoesShit.Tests\bin\Debug\Teh.Tests.dll";

            var kernel = new StandardKernel(new SlayerModule(solutionPath, testAssemblyPath));

            var sourceWatcher = kernel.Get<SourceWatcher>();

            sourceWatcher.Watch(solutionPath, @"*.cs");

            DisplayOptions();
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

                if (keyValue == "q")
                    break;
            }
            Console.WriteLine("See you next time...");

        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // eating the ctrl-c for breakfast...
            e.Cancel = true;
        }

        static void DisplayConfig()
        {
            Console.WriteLine("Current Configuration");
            Console.WriteLine("  Solution: " + solutionPath);
            Console.WriteLine("  Test Assembly: " + testAssemblyPath);
            Console.WriteLine();
        }

        static void DisplayOptions()
        {
            Console.WriteLine("Interactive Console Options:");
            Console.WriteLine("  ? = Display options");
            Console.WriteLine("  C = Clear the window");
            Console.WriteLine("  I = Show current configuration");
            Console.WriteLine("  Q = Quit");
            Console.WriteLine();
        }
    }
}
