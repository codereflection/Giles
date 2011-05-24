using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Giles.Core.Runners
{
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

    public class MSpecTestFrameworkInspector : TestFrameworkInspector
    {
        public override Func<AssemblyName, bool> Requirement
        {
            get
            {
                return assemblyName =>
                    assemblyName.Name.Equals("Machine.Specifications", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public override Func<IFrameworkRunner> Get
        {
            get { return () => GetRunnerBy("Giles.Runner.Machine.Specifications.dll"); }
        }
    }

    public class NUnitTestFrameworkInspector : TestFrameworkInspector
    {
        public override Func<AssemblyName, bool> Requirement
        {
            get
            {
                return assemblyName =>
                    assemblyName.Name.Equals("nunit.framework", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public override Func<IFrameworkRunner> Get
        {
            get { return () => GetRunnerBy("Giles.Runner.NUnit.dll"); }
        }
    }
}