using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Giles.Core.IO
{
	public interface ISolutionParser
	{
		IEnumerable<string> GetProjectPaths(string solutionPath);
	}

	public class LegacyParser : ISolutionParser
	{
		public IEnumerable<string> GetProjectPaths(string solutionPath)
		{
			return new List<string>
			{
				solutionPath
			};
		}
	}

	public class ReflectionSolutionParser : ISolutionParser
	{
		static readonly Type SolutionParser;
		static readonly PropertyInfo SolutionReader;
		static readonly MethodInfo ParseSolution;
		static readonly PropertyInfo Projects;

		static ReflectionSolutionParser()
		{
			SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
			if (SolutionParser != null)
			{
				SolutionReader = SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
				Projects = SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
				ParseSolution = SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
			}
		}

		public IEnumerable<string> GetProjectPaths(string solutionPath)
		{
			if (SolutionParser == null)
			{
				throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
			}
			var solutionParser = SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(null);
			using (var streamReader = new StreamReader(solutionPath))
			{
				SolutionReader.SetValue(solutionParser, streamReader, null);
				ParseSolution.Invoke(solutionParser, null);
			}
			var result = new List<string>();
			var array = (Array)Projects.GetValue(solutionParser, null);
			var solutionProject = new SolutionProject();
			for (int i = 0; i < array.Length; i++)
			{
				result.Add(solutionProject.GetRelativePath(array.GetValue(i)));
			}
			return result;
		}

		class SolutionProject
		{
			static readonly Type ProjectInSolution;
			static readonly PropertyInfo RelativePath;

			static SolutionProject()
			{
				ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
				if (ProjectInSolution != null)
				{
					RelativePath = ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
				}
			}

			public string GetRelativePath(object solutionProject)
			{
				return RelativePath.GetValue(solutionProject, null) as string;
			}
		}
	}
}
