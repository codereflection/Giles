using System;
using System.Collections.Generic;
using Giles.Core.Runners;
using ImpromptuInterface.Dynamic;

namespace Giles.Runner.Machine.Specifications
{
    public static class MSpecTypes
    {
        public static IEnumerable<Type> Types { get; set; }
    }

    public class GilesMSpecRunListener
    {
        public static object GetAnonymousListener(SessionResults sessionResults, List<TestResult> testResults, ResultFormatterFactory resultFormatterFactory)
        {
            return new
                {
                    OnAssemblyStart = ReturnVoid.Arguments<dynamic>(assembly => { }),

                    OnAssemblyEnd = ReturnVoid.Arguments<dynamic>(assembly => { }),
                           
                    OnRunStart = ReturnVoid.Arguments(() => { }),
                           
                    OnRunEnd = ReturnVoid.Arguments(() => testResults.ForEach(x => sessionResults.TestResults.Add(x))),
                           
                    OnContextStart = ReturnVoid.Arguments<dynamic>(context =>
                                                                       {
                                                                           string r = string.Format("\n{0}", context.FullName);
                                                                           sessionResults.Messages.Add(r);
                                                                       }),
                           
                    OnContextEnd = ReturnVoid.Arguments<dynamic>(context => { }),
                           
                    OnSpecificationStart = ReturnVoid.Arguments<Object>(specification => { }),
                           
                    OnSpecificationEnd = ReturnVoid.Arguments<dynamic, dynamic>((specification, result) =>
                                        {
                                            var formatter = ResultFormatterFactory.GetResultFormatterFor(result: result.Status.ToString());

                                            string formatResult = formatter.FormatResult(specification, result);
                                            sessionResults.Messages.Add(formatResult);

                                            var testResult =
                                                new TestResult { Name = specification.Name, TestRunner = "MSPEC" };

                                            ProcessTestResult((object) result, testResult, testResults);
                                        }),

                    OnFatalError = ReturnVoid.Arguments<dynamic>(exception => sessionResults.Messages.Add("Fatal error: " + exception)),

                    sessionResults,
                           
                    testResults,
                           
                    resultFormatterFactory
                };
        }

        private static void ProcessTestResult(dynamic result, TestResult testResult, ICollection<TestResult> testResults)
        {
            if (result.Passed)
                testResult.State = TestState.Passed;
            else if (result.Status.ToString() == "Ignored")
            {
                testResult.State = TestState.Ignored;
                testResult.Message = "Ignored";
            }
            else if (result.Status.ToString() == "NotImplemented")
            {
                testResult.State = TestState.Ignored;
                testResult.Message = "Not Implemented";
            }
            else
            {
                testResult.State = TestState.Failed;
                if (result.Exception != null)
                    testResult.StackTrace = result.Exception.ToString();
            }
            testResults.Add(testResult);
        }
    }
}