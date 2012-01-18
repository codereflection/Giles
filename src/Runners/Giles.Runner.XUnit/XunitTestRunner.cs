using System.Collections.Generic;
using System.Linq;
using Giles.Core.Runners;
using System;
using Giles.Core.Utility;
using XunitFx = Xunit;
using System.Reflection;

namespace Giles.Runner.Xunit {
    public class XunitTestRunner : IFrameworkRunner {
        public SessionResults RunAssembly(Assembly assembly, IEnumerable<string> filters) {

            var logger = new GilesXunitLogger();
            
            using (var exWrapper = new XunitFx.ExecutorWrapper(new Uri(assembly.CodeBase).LocalPath, null, false)) {
                var runner = new XunitFx.TestRunner(exWrapper, logger);
                if (filters.Count() == 0)
                    runner.RunAssembly();
                else
                    filters.Each(x => runner.RunClass(x));
            }

            return logger.SessionResults;
        }

        public IEnumerable<string> RequiredAssemblies()
        {
            return new[]
                       {
                           Assembly.GetAssembly(typeof(XunitTestRunner)).Location, "xunit.runner.utility.dll"
                       };
        }
    }
}