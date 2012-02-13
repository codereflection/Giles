using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Giles.Specs.Console
{
    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_a_new_value_is_entered
    {
        static List<string> result;
        static List<string> userValues;
        static Queue<string> stack;
        static readonly List<string> Messages = new List<string>();
        static readonly List<string> DefaultValues = new List<string> { "value1" };

        Establish context = () =>
            {
                userValues = new List<string> { "newValue", Environment.NewLine };
                stack = new Queue<string>(userValues);
                UserInputHandler.Output = value => Messages.Add(value);
                UserInputHandler.Input = () => stack.Dequeue();
            };

        Because of = () =>
            result = UserInputHandler.GetUserValuesFor(DefaultValues, "The prompt");

        It should_get_the_user_entered_values = () =>
            result.ShouldContainOnly(userValues.First());
    }

    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_the_default_setting_is_accepted
    {
        static readonly List<string> Messages = new List<string>();
        static readonly List<string> DefaultValues = new List<string> { "value1" };
        static List<string> result;

        Establish context = () =>
            {
                UserInputHandler.Output = value => Messages.Add(value);
                UserInputHandler.Input = () => Environment.NewLine;
            };

        Because of = () =>
            result = UserInputHandler.GetUserValuesFor(DefaultValues, "The prompt");

        It should_save_the_default_values = () =>
            result.ShouldContainOnly(DefaultValues);
    }
}