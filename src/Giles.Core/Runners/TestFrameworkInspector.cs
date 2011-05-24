using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Giles.Core.Runners
{
    /// <summary>
    /// A Giles Inspector has two purposes:
    /// 1) Provide a method to determine if the target assembly meets the requirements to be a valid test assembly for
    ///    the runner (i.e. - requiring that the assembly has a reference to nunit.framework)
    /// 2) Provide a method to get an instance of the Giles runner to run the tests in the target assembly
    /// </summary>
    public abstract class TestFrameworkInspector
    {
        public abstract Func<AssemblyName, bool> Requirement { get; }
        public abstract Func<IFrameworkRunner> Get { get; }

        public static IFrameworkRunner GetRunnerBy(string runnerAssemblyName)
        {
            var assemblyLocation =
                Path.Combine(Path.GetDirectoryName(typeof(TestFrameworkResolver).Assembly.Location),
                             runnerAssemblyName);

            var runner = GetRunnerFrom(assemblyLocation);

            if (runner == null)
                return null;

            return Activator.CreateInstance(runner) as IFrameworkRunner;
        }

        public static Type GetRunnerFrom(string assemblyLocation)
        {
            var result = Assembly.LoadFrom(assemblyLocation).GetTypes()
                .Where(x => typeof(IFrameworkRunner).IsAssignableFrom(x) && x.IsClass)
                .FirstOrDefault();
            return result;
        }    
    }
}