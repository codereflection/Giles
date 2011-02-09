using System;
using System.Collections.Generic;
using Giles.Core.Watchers;
using Ninject;
using Ninject.Modules;

namespace Giles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Giles - your own personal watcher");

            var kernel = new StandardKernel(new SlayerModule());
            var sourceWatcher = kernel.Get<SourceWatcher>();

            sourceWatcher.Watch(@"D:\Dev\Prototypes\TestableTestStuff\TestableTestStuff.sln", @"*.cs");

            // put in the console watcher here for commands
            while (true)
            {
                
            }
        }
    }
}
