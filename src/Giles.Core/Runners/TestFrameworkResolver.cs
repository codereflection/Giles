using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Giles.Core.Utility;

namespace Giles.Core.Runners
{
    public class TestFrameworkResolver
    {
        List<TestFrameworkInspector> supportedFrameworkRunners;

        public TestFrameworkResolver()
        {
            BuildRunnerList();
        }

        void BuildRunnerList()
        {
            supportedFrameworkRunners = new List<TestFrameworkInspector>
                                            {
                                                new MSpecTestFrameworkInspector(),
                                                new NUnitTestFrameworkInspector(),
                                            };
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
    }
}