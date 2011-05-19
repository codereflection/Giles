using System;
using System.Collections.Generic;
using Giles.Core.Runners;
using NUnit.Core;
using TestResult = NUnit.Core.TestResult;

namespace Giles.Runner.NUnit
{
    public class GilesNUnitEventListener : EventListener
    {
        const string _testRunnerName = "NUNIT";
        private readonly SessionResults sessionResults = new SessionResults();

        public SessionResults SessionResults
        {
            get { return sessionResults; }
        }

        readonly List<Core.Runners.TestResult> testResults = new List<Core.Runners.TestResult>();


        public void RunStarted(string name, int testCount)
        {
        }

        public void RunFinished(TestResult result)
        {
            if (testResults.Count == 0) return;

            foreach (var testResult in testResults)
            {
                SessionResults.TestResults.Add(testResult);
            }
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestStarted(TestName testName)
        {
        }

        public void TestFinished(TestResult result)
        {
            sessionResults.Messages.Add(string.Format("\n{0}: {1}", result.Name, result.ResultState.ToString()));
            var testResult = new Core.Runners.TestResult { Name = result.Name, TestRunner = _testRunnerName };
            if (result.IsSuccess)
                testResult.State = TestState.Passed;
            else switch (result.ResultState)
            {
                case ResultState.Ignored:
                    testResult.State = TestState.Ignored;
                    testResult.Message = "Ignored";
                    break;
                default:
                    testResult.State = TestState.Failed;
                    testResult.StackTrace = result.StackTrace;
                    break;
            }
            testResults.Add(testResult);
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void SuiteFinished(TestResult result)
        {
        }

        public void UnhandledException(Exception exception)
        {
        }

        public void TestOutput(TestOutput testOutput)
        {
        }
    }
}