using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giles.Core.Configuration;
using Giles.Core.UI;
using Giles.Core.Utility;

namespace Giles.Core.Runners
{
    public class GilesTestListener
    {
        readonly GilesConfig config;
        readonly Dictionary<string, StringBuilder> output = new Dictionary<string, StringBuilder>();
        readonly Dictionary<TestState, int> totalResults;
        readonly Dictionary<string, Dictionary<TestState, int>> testRunnerResults;
        readonly IList<TestResult> testRunnerFailures = new List<TestResult>();

        public GilesTestListener()
        {
            totalResults = SetupTestResults();
            testRunnerResults = new Dictionary<string, Dictionary<TestState, int>>();
        }

        public GilesTestListener(GilesConfig config) : this()
        {
            this.config = config;
        }

        static Dictionary<TestState, int> SetupTestResults()
        {
            return new Dictionary<TestState, int>
                {
                    {TestState.Passed, 0},
                    {TestState.Failed, 0},
                    {TestState.Ignored, 0}
                };
        }

        public Dictionary<string, StringBuilder> GetOutput()
        {
            return output;
        }

        public void WriteLine(string text, string category)
        {
            if (!GetOutput().ContainsKey(category))
                GetOutput().Add(category, new StringBuilder());

            GetOutput()[category].AppendLine(text);
        }

        public void AddTestSummary(TestResult summary)
        {
            if (!testRunnerResults.ContainsKey(summary.TestRunner))
                testRunnerResults.Add(summary.TestRunner, SetupTestResults());

            if (summary.State == TestState.Failed)
                testRunnerFailures.Add(summary);

            testRunnerResults[summary.TestRunner][summary.State] += 1;

            totalResults[summary.State] += 1;
        }

        public void DisplayResults()
        {
            if (testRunnerResults.Count == 0)
            {
                config.UserDisplay.ToList().ForEach(display => display.DisplayResult(new ExecutionResult
                {
                    ExitCode = 1,
                    ErrorMessage = "No tests were run. Check your filter names and test assembly options",
                }));
                return;
            }

            var results = CreateResults();
            
            if (config.UserDisplay.IsNullOrEmpty())
                config.UserDisplay = new List<IUserDisplay> { new ConsoleUserDisplay() };

            config.UserDisplay.ToList().ForEach(display => results.ForEach(display.DisplayResult));
        }

        private List<ExecutionResult> CreateResults()
        {
            var result = new List<ExecutionResult>();

            testRunnerResults.ToList().ForEach(x => result.Add(new ExecutionResult
            {
                Runner = new TestRunnerResult
                {
                    RunnerName = x.Key,
                    Stats = new TestStatistics
                    {
                        Failed = x.Value[TestState.Failed],
                        Passed = x.Value[TestState.Passed],
                        Ignored = x.Value[TestState.Ignored]
                    }
                },
                ExitCode = x.Value[TestState.Failed] > 0 ? 1 : 0,
            }));

            return result;
        }

        public void DisplayVerboseResults()
        {
            Console.WriteLine("\n\nVerbose test results:");
            output.Each(x => Console.WriteLine(x.Value));
        }

        public void DisplayErrors()
        {
            var messages = new StringBuilder(string.Format("\n\nTest Run Errors ({0})\n", testRunnerFailures.Count()));
            testRunnerFailures.Each(x =>
                                        {
                                            messages.AppendLine("-------------------");
                                            messages.AppendLine(x.Name);
                                            messages.AppendLine(x.Message);
                                            messages.AppendLine(x.StackTrace);
                                        });
            Console.WriteLine(messages);
        }
    }
}