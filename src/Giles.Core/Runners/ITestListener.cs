namespace Giles.Core.Runners
{
    public interface ITestListener
    {
        void WriteLine(string text, string category);
        void AddTestSummary(TestResult summary);
    }
}