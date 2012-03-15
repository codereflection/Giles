using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giles.Core.Runners;

namespace Giles.Runner.NSpec
{
    public class NSpecTestFrameworkInspector : TestFrameworkInspector
    {
        public override Func<System.Reflection.AssemblyName, bool> Requirement
        {
            get
            {
                return referencedAssembly =>
                       referencedAssembly.Name.Equals("NSpec", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
