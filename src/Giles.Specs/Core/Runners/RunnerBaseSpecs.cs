using System.Diagnostics;
using Giles.Core.Runners;
using Machine.Specifications;

namespace Giles.Specs.Core.Runners
{
    public class when_setting_up_a_process
    {
        static RunnerBase runner;
        static Process result;
        static string fileName;
        static string arguments;

        Establish context = () =>
                                {
                                    runner = new RunnerBase();
                                    fileName = "test.exe";
                                    arguments = "/runtest";
                                };
        Because of = () =>
            result = runner.SetupProcess(fileName, arguments);

        It should_have_a_result = () =>
            result.ShouldNotBeNull();

        It sets_the_file_name = () =>
            result.StartInfo.FileName.ShouldEqual(fileName);

        It sets_the_arguements = () =>
            result.StartInfo.Arguments.ShouldEqual(arguments);

        It does_not_use_shell_execute = () =>
            result.StartInfo.UseShellExecute.ShouldBeFalse();

        It redirects_standard_output = () =>
            result.StartInfo.RedirectStandardOutput.ShouldBeTrue();
			
    }
}