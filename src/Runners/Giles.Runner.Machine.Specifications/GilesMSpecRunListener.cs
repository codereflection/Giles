using System.Collections.Generic;
using Giles.Core.Runners;
using Machine.Specifications;
using Machine.Specifications.Runner;

namespace Giles.Runner.Machine.Specifications
{
    public class GilesMSpecRunListener : ISpecificationRunListener
    {
        const string _testRunnerName = "MSPEC";
        readonly ResultFormatterFactory resultFormatterFactory;
        private readonly SessionResults sessionResults = new SessionResults();

        public SessionResults SessionResults
        {
            get { return sessionResults; }
        }

        readonly List<TestResult> testResults = new List<TestResult>();

        public GilesMSpecRunListener()
        {
            resultFormatterFactory = new ResultFormatterFactory();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {

        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {

        }

        public void OnRunStart()
        {

        }

        public void OnRunEnd()
        {
            if (testResults.Count == 0) return;

            foreach (var testResult in testResults)
            {
                SessionResults.TestResults.Add(testResult);
            }
        }

        public void OnContextStart(ContextInfo context)
        {
            SessionResults.Messages.Add(context.FullName);
        }

        public void OnContextEnd(ContextInfo context)
        {
            //testListener.WriteLine("", "Output");
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            var formatter = resultFormatterFactory.GetResultFormatterFor(result);
            SessionResults.Messages.Add(formatter.FormatResult(specification));

            var testResult = new TestResult { Name = specification.Name, TestRunner = _testRunnerName };

            if (result.Passed)
            {
                testResult.State = TestState.Passed;
            }
            else switch (result.Status)
                {
                    case Status.Ignored:
                        testResult.State = TestState.Ignored;
                        testResult.Message = "Ignored";
                        break;
                    case Status.NotImplemented:
                        testResult.State = TestState.Ignored;
                        testResult.Message = "Not Implemented";
                        break;
                    default:
                        testResult.State = TestState.Failed;
                        if (result.Exception != null)
                        {
                            testResult.StackTrace = result.Exception.ToString();
                        }
                        break;
                }

            testResults.Add(testResult);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            SessionResults.Messages.Add("Fatal error: " + exception);
        }
    }
}