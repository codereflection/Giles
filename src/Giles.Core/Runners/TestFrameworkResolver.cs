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
        readonly List<TestFrameworkInspector> frameworkRunners = new List<TestFrameworkInspector>();

        public TestFrameworkResolver()
        {
            frameworkRunners.AddRange(GetNewInstancesByType<TestFrameworkInspector>());
        }

        /// <summary>
        /// Inspects the passed target test assembly to see if it meets all of the requirements for
        /// and of the Giles test framework runners.
        /// </summary>
        /// <param name="targetAssembly">Target test assembly to inspect</param>
        /// <returns>A list of test framework runners which can run the tests in the target assembly</returns>
        public IEnumerable<IFrameworkRunner> Resolve(Assembly targetAssembly)
        {
            if (targetAssembly == null)
                return Enumerable.Empty<IFrameworkRunner>();

            var referencedAssemblies = targetAssembly.GetReferencedAssemblies();

            var runners =
                frameworkRunners
                    .Where(theRunner => referencedAssemblies.AnyMatchesRequirementFor(theRunner.Requirement))
                    .Select(AnInstanceOfTheTestRunner);

            return runners;
        }

        static IFrameworkRunner AnInstanceOfTheTestRunner(TestFrameworkInspector theRunner)
        {
            return GetRunnerBy(theRunner.GetType().Assembly.Location);
        }


        /// <summary>
        /// Gets a list of new instances of classes which implement, extend or are of type T
        /// from all of the assemblies in the executing path
        /// </summary>
        /// <returns>List of new instances of classes which implement, extend or are of type T</returns>
        public static IEnumerable<T> GetNewInstancesByType<T>() where T : class
        {
            var files = AssemblyExtensions.GetAssembliesFromExecutingPath();

            var result = new List<T>();

            files.Each(f =>
                result.AddRange(AssemblyExtensions.FromAssemblyGetInstancesOfType<T>(f)));

            return result;
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