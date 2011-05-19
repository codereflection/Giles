using System;
using Xunit;

namespace Giles.Runner.Xunit {
    public class XunitLogger : IRunnerLogger {
        public void AssemblyFinished(string assemblyFilename, int total, int failed, int skipped, double time) {
            throw new NotImplementedException();
        }
        public void AssemblyStart(string assemblyFilename, string configFilename, string xUnitVersion) {
            throw new NotImplementedException();
        }
        public bool ClassFailed(string className, string exceptionType, string message, string stackTrace) {
            throw new NotImplementedException();
        }
        public void ExceptionThrown(string assemblyFilename, Exception exception) {
            throw new NotImplementedException();
        }
        public void TestFailed(string name, string type, string method, double duration, string output, string exceptionType, string message, string stackTrace) {
            throw new NotImplementedException();
        }
        public bool TestFinished(string name, string type, string method) {
            throw new NotImplementedException();
        }
        public void TestPassed(string name, string type, string method, double duration, string output) {
            throw new NotImplementedException();
        }
        public void TestSkipped(string name, string type, string method, string reason) {
            throw new NotImplementedException();
        }
        public bool TestStart(string name, string type, string method) {
            throw new NotImplementedException();
        }
    }
}
