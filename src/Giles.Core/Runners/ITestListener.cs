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
        Dictionary<string, StringBuilder> output = new Dictionary<string, StringBuilder>();

        public void WriteLine(string text, string category)
        {
            if (!output.ContainsKey(category))
                output.Add(category, new StringBuilder());

            output[category].AppendLine(text);
        }

        public void TestFinished(TestResult summary)
        {
            Console.WriteLine("GilesTestListener: TestFinished fired w/ message:" + summary.Message);
        }

        public void TestResultsUrl(string resultsUrl)
        {
            Console.WriteLine("GilesTestListener: TestResultsUrl fired w/ url: " + resultsUrl);
        }
    }
}