using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Giles.Core.Runners
{
    public class TestRunnerResolver
    {
        readonly Func<AssemblyName, bool> mSpecRunnerPredicate =
            assemblyName => assemblyName.Name.Equals("Machine.Specifications", StringComparison.InvariantCultureIgnoreCase);

        readonly Func<AssemblyName, bool> nUnitRunnerPredicate =
            assemblyName => assemblyName.Name.Equals("nunit.framework", StringComparison.InvariantCultureIgnoreCase);

        readonly Func<AssemblyName, bool> xUnitRunnerPredicate =
            assemblyName => assemblyName.Name.Equals("xunit", StringComparison.InvariantCultureIgnoreCase);

        List<FrameworkRunnerLocator> runners;

        public TestRunnerResolver()
        {
            BuildRunnerList();
        }


        public IEnumerable<IFrameworkRunner> Resolve(Assembly assembly)
        {
            if (assembly == null)
                return Enumerable.Empty<IFrameworkRunner>();

            var referencedAssemblies = assembly.GetReferencedAssemblies();

            var result =
                runners.Where(runner => referencedAssemblies.Count(runner.CheckReference) > 0).Select(
                    runner => runner.GetTheRunner.Invoke());

            return result;
        }


        void BuildRunnerList()
        {
            var mspecFrameworkRunner = new FrameworkRunnerLocator { CheckReference = mSpecRunnerPredicate, GetTheRunner = GetMSpecRunner };
            var nunitFrameworkRunner = new FrameworkRunnerLocator { CheckReference = nUnitRunnerPredicate, GetTheRunner = GetNUnitRunner };
            var xunitFrameworkRunner = new FrameworkRunnerLocator { CheckReference = xUnitRunnerPredicate, GetTheRunner = GetXunitRunner };
            runners = new List<FrameworkRunnerLocator> { mspecFrameworkRunner, nunitFrameworkRunner, xunitFrameworkRunner };
        }

        static IFrameworkRunner GetMSpecRunner()
        {
            return GetRunnerBy("Giles.Runner.Machine.Specifications.dll");
        }

        static IFrameworkRunner GetNUnitRunner()
        {
            return GetRunnerBy("Giles.Runner.NUnit.dll");
        }

        static IFrameworkRunner GetXunitRunner() {
            return GetRunnerBy("Giles.Runner.XUnit.dll");
        }

        static IFrameworkRunner GetRunnerBy(string runnerAssemblyName)
        {
            var assemblyLocation =
                Path.Combine(Path.GetDirectoryName(typeof(TestRunnerResolver).Assembly.Location),
                             runnerAssemblyName);

            var runner = GetRunner(assemblyLocation);

            if (runner == null)
                return null;

            return Activator.CreateInstance(runner) as IFrameworkRunner;
        }

        static Type GetRunner(string assemblyLocation)
        {
            var result = Assembly.LoadFrom(assemblyLocation).GetTypes()
                .Where(x => typeof(IFrameworkRunner).IsAssignableFrom(x) && x.IsClass)
                .FirstOrDefault();
            return result;
        }
    }
}