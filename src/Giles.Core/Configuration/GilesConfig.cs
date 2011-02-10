using System.Collections.Generic;

namespace Giles.Core.Configuration
{
    public class GilesConfig
    {
        public IDictionary<string, RunnerAssembly> TestRunners = new Dictionary<string, RunnerAssembly>();
        public string TestAssemblyPath;
        public string SolutionPath;
    }
}