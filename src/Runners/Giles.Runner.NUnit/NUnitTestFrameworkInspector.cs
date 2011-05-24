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
    }
}