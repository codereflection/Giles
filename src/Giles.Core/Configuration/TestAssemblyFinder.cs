using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Giles.Core.IO;

namespace Giles.Core.Configuration
{
    public class TestAssemblyFinder
    {
        private static readonly IFileSystem FileSystem = new FileSystem();

        public Func<IFileSystem> GetFileSystem = () => FileSystem;

        //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Giles.Specs", "Giles.Specs\Giles.Specs.csproj", "{8FAE1516-9D4A-4575-83BF-515BC6AF8AC3}"
        private static readonly Regex SolutionFileRegex = new Regex(
            @"Project\(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC\}""\)" +
            @"[^""]*""[^""]*""" +
            @"[^""]*""([^""]*)""", RegexOptions.Compiled);

        private static readonly string[] SupportedAssemblies
                = new[]
                  {
                          "Machine.Specifications.dll",
                          "nunit.framework.dll",
                          "xunit.dll"
                  };

        public List<string> FindTestAssembliesIn(string solutionFilePath)
        {
            var projectFiles = GetProjectFilePaths(solutionFilePath);
            var testAssemblies = new List<Tuple<int, string>>();
            foreach (var file in projectFiles)
            {
                using (var stream = GetFileSystem().OpenFile(file, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite))
                {
                    var project = MsBuildProject.Load(stream);
                    var isTestProjectScore = GetIsTestProjectScore(project);
                    if (isTestProjectScore > 0)
                    {
                        var assemblyFilePath = project.GetAssemblyFilePath(file);
                        testAssemblies.Add(new Tuple<int, string>(
                            isTestProjectScore, assemblyFilePath));
                    }
                }
            }

            // return assemblies with highest score first
            return (from tuple in testAssemblies
                    orderby tuple.Item1 descending,
                            tuple.Item2 ascending // keep things deterministic for testing
                    select tuple.Item2).ToList();
        }

        private IEnumerable<String> GetProjectFilePaths(string solutionFilePath)
        {
            var solutionContents = GetFileSystem().ReadAllText(solutionFilePath);
            var solutionFileDir = Path.GetDirectoryName(solutionFilePath);
            var match = SolutionFileRegex.Match(solutionContents);
            while (match.Success)
            {
                var relativePath = match.Groups[1].Value;
                match = match.NextMatch();
                var path = Path.Combine(solutionFileDir, relativePath);
                yield return path;
            }
        }

        private int GetIsTestProjectScore(MsBuildProject project)
        {
            // TODO: may want to group this information with other info about supported test frameworks
            var numberOfReferencedTestAssemblies =
                    SupportedAssemblies.Where(item => IsTestFrameworkReferenced(project, item))
                            .Count();
            var score = numberOfReferencedTestAssemblies * 10;

            // no supported test framework, abort
            if (score == 0)
                return 0;

            var assemblyName = project.GetAssemblyName();
            if (assemblyName.Contains("UnitTest"))
                score += 5;
            else if (assemblyName.Contains("Test"))
                score += 3;

            if (assemblyName.Contains("Spec"))
                score += 2; // more likely to have non-test project with Spec in it?

            return score;
        }

        private static bool IsTestFrameworkReferenced(MsBuildProject project, string frameworkAssemblyFilename)
        {
            return project.GetLocalAssemblyRefs().Any(info =>
                info.EndsWith(frameworkAssemblyFilename, StringComparison.OrdinalIgnoreCase));
        }
    }
}