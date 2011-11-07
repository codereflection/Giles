using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Giles.Core.Runners;
using Machine.Specifications;

namespace Giles.Specs.Core.Runners
{
    [Subject(typeof(TestFrameworkResolver))]
    public class with_a_test_framework_resolver
    {
        protected static TestFrameworkResolver subject;

        Establish context = () =>
            subject = new TestFrameworkResolver();
    }

    [Subject(typeof(TestFrameworkResolver))]
    public class when_resolving_for_a_test_runner_in_an_assembly_without_a_test_framework
        : with_a_test_framework_resolver
    {
        static Assembly assembly;
        static IEnumerable<IFrameworkRunner> result;

        Establish context = () =>
            assembly = typeof(TestFrameworkResolver).Assembly;

        Because of = () =>
            result = subject.Resolve(assembly);

        It should_return_an_empty_list_of_framework_runners = () =>
            result.ShouldBeEmpty();
    }

    [Subject(typeof(TestFrameworkResolver))]
    public class when_resolving_for_the_mspec_runner_in_an_assembly_that_references_mspec
        : with_a_test_framework_resolver
    {
        static Assembly assemby;
        static IEnumerable<IFrameworkRunner> result;

        Establish context = () =>
            assemby = typeof(with_a_test_framework_resolver).Assembly;

        Because of = () =>
            result = subject.Resolve(assemby);

        It should_return_a_result = () =>
            result.ShouldNotBeEmpty();

        It should_return_the_mspec_framework_runner = () =>
            result.First().ShouldBeOfType<Giles.Runner.Machine.Specifications.MSpecRunner>();
    }

    [Subject(typeof(TestFrameworkResolver))]
    public class when_resolving_for_a_test_runner_and_an_assembly_is_not_passed
        : with_a_test_framework_resolver
    {
        static Assembly assembly;
        static IEnumerable<IFrameworkRunner> result;

        Establish context = () =>
            assembly = null;

        Because of = () =>
            result = subject.Resolve(assembly);

        It should_return_an_empty_list_of_framework_runners = () =>
            result.ShouldBeEmpty();
    }
}