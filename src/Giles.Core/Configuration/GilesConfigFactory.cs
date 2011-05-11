using System;
using System.Collections.Generic;
using System.Reflection;
using Giles.Core.Runners;
using Giles.Core.UI;

namespace Giles.Core.Configuration
{
    public class GilesConfigFactory
    {
        readonly GilesConfig config;
        readonly string solutionPath;
        readonly string testAssemblyPath;

        public GilesConfigFactory(GilesConfig config, string solutionPath, string testAssemblyPath)
        {
            this.config = config;
            this.solutionPath = solutionPath;
            this.testAssemblyPath = testAssemblyPath;
        }

        public Func<string, Assembly> LoadAssembly;

        public GilesConfig Build()
        {
            config.TestAssemblyPath = testAssemblyPath;
            config.SolutionPath = solutionPath;

            config.UserDisplay = new List<IUserDisplay> {new ConsoleUserDisplay(), new GrowlUserDisplay()};
            config.Executor = new CommandProcessExecutor();
            return config;
        }
    }
}