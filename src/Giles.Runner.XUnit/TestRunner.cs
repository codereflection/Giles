using Giles.Core.Runners;
using System;
using XunitFx = Xunit;

namespace Giles.Runner.Xunit {
    public class XunitTestRunner : IFrameworkRunner {
        public SessionResults RunAssembly(System.Reflection.Assembly assembly) {
            using (var exWrapper = new XunitFx.ExecutorWrapper(new Uri(assembly.CodeBase).LocalPath, null, false)) {
                var logger = new XunitLogger();
                var runner = new XunitFx.TestRunner(exWrapper, logger);
            }
        }
    }
}
