using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giles.Core.Configuration;
using Machine.Specifications.Utility;

namespace Giles.Core.Runners
{
    public class GilesTestListener : ITestListener
    {
        readonly GilesConfig config;
        readonly Dictionary<string, StringBuilder> output = new Dictionary<string, StringBuilder>();
        readonly Dictionary<TestState, int> totalResults;
        readonly Dictionary<string, Dictionary<TestState, int>> testRunnerResults;

        public GilesTestListener(GilesConfig config)
        {
            this.config = config;
            totalResults = SetupTestResults();
            testRunnerResults = new Dictionary<string, Dictionary<TestState, int>>();
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

        public void TestFinished(TestResult summary)
        {
            if (!testRunnerResults.ContainsKey(summary.TestRunner))
                testRunnerResults.Add(summary.TestRunner, SetupTestResults());

            testRunnerResults[summary.TestRunner][summary.State] += 1;

            totalResults[summary.State] += 1;
        }

        public void TestResultsUrl(string resultsUrl)
        {
            Console.WriteLine("GilesTestListener: TestResultsUrl fired w/ url: " + resultsUrl);
        }

        public void DisplayResults()
        {
            var message = "";
            testRunnerResults.ToList().ForEach(x =>
                                                   {
                                                       message +=
                                                           string.Format(
                                                               "{0} Results: Passed: {1}, Failed: {2}, Ignored: {3}\n",
                                                               x.Key,
                                                               x.Value[TestState.Passed],
                                                               x.Value[TestState.Failed],
                                                               x.Value[TestState.Ignored]);
                                                   });

            message += string.Format("Total Passed: {0}, Failed: {1}, Ignored: {2}",
                                                                      totalResults[TestState.Passed],
                                                                      totalResults[TestState.Failed],
                                                                      totalResults[TestState.Ignored]);

            config.UserDisplay.Each(display => display.DisplayMessage(message));
        }
    }
}