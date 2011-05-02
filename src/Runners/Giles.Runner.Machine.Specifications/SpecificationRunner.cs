using System.Collections.Generic;
using System.Reflection;
using Giles.Core.Configuration;
using Giles.Core.Runners;
using Giles.Core.UI;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Runner;

namespace Giles.Runner.Machine.Specifications
{
    public class SpecificationRunner : IFrameworkRunner
    {
        public SessionRunState SessionResults(Assembly assembly)
        {
            var testListener = new GilesTestListener(new GilesConfig { UserDisplay = new List<IUserDisplay> { new ConsoleUserDisplay() } });
            var runListener = new GilesMSpecRunListener(testListener);
            var runner = new AppDomainRunner(runListener, RunOptions.Default);
            runner.RunAssembly(assembly);
            
            testListener.DisplayResults();

            return runListener.SessionRunState;
        }
    }
}