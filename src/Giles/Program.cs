using System;
using System.Collections.Generic;
using Giles.Core.Watchers;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace Giles
{
    class Program
    {
        static string solutionPath;
        static string testAssemblyPath;

        static void Main(string[] args)
        {
            Console.WriteLine("Giles - your own personal watcher");

            solutionPath = args[0];
            testAssemblyPath = args[1];
            //solutionPath = @"D:\Dev\Prototypes\TestableTestStuff\TestableTestStuff.sln";
            //testAssemblyPath = @"D:\Dev\Prototypes\TestableTestStuff\ClassThatDoesShit.Tests\bin\Debug\Teh.Tests.dll";

            var kernel = new StandardKernel(new SlayerModule(solutionPath, testAssemblyPath));

            var sourceWatcher = kernel.Get<SourceWatcher>();

            sourceWatcher.Watch(solutionPath, @"*.cs");

            // put in the console watcher here for commands
            while (true)
            {
                
            }
        }
    }
}
