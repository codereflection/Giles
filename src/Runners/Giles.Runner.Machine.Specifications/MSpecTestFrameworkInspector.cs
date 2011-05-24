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
                return assemblyName =>
                       assemblyName.Name.Equals("Machine.Specifications", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public override Func<IFrameworkRunner> Get
        {
            get { return () => GetRunnerBy("Giles.Runner.Machine.Specifications.dll"); }
        }
    }
}