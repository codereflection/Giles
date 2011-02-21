using System;
using System.Collections.Generic;
using System.Text;

namespace Giles.Core.Runners
{
    public interface ITestListener
    {
        void WriteLine(string text, string category);
        void TestFinished(TestResult summary);
        void TestResultsUrl(string resultsUrl);
    }

    public class GilesTestListener : ITestListener
    {
        readonly Dictionary<string, StringBuilder> output = new Dictionary<string, StringBuilder>();
        Dictionary<TestState, int> results;

        public GilesTestListener()
        {
            results = new Dictionary<TestState, int>
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
            results[summary.State] += 1;
        }

        public void TestResultsUrl(string resultsUrl)
        {
            Console.WriteLine("GilesTestListener: TestResultsUrl fired w/ url: " + resultsUrl);
        }

        public void WriteResults()
        {
            Console.WriteLine("Passed: {0}, Failed: {1}, Ignored: {2}",
                              results[TestState.Passed],
                              results[TestState.Failed],
                              results[TestState.Ignored]);
        }
    }
}