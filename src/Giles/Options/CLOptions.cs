using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Giles.Options
{
    public class CLOptions
    {
        [Option("s", "SolutionPath", Required = true, HelpText = "Path to the sln file")]
        public string SolutionPath;

        [Option("t", "TestAssemblyPath", Required = true, HelpText = "Path to the test assembly dll")]
        public string TestAssemblyPath;

        [Option("p", "ProjectRoot", Required = true, HelpText = "Path to the root of the project")]
        public string ProjectRoot;

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
    }
}