using System;
using System.Reflection;
using Giles.Core.Runners;

namespace Giles.Runner.Machine.Specifications
{
    public class MSpecTestFrameworkInspector : TestFrameworkInspector
    {
        public override Func<AssemblyName, bool> Requirement
        {
            get
            {
                return referencedAssembly =>
                       referencedAssembly.Name.Equals("Machine.Specifications", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public override Func<IFrameworkRunner> Get
        {
            get { return () => GetRunnerBy(typeof(MSpecTestFrameworkInspector).Assembly.ManifestModule.Name); }
        }
    }
}