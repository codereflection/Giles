using System;
using Giles.Core.Watchers;
using Ninject;

namespace Giles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Giles - your own personal watcher");

            Console.ReadKey();

            var kernel = new StandardKernel();
            var sourceWatcher = kernel.Get<SourceWatcher>();

            sourceWatcher.Watch(@"D:\Dev\Prototypes\TestableTestStuff", @"*.cs");
        }
    }
}
