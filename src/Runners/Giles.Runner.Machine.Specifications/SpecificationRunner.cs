using System.Reflection;
using Giles.Core.Runners;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Giles.Runner.Machine.Specifications
{
    public class SpecificationRunner : IFrameworkRunner
    {
        public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
        {
            var listener = new GilesRunListener(testListener);
            var runner = new AppDomainRunner(listener, RunOptions.Default);
            runner.RunAssembly(assembly);

            return listener.TestRunState;
        }
    }
}