using Giles.Core.Runners;
using System;
using XunitFx = Xunit;
using System.Collections.Generic;
using System.Reflection;

namespace Giles.Runner.Xunit {
    public class XunitTestRunner : IFrameworkRunner {
        public SessionResults RunAssembly(Assembly assembly) {
            var sessionResults = new SessionResults();
            var logger = new GilesXunitLogger();
            using (var exWrapper = new XunitFx.ExecutorWrapper(new Uri(assembly.CodeBase).LocalPath, null, false)) {
                var runner = new XunitFx.TestRunner(exWrapper, logger);
                var result = runner.RunAssembly();
            }
            return logger.SessionResults;
        }
    }
}