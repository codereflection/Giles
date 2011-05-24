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

            var result =
                supportedFrameworkRunners
                    .Where(theRunner => referencedAssemblies.AnyMatchesRequirementFor(theRunner.Requirement))
                    .Select(theRunner => theRunner.Get.Invoke());

            return result;
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
    }
}