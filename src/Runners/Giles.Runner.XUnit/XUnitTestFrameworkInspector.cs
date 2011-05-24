using System;
using System.Reflection;
using Giles.Core.Runners;

namespace Giles.Runner.Xunit
{
    public class XUnitTestFrameworkInspector : TestFrameworkInspector
    {
        public override Func<AssemblyName, bool> Requirement
        {
            get
            {
                return referencedAssembly =>
                       referencedAssembly.Name.Equals("xunit", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}