using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Extensions;
using NSpec.Domain.Formatters;
using Giles.Core.Runners;

namespace Giles.Runner.NSpec
{
    class GilesSessionResultsFormatter : IFormatter, ILiveFormatter
    {
        SessionResults _sessionResults;

        public GilesSessionResultsFormatter(SessionResults sessionResults)
        {
            _sessionResults = sessionResults;
        }

        public void Write(ContextCollection contexts)
        {
            var examples = contexts.Examples().Count();
            var failures = contexts.Failures().Count();

            _sessionResults.SessionRunState = GetSessionRunState(examples, failures);

            contexts.Examples().Each(e => _sessionResults.TestResults.Add(e.ToTestResult()));
        }

        public void Write(Context context)
        {
            if (context.Level == 1) _sessionResults.Messages.Add(Environment.NewLine);

            _sessionResults.Messages.Add(indent.Times(context.Level - 1) + context.Name);
        }

        public void Write(Example e, int level)
        {
            var failure = e.Exception == null ? "" : " - FAILED - {0}".With(e.Exception.CleanMessage());

            var whiteSpace = indent.Times(level);

            var result = e.Pending ? whiteSpace + e.Spec + " - PENDING" : whiteSpace + e.Spec + failure;

            _sessionResults.Messages.Add(result);
        }

        SessionRunState GetSessionRunState(int examples, int failures)
        {
            if (examples == 0) return SessionRunState.NoTests;
            if (failures > 0) return SessionRunState.Failure;
            return SessionRunState.Success;
        }

        const string indent = "  ";
    }

    public static class ExampleExtensions
    {
        public static TestResult ToTestResult(this Example example)
        {
            TestResult testResult = new TestResult()
            {
                Name = example.FullName().Replace("_", " "),
                TestRunner = TESTRUNNER,
                State = GetState(example),
                StackTrace = GetStackTrace(example)
            };
            return testResult;
        }

        static TestState GetState(Example example)
        {
            if (example.Pending) return TestState.Ignored;
            if (example.HasRun && example.Exception == null) return TestState.Passed;
            return TestState.Failed;
        }

        static string GetStackTrace(Example example)
        {
            if (example == null || example.Exception == null) return String.Empty;

            var stackTrace =
                example.Exception
                    .GetOrFallback(e => e.StackTrace, "").Split('\n')
                    .Where(l => !internalNameSpaces.Any(l.Contains));

            return stackTrace.Flatten(Environment.NewLine).TrimEnd() + Environment.NewLine;
        }

        const string TESTRUNNER = "NSPEC";
        static string[] internalNameSpaces =
            new[]
                {
                    "NSpec.Domain",
                    "NSpec.AssertionExtensions",
                    "NUnit.Framework"
                };
    }
}
