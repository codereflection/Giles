using System;
using System.Reflection;
using Giles.Core.Runners;

namespace Giles.Runner.NUnit
{
    public class NUnitTestFrameworkInspector : TestFrameworkInspector
    {
        public override Func<AssemblyName, bool> Requirement
        {
            get
            {
                return assemblyName =>
                       assemblyName.Name.Equals("nunit.framework", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public override Func<IFrameworkRunner> Get
        {
            get { return () => GetRunnerBy("Giles.Runner.NUnit.dll"); }
        }
    }
}