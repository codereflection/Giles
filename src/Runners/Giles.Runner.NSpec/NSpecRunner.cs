using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giles.Core.Configuration;
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

        public SessionResults RunAssembly(Assembly assembly, IEnumerable<Filter> filters)
        {
            var sessionResults = new SessionResults();
            var tags = string.Empty;
            var filtersList = filters as IList<Filter> ?? filters.ToList();

            if (filtersList.Any())
            {
                var sb = new StringBuilder();

                foreach (var filter in filtersList)
                {
                    sb.Append(filter);
                    sb.Append(",");
                }

                sb.Remove(sb.Length, 1);

                tags = sb.ToString();
            }

            var runner = new RunnerInvocation(assembly.Location, tags, new GilesSessionResultsFormatter(sessionResults), false);
            runner.Run();
            return sessionResults;
        }
    }
}
