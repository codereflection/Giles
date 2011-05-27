using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Giles.Core.Runners;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Runner;

namespace Giles.Runner.Machine.Specifications
{
    public class MSpecRunner : IFrameworkRunner
    {
        public SessionResults RunAssembly(Assembly assembly)
        {
            var runListener = new GilesMSpecRunListener();
            var runner = new AppDomainRunner(runListener, RunOptions.Default);
            runner.RunAssembly(assembly);
            return runListener.SessionResults;
        }

        public IEnumerable<string> RequiredAssemblies()
        {
            return new[] { Assembly.GetAssembly(typeof(MSpecRunner)).Location };
        }
    }
}