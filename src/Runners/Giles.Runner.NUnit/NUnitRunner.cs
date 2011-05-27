using System;
using System.Collections.Generic;
using System.Reflection;
using Giles.Core.Runners;
using NUnit.Core;

namespace Giles.Runner.NUnit
{
    public class NUnitRunner : IFrameworkRunner
    {
        public SessionResults RunAssembly(Assembly assembly)
        {
            var remoteTestRunner = new RemoteTestRunner(0);
            var package = SetupTestPackager(assembly);
            remoteTestRunner.Load(package);
            var listener = new GilesNUnitEventListener();
            remoteTestRunner.Run(listener);
            return listener.SessionResults;
        }

        public IEnumerable<string> RequiredAssemblies()
        {
            return new[] { "nunit.core.dll", "nunit.core.interfaces.dll" };
        }

        private static TestPackage SetupTestPackager(Assembly assembly)
        {
            return new TestPackage(assembly.FullName, new[] { assembly.Location });
        }
    }
}