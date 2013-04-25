using System.Collections.Generic;
using System.Reflection;

namespace Giles.Core.Runners
{
    public interface IFrameworkRunner
    {
        SessionResults RunAssembly(Assembly assembly, IEnumerable<string> filters);

        /// <summary>
        /// Returns a list of assemblies which the test runner implementation requires to run
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> RequiredAssemblies();
    }
}