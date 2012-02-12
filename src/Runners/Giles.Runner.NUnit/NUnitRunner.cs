using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Giles.Core.Runners;
using NUnit.Core;
using NUnit.Core.Filters;

namespace Giles.Runner.NUnit
{
    public class NUnitRunner : IFrameworkRunner
    {
        IEnumerable<string> filters;

        public SessionResults RunAssembly(Assembly assembly, IEnumerable<string> filters)
        {
            this.filters = filters;
            var remoteTestRunner = new RemoteTestRunner(0);
            var package = SetupTestPackager(assembly);
            remoteTestRunner.Load(package);
            var listener = new GilesNUnitEventListener();
            if (filters.Count() == 0)
                remoteTestRunner.Run(listener);
            else
                remoteTestRunner.Run(listener, GetFilters());
            return listener.SessionResults;
        }

        ITestFilter GetFilters()
        {
            var simpleNameFilter = new SimpleNameFilter(filters.ToArray());
            return simpleNameFilter;
        }

        public IEnumerable<string> RequiredAssemblies()
        {
            return new[]
                       {
                           Assembly.GetAssembly(typeof(NUnitRunner)).Location, 
                           "nunit.core.dll", "nunit.core.interfaces.dll"
                       };
        }

        private static TestPackage SetupTestPackager(Assembly assembly)
        {
            return new TestPackage(assembly.FullName, new[] { assembly.Location });
        }
    }
}