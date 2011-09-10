using System;
using System.IO;
using System.Linq;
using System.Text;
using Giles.Core.Configuration;
using Giles.Core.Runners;
using Machine.Specifications;

namespace Giles.Specs.Core.Runners
{
    public class with_a_giles_test_listener
    {
        protected const string testMessage = "The monkies have tested the cheese, and the cheese is good.";
        protected const string testCategory = "monkeyCategory";
        protected static GilesConfig config;
        protected static GilesTestListener listener;
        protected static FakeUserDisplay fakeUserDisplay;

        Establish context = () =>
        {
            fakeUserDisplay = new FakeUserDisplay();
            config = new GilesConfig();
            config.UserDisplay.Add(fakeUserDisplay);

            consoleOutputBuffer = new StringBuilder();
            consoleOutputWriter = new StringWriter(consoleOutputBuffer);
            Console.SetOut(consoleOutputWriter);

            listener = new GilesTestListener(config);
        };

        protected static TextWriter consoleOutputWriter;
        protected static StringBuilder consoleOutputBuffer;
    }

    public class when_writing_a_line_to_the_test_listener : with_a_giles_test_listener
    {
        Because of = () =>
            listener.WriteLine(testMessage, testCategory);

        It should_put_the_message_in_the_test_listener_output_in_the_correct_category = () =>
            listener.GetOutput()[testCategory].ToString().Contains(testMessage);
    }

    public class when_displaying_the_verbose_test_results : with_a_giles_test_listener
    {
        Establish context = () => 
            listener.WriteLine(testMessage, testCategory);

        Because of = () =>
            listener.DisplayVerboseResults();

        It should_write_the_verbose_results_out_to_the_console = () =>
            consoleOutputBuffer.ToString().ShouldNotBeEmpty();
    }

    public class when_displaying_the_results : with_a_giles_test_listener
    {
        Establish context = () =>
            listener.AddTestSummary(new TestResult() { Message = testMessage, TestRunner = "RakeFunner", State = TestState.Failed });

        Because of = () =>
            listener.DisplayResults();

        It should_display_the_messages_on_the_User_Display = () =>
            fakeUserDisplay.DisplayResultsReceived.ShouldNotBeEmpty();

        It should_display_the_number_of_successful_tests = () =>
            fakeUserDisplay.DisplayResultsReceived.Count(x => x.Output.Contains("Passed: 0")).ShouldEqual(1);

        It should_display_the_number_of_failed_tests = () =>
            fakeUserDisplay.DisplayResultsReceived.Count(x => x.Output.Contains("Failed: 1")).ShouldEqual(1);

        It should_display_the_number_of_ignored_tests = () =>
            fakeUserDisplay.DisplayResultsReceived.Count(x => x.Output.Contains("Ignored: 0")).ShouldEqual(1);

    }

    public class when_displaying_all_the_errors_from_the_last_test_run : with_a_giles_test_listener
    {
        const string testName = "FailingTestThatFails";
        const string testRunner = "FakeRunner";

        Establish context = () =>
            listener.AddTestSummary(new TestResult
                                        {
                                            Message = testMessage, 
                                            TestRunner = testRunner, 
                                            State = TestState.Failed, 
                                            Name = testName
                                        });

        Because of = () =>
            listener.DisplayErrors();

        It should_display_the_failing_test_name = () =>
            consoleOutputBuffer.ToString().ShouldContain(testName);

        It should_display_the_failing_test_message = () =>
            consoleOutputBuffer.ToString().ShouldContain(testMessage);

    }
}