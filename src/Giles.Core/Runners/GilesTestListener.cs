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
            var messages = AggregateTestRunnerResults();

            var result = new ExecutionResult
                {
                    ExitCode = totalResults[TestState.Failed] > 0 ? 1 : 0,
                    Output = messages.ToString(),
                    Runner = string.Empty
                };

            if (config.UserDisplay.Count() == 0)
                config.UserDisplay = new List<IUserDisplay> { new ConsoleUserDisplay() };

            config.UserDisplay.ToList().ForEach(display => display.DisplayResult(result));
        }

        StringBuilder AggregateTestRunnerResults()
        {
            var messages = new StringBuilder();
            testRunnerResults.ToList().ForEach(x => messages.Append(
                string.Format(
                    "{0} Results: Passed: {1}, Failed: {2}, Ignored: {3}\n",
                    x.Key,
                    x.Value[TestState.Passed],
                    x.Value[TestState.Failed],
                    x.Value[TestState.Ignored])));

            messages.Append(string.Format("Total Passed: {0}, Failed: {1}, Ignored: {2}",
                                          totalResults[TestState.Passed],
                                          totalResults[TestState.Failed],
                                          totalResults[TestState.Ignored]));
            return messages;
        }

        public void DisplayVerboseResults()
        {
            Console.WriteLine("\n\nVerbose test results:");
            output.Each(x => Console.WriteLine(x.Value));
        }

        public void DisplayErrors()
        {
            var messages = new StringBuilder();
            testRunnerFailures.Each(x =>
                                        {
                                            messages.AppendLine(x.Name);
                                            messages.AppendLine(x.Message);
                                        });
            Console.WriteLine(messages);
        }
    }
}