using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giles.Core.Runners;
using System.Reflection;
using NSpec;
using NSpec.Domain;

namespace Giles.Runner.NSpec
{
    public class NSpecRunner : IFrameworkRunner
    {
        public IEnumerable<string> RequiredAssemblies()
        {
            return new[] { Assembly.GetAssembly(typeof(NSpecRunner)).Location, "NSpec.dll" };
        }

        public SessionResults RunAssembly(Assembly assembly)
        {
            var sessionResults = new SessionResults();
            var runner = new RunnerInvocation(assembly.Location, String.Empty, new GilesSessionResultsFormatter(sessionResults), false);
            var runResults = runner.Run();
            return sessionResults;
        }
    }
}
