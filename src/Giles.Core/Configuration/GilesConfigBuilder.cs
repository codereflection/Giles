using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Giles.Core.UI;

namespace Giles.Core.Configuration
{
    public class GilesConfigBuilder
    {
        GilesConfig config = new GilesConfig();
        readonly string solutionPath;
        readonly List<string> testAssemblies;

        public GilesConfigBuilder(string solutionPath, List<string> testAssemblies)
        {
            this.solutionPath = solutionPath;
            this.testAssemblies = testAssemblies;
        }

        public GilesConfig Build()
        {
            var path = GilesConfigPath(solutionPath);
            if (File.Exists(path))
                config = new JavaScriptSerializer().Deserialize<GilesConfig>(File.ReadAllText(path));
            else
            {
                config.TestAssemblies = testAssemblies;
                config.SolutionPath = "" + solutionPath + "";
                config.Filters = new List<string>();
            }
            config.UserDisplay.Add(new ConsoleUserDisplay());
            config.UserDisplay.Add(new GrowlUserDisplay());
            return config;
        }

        public static string Save(GilesConfig config)
        {
            var serializer = new JavaScriptSerializer();
            var data = serializer.Serialize(config);

            var path = GilesConfigPath(config.SolutionPath);
            File.WriteAllText(path, data);
            return path;
        }

        static string GilesConfigPath(string solutionPath)
        {
            return Path.Combine(Path.GetDirectoryName(solutionPath), ".giles");
        }
    }
}