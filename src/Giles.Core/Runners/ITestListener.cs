namespace Giles.Core.Runners
{
    public interface ITestListener
    {
        void WriteLine(string text, string category);
        void TestFinished(TestResult summary);
        void TestResultsUrl(string resultsUrl);
    }
}