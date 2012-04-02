using System;
using System.Collections.Generic;
using System.Linq;
using Giles.Core.Runners;
using System.Reflection;
using NSpec.Domain;

namespace Giles.Runner.NSpec
{
    public class NSpecRunner : IFrameworkRunner
    {
        public IEnumerable<string> RequiredAssemblies()
        {
            return new[] { Assembly.GetAssembly(typeof(NSpecRunner)).Location, "NSpec.dll" };
        }

        public SessionResults RunAssembly(Assembly assembly, IEnumerable<string> filters)
        {
            var sessionResults = new SessionResults();
            var tags = string.Empty;
            if (filters.Count() > 0)
                tags = filters.Aggregate((working, next) => working + "," + next);
            var runner = new RunnerInvocation(assembly.Location, tags, new GilesSessionResultsFormatter(sessionResults), false);
            var runResults = runner.Run();
            return sessionResults;
        }
    }
}
