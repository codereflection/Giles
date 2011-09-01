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

    public class GilesMSpecRunListener //: ISpecificationRunListener
    {
        public static object GetAnonymousListener(SessionResults sessionResults, List<TestResult> testResults, ResultFormatterFactory resultFormatterFactory)
        {
            return new
                       {
                           OnAssemblyStart = ReturnVoid.Arguments<dynamic>(a => { }),
                           //void OnAssemblyStart(AssemblyInfo assembly);
                           OnAssemblyEnd = ReturnVoid.Arguments<dynamic>(a => { }),
                           //void OnAssemblyEnd(AssemblyInfo assembly);
                           OnRunStart = ReturnVoid.Arguments(() => { }),
                           //void OnRunStart();
                           OnRunEnd = ReturnVoid.Arguments(() => testResults.ForEach(x => sessionResults.TestResults.Add(x))),
                           //void OnRunEnd();
                           OnContextStart = ReturnVoid.Arguments<dynamic>(c => { }),
                           //void OnContextStart(ContextInfo context);
                           OnContextEnd = ReturnVoid.Arguments<dynamic>(c => { }),
                           //void OnContextEnd(ContextInfo context);
                           OnSpecificationStart = ReturnVoid.Arguments<Object>(s => { }),
                           //void OnSpecificationStart(SpecificationInfo specification);
                           OnSpecificationEnd = ReturnVoid.Arguments<dynamic, dynamic>((s, r) =>
                                                {
                                                    var formatter = ResultFormatterFactory.GetResultFormatterFor(result: r.Status.ToString());

                                                    string formatResult = formatter.FormatResult(s, r);
                                                    sessionResults.Messages.Add(formatResult);
                                                    Console.WriteLine("Passed: {0}", r.Passed);

                                                    var testResult =
                                                        new TestResult { Name = s.Name, TestRunner = "MSPEC" };

                                                    if (r.Passed)
                                                    {
                                                        testResult.State =
                                                            TestState.Passed;
                                                    }

                                                    testResults.Add(testResult);
                                                }),
                           //void OnSpecificationEnd(SpecificationInfo specification, Result result);
                           OnFatalError = ReturnVoid.Arguments<Object>(e => { }),
                           //void OnFatalError(ExceptionResult exception);
                           sessionResults,
                           testResults,
                           resultFormatterFactory
                       };
        }


        //const string _testRunnerName = "MSPEC";
        //readonly ResultFormatterFactory resultFormatterFactory;
        //private readonly SessionResults sessionResults = new SessionResults();

        //public SessionResults SessionResults
        //{
        //    get { return sessionResults; }
        //}

        //readonly List<TestResult> testResults = new List<TestResult>();

        //public GilesMSpecRunListener()
        //{
        //    resultFormatterFactory = new ResultFormatterFactory();
        //}

        //public void OnAssemblyStart(AssemblyInfo assembly)
        //{

        //}

        //public void OnAssemblyEnd(AssemblyInfo assembly)
        //{

        //}

        //public void OnRunStart()
        //{

        //}

        //public void OnRunEnd()
        //{
        //    if (testResults.Count == 0) return;

        //    foreach (var testResult in testResults)
        //    {
        //        SessionResults.TestResults.Add(testResult);
        //    }
        //}

        //public void OnContextStart(ContextInfo context)
        //{
        //    SessionResults.Messages.Add(string.Format("\n{0}", context.FullName));
        //}

        //public void OnContextEnd(ContextInfo context)
        //{
        //    //testListener.WriteLine("", "Output");
        //}

        //public void OnSpecificationStart(SpecificationInfo specification)
        //{
        //}

        //public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        //{
        //    var formatter = resultFormatterFactory.GetResultFormatterFor(result);
        //    SessionResults.Messages.Add(formatter.FormatResult(specification, result));

        //    var testResult = new TestResult { Name = specification.Name, TestRunner = _testRunnerName };

        //    if (result.Passed)
        //    {
        //        testResult.State = TestState.Passed;
        //    }
        //    else switch (result.Status)
        //        {
        //            case Status.Ignored:
        //                testResult.State = TestState.Ignored;
        //                testResult.Message = "Ignored";
        //                break;
        //            case Status.NotImplemented:
        //                testResult.State = TestState.Ignored;
        //                testResult.Message = "Not Implemented";
        //                break;
        //            default:
        //                testResult.State = TestState.Failed;
        //                if (result.Exception != null)
        //                {
        //                    testResult.StackTrace = result.Exception.ToString();
        //                }
        //                break;
        //        }

        //    testResults.Add(testResult);
        //}

        //public void OnFatalError(ExceptionResult exception)
        //{
        //    SessionResults.Messages.Add("Fatal error: " + exception);
        //}
    }
}