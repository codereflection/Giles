using System;
using System.Collections.Generic;

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
        Ignored
    }

    public enum SessionRunState
    {
        Success,
        Failure,
        Error,
        NoTests
    }

    public class SessionResults
    {
        public SessionRunState SessionRunState { get; set; }
        public IEnumerable<TestResult> TestResults { get; set; }
    }
}