using Giles.Core.UI;

namespace Giles.Core.Configuration
{
    public class GilesConfigBuilder
    {
        readonly GilesConfig config = new GilesConfig();
        readonly string solutionPath;
        readonly string testAssemblyPath;

        public GilesConfigBuilder(string solutionPath, string testAssemblyPath)
        {
            this.solutionPath = solutionPath;
            this.testAssemblyPath = testAssemblyPath;
        }

        public GilesConfig Build()
        {
            config.TestAssemblyPath = testAssemblyPath;
            config.SolutionPath = "" + solutionPath + "";

            config.UserDisplay.Add(new ConsoleUserDisplay());
            config.UserDisplay.Add(new GrowlUserDisplay());
            return config;
        }
    }
}