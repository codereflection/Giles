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
                return referencedAssembly =>
                       referencedAssembly.Name.Equals("nunit.framework", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public override Func<IFrameworkRunner> Get
        {
            get { return () => GetRunnerBy(typeof(NUnitTestFrameworkInspector).Assembly.ManifestModule.Name); }
        }
    }
}