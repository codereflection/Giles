using System;
using System.Linq;
using Giles.Core.Configuration;
using Giles.Core.Runners;
using Machine.Specifications;

namespace Giles.Specs.Core.Runners
{
    [Subject("Build Runner")]
    public class with_a_build_runner
    {
        protected static GilesConfig config;
        protected static Settings settings;
        protected static BuildRunner subject;
        protected static FakeUserDisplay fakeUserDisplay;

        Establish context = () =>
            {
                fakeUserDisplay = new FakeUserDisplay();
                config = new GilesConfig { UserDisplay = new[] { fakeUserDisplay } };
                settings = new Settings();
                subject = new BuildRunner(config, settings);
            };
    }

    public class when_asked_to_run_a_build_and_the_build_was_successful : with_a_build_runner
    {
        static ExecutionResult ExecuteHandler(string filename, string args)
        { ExecuteWasCalled = true; return new ExecutionResult { ExitCode = successExitCode }; }

        static int successExitCode;
        static bool ExecuteWasCalled;
        static bool result;

        Establish context = () =>
            {
                successExitCode = 0;
                CommandProcessExecutor.Execute = (filename, args) => ExecuteHandler(filename, args);
            };

        Because of = () =>
            result = subject.Run();

        It should_have_called_execute_on_the_command_executor = () =>
            ExecuteWasCalled.ShouldBeTrue();

        It should_return_success = () =>
            result.ShouldBeTrue();

        It should_display_the_build_complete_message_to_the_user_display = () =>
            fakeUserDisplay.messagesReceived.Any(x => x.Contains("Build complete")).ShouldBeTrue();

        It should_display_a_message_of_success_to_the_user_display = () =>
            fakeUserDisplay.messagesReceived.Any(x => x.Contains("Success")).ShouldBeTrue();
    }

    public class when_asked_to_run_a_build_and_the_build_was_failed : with_a_build_runner
    {
        static ExecutionResult ExecuteHandler(string filename, string args)
        { ExecuteWasCalled = true; return new ExecutionResult { ExitCode = failureExitCode }; }

        static int failureExitCode;
        static bool ExecuteWasCalled;
        static bool result;

        Establish context = () =>
            {
                failureExitCode = 100;
                CommandProcessExecutor.Execute = (filename, args) => ExecuteHandler(filename, args);
            };

        Because of = () =>
            result = subject.Run();

        It should_have_called_execute_on_the_command_executor = () =>
            ExecuteWasCalled.ShouldBeTrue();

        It should_return_failure = () =>
            result.ShouldBeFalse();

        It should_display_the_build_complete_message_to_the_user_display = () =>
            fakeUserDisplay.messagesReceived.Any(x => x.Contains("Build complete")).ShouldBeTrue();

        It should_display_a_message_of_failure_to_the_user_display = () =>
            fakeUserDisplay.messagesReceived.Any(x => x.Contains("Failure")).ShouldBeTrue();
    }
}