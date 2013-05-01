using System.Collections.Generic;
using Giles.Core.UI;

namespace Giles.Core.Configuration
{
    public class GilesConfigBuilder
    {
        readonly GilesConfig config = new GilesConfig();
        readonly string solutionPath;
        readonly List<string> testAssemblies;

        public GilesConfigBuilder(string solutionPath, List<string> testAssemblies)
        {
            this.solutionPath = solutionPath;
            this.testAssemblies = testAssemblies;
        }

        public GilesConfig Build()
        {
            config.TestAssemblies = testAssemblies;
            config.SolutionPath = "" + solutionPath + "";

            config.UserDisplay.Add(new ConsoleUserDisplay());
            config.UserDisplay.Add(new GrowlUserDisplay());
            config.Filters = new List<Filter>();
            return config;
        }
    }
}