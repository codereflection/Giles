using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Giles.Options;
using Machine.Specifications;

namespace Giles.Specs.Console.Options
{
    public class with_command_line_options
    {
        protected static CLOptions options;
        protected static List<string> result;
        static CommandLineParser parser;
        protected static string[] commandLineArgs;

        protected static void SetupOptions()
        {
            options = new CLOptions();
            parser = new CommandLineParser(new CommandLineParserSettings(false));
            parser.ParseArguments(commandLineArgs, options);
        }

    }

    public class with_a_single_test_assembly : with_command_line_options
    {
        Establish context = () =>
            {
                commandLineArgs = new[] { "-s", "mySolution.sln", "-t", "myTestAssembly.dll" };
                SetupOptions();       
            };

        Because of = () =>
            result = options.GetTestAssemblies();

        It should_locate_the_test_assembly_passed_ = () =>
            result.ShouldContain("myTestAssembly.dll");
    }

    public class with_a_multiple_test_assemblies : with_command_line_options
    {
        Establish context = () =>
            {
                commandLineArgs = new[] { "-s", "mySolution.sln", "-t", "myTestAssembly.dll,myOtherTestAssembly.dll" };
                SetupOptions();       
            };

        Because of = () =>
            result = options.GetTestAssemblies();

        It should_locate_the_first_test_assembly_passed_ = () =>
            result.ShouldContain("myTestAssembly.dll");

        It should_locate_the_second_test_assembly_passed_ = () =>
            result.ShouldContain("myOtherTestAssembly.dll");
    }

    public class when_the_test_assembly_path_is_surrounded_by_double_quotes : with_command_line_options
    {
        Establish context = () =>
            {
                commandLineArgs = new[] { "-s", "mySolution.sln", "-t", "\"myTestAssembly.dll\"" };
            };

        Because of = () =>
            result = options.GetTestAssemblies();

        It should_remove_the_double_quotes_from_the_test_assembly_path = () =>
            result.First().ShouldEqual("myTestAssembly.dll");
    }

    public class when_the_test_assembly_path_is_surrounded_by_single_quotes : with_command_line_options
    {
        Establish context = () =>
        {
            commandLineArgs = new[] { "-s", "mySolution.sln", "-t", "'myTestAssembly.dll'" };
        };

        Because of = () =>
            result = options.GetTestAssemblies();

        It should_remove_the_single_quotes_from_the_test_assembly_path = () =>
            result.First().ShouldEqual("myTestAssembly.dll");
    }

    
}