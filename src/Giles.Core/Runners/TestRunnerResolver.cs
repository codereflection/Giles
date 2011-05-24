using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Giles.Core.Utility;

namespace Giles.Core.Runners
{
    public class TestFrameworkResolver
    {
        readonly List<TestFrameworkInspector> supportedFrameworkRunners = new List<TestFrameworkInspector>();

        public TestFrameworkResolver()
        {
            BuildRunnerList();
        }

        public IEnumerable<IFrameworkRunner> Resolve(Assembly assembly)
        {
            if (assembly == null)
                return Enumerable.Empty<IFrameworkRunner>();

            var referencedAssemblies = assembly.GetReferencedAssemblies();

            var runners =
                supportedFrameworkRunners
                    .Where(theRunner => referencedAssemblies.AnyMatchesRequirementFor(theRunner.Requirement))
                    .Select(AnInstanceOfTheTestRunner);

            return runners;
        }

        static IFrameworkRunner AnInstanceOfTheTestRunner(TestFrameworkInspector theRunner)
        {
            return GetRunnerBy(theRunner.GetType().Assembly.Location);
        }


        void BuildRunnerList()
        {
            var files = GetAssembliesFromExecutingPath();

            files.Each(AddTestFrameworkInspectorsFromAssembly);
        }

        void AddTestFrameworkInspectorsFromAssembly(string file)
        {
            var assembly = Assembly.LoadFrom(file);
            var types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(TestFrameworkInspector))).ToList();

            if (types.Count == 0)
                return;

            types.Each(type => supportedFrameworkRunners.Add(Activator.CreateInstance(type) as TestFrameworkInspector));
        }

        static IEnumerable<string> GetAssembliesFromExecutingPath()
        {
            var location = Path.GetDirectoryName(typeof(TestFrameworkResolver).Assembly.Location);

            return Directory.GetFiles(location, "*.dll");
        }

        /// <summary>
        /// Returns an instance of the first test framework runner that implements IFrameworkRunner found in the runnerAssemblyPath
        /// </summary>
        /// <param name="runnerAssemblyPath">Filename to load the assembly from</param>
        /// <returns>An instance of the first class found that implements IFrameworkRunner</returns>
        public static IFrameworkRunner GetRunnerBy(string runnerAssemblyPath)
        {
            var assemblyLocation =
                Path.Combine(Path.GetDirectoryName(typeof(TestFrameworkResolver).Assembly.Location),
                             runnerAssemblyPath);

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