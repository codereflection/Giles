using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Giles.Options
{
    public class CLOptions
    {
        [Option("s", "SolutionPath", Required = true, HelpText = "Path to the sln file")]
        public string SolutionPath;

        [Option("t", "TestAssemblyPaths", Required = false, HelpText = "Comma separated paths to the test assembly dll's")]
        public string TestAssemblyPaths;

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var helpText = new HelpText("Giles Options")
                               {
                                   AdditionalNewLineAfterOption = true
                               };
            helpText.AddOptions(this);

            return helpText;            
        }

        public List<string> GetTestAssemblies()
        {
            return string.IsNullOrEmpty(TestAssemblyPaths) ? 
                new List<string>() :
                ParseTestAssemblyOption();
        }

        List<string> ParseTestAssemblyOption()
        {
            var result = new List<string>(TestAssemblyPaths.Replace("\'", string.Empty).Replace("\"", string.Empty).Split(','));

            for (var index = 0; index < result.Count; index++)
                result[index] = result[index].Trim();

            return result;
        }
    }
}