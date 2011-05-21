using System;
using XunitFx = Xunit;
using Giles.Core.Runners;
using System.Collections.Generic;

namespace Giles.Runner.Xunit {
    public class GilesXunitLogger : XunitFx.IRunnerLogger {

        public const string _runnerName = "Xunit";

        private SessionResults  _sessionResults = null;
        public SessionResults SessionResults { get { return _sessionResults; } }

        public readonly List<Core.Runners.TestResult> testResults = new List<Core.Runners.TestResult>();

        public void AssemblyFinished(string assemblyFilename, int total, int failed, int skipped, double time) {
            if (testResults.Count == 0) return;

            foreach (var testResult in testResults) {
                SessionResults.TestResults.Add(testResult);
            }
        }

        public void AssemblyStart(string assemblyFilename, string configFilename, string xUnitVersion) {

        }
        public bool ClassFailed(string className, string exceptionType, string message, string stackTrace) {
            return false;
        }
        public void ExceptionThrown(string assemblyFilename, Exception exception) {
            var testResult = new Core.Runners.TestResult { Name = assemblyFilename, 
                                                             TestRunner = _runnerName, 
                                                             Message = exception.Message, 
                                                             StackTrace = exception.StackTrace, 
                                                             State = TestState.Failed };
            testResults.Add(testResult);
        }
        public void TestFailed(string name, string type, string method, double duration, string output, string exceptionType, string message, string stackTrace) {
            var testResult = new Core.Runners.TestResult { Name = name, 
                                                             TestRunner = _runnerName, 
                                                             Message = output, 
                                                             State = TestState.Failed };
            testResults.Add(testResult);
        }
        public bool TestFinished(string name, string type, string method) {
            return true;
        }
        public void TestPassed(string name, string type, string method, double duration, string output) {
            var testResult = new Core.Runners.TestResult { Name = name, 
                                                             TestRunner = _runnerName, 
                                                             Message = output, 
                                                             State= TestState.Passed };
            testResults.Add(testResult);
        }
        public void TestSkipped(string name, string type, string method, string reason) {
            var testResult = new Core.Runners.TestResult { Name = type + ":" + method, 
                                                             TestRunner = _runnerName, 
                                                             Message = "Ignored:" + reason, 
                                                             State = TestState.Ignored };
            testResults.Add(testResult);
        }
        public bool TestStart(string name, string type, string method) {
            return true;
        }
    }
}
