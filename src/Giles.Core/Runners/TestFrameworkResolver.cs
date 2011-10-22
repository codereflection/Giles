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
        readonly List<TestFrameworkInspector> frameworkInspectors = new List<TestFrameworkInspector>();

        public TestFrameworkResolver()
        {
            var inspectors = TypeLoader.GetNewInstancesByType<TestFrameworkInspector>();
            frameworkInspectors.AddRange(inspectors);
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
                frameworkInspectors
                    .Where(inspector => referencedAssemblies.AnyMatchesRequirementFor(inspector.Requirement))
                    .Select(AnInstanceOfTheTestRunner);

            return runners;
        }

        static IFrameworkRunner AnInstanceOfTheTestRunner(TestFrameworkInspector theRunner)
        {
            return GetRunnerBy(theRunner.GetType().Assembly.Location);
        }

        /// <summary>
        /// Returns an instance of the first test framework runner that implements IFrameworkRunner found in the runnerAssemblyPath
        /// </summary>
        /// <param name="runnerAssemblyPath">Filename to load the assembly from</param>
        /// <returns>An instance of the first class found that implements IFrameworkRunner</returns>
        public static IFrameworkRunner GetRunnerBy(string runnerAssemblyPath)
        {
            var codeBase = new Uri(typeof(TestFrameworkResolver).Assembly.CodeBase);
            var basePath = Path.GetDirectoryName(codeBase.LocalPath);
            var assemblyLocation = Path.Combine(basePath, runnerAssemblyPath);

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