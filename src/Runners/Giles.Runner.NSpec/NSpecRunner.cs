using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giles.Core.Runners;

namespace Giles.Runner.NSpec
{
    public class NSpecRunner : IFrameworkRunner
    {
        public IEnumerable<string> RequiredAssemblies()
        {
            throw new NotImplementedException();
        }

        public SessionResults RunAssembly(System.Reflection.Assembly assembly)
        {
            throw new NotImplementedException();
        }
    }
}
