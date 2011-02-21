using System;

namespace Giles.Core.Runners
{
    public class TestResult
    {
        public string Message;
        public string Name;
        public string StackTrace;
        public TestState State;
        public string TestRunner;
        public TimeSpan TimeSpan;
        public int TotalTests;
    }

    public enum TestState
    {
        Passed,
        Failed,
        Ignored,
    }

}