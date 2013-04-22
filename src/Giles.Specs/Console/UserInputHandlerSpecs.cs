using System;
using System.Collections.Generic;
using System.Linq;
using Giles.Core.Configuration;
using Machine.Specifications;

namespace Giles.Specs.Console
{
    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_a_new_value_is_entered
    {
        static List<Filter> result;
        static List<string> userValues;
        static Queue<string> queue;
        static readonly List<string> Messages = new List<string>();
        private static readonly List<Filter> DefaultValues = new List<Filter> {new Filter {Name = "value1"}};

        Establish context = () =>
            {
                userValues = new List<string> { "newValue", Environment.NewLine };
                queue = new Queue<string>(userValues);
                UserInputHandler.Output = value => Messages.Add(value);
                UserInputHandler.Input = () => queue.Dequeue();
            };

        Because of = () =>
            result = UserInputHandler.GetUserValuesFor(DefaultValues, "The prompt");

        It should_get_the_user_entered_values = () =>
            result[0].ShouldEqual(DefaultValues[0]);
    }

    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_the_default_setting_is_accepted
    {
        static readonly List<string> Messages = new List<string>();
        private static readonly List<Filter> DefaultValues = new List<Filter> {new Filter {Name = "value1"}};
        static List<Filter> result;

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

    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_the_user_adds_to_the_default_values
    {
        static List<Filter> result;
        static List<string> userValues;
        static Queue<string> queue;
        static readonly List<string> Messages = new List<string>();
        private static readonly List<Filter> DefaultValues = new List<Filter> {new Filter {Name = "value1"}};

        Establish context = () =>
            {
                userValues = new List<string> { "+newValue", Environment.NewLine };
                queue = new Queue<string>(userValues);
                UserInputHandler.Output = value => Messages.Add(value);
                UserInputHandler.Input = () => queue.Dequeue();
            };

        Because of = () =>
            result = UserInputHandler.GetUserValuesFor(DefaultValues, "The Prompt");

        It should_have_both_the_default_values = () => 
            result.ShouldContain(DefaultValues.ToArray());

        It should_add_the_new_items_and_remove_the_add_item_operator = () =>
            result.FirstOrDefault(x => x.Name.EndsWith("newValue")).Name.StartsWith("+").ShouldBeFalse();
    }

    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_the_user_adds_to_the_default_values_with_only_one_value_having_a_modifier
    {
        static List<Filter> result;
        static List<string> userValues;
        static Queue<string> queue;
        static readonly List<string> Messages = new List<string>();
        private static readonly List<Filter> DefaultValues = new List<Filter> {new Filter {Name = "value1"}};

        Establish context = () =>
            {
                userValues = new List<string> { "+newValue1", "newValue2", Environment.NewLine };
                queue = new Queue<string>(userValues);
                UserInputHandler.Output = value => Messages.Add(value);
                UserInputHandler.Input = () => queue.Dequeue();
            };

        Because of = () =>
            result = UserInputHandler.GetUserValuesFor(DefaultValues, "The Prompt");

        It should_have_both_the_default_values = () => 
            result.ShouldContain(DefaultValues.ToArray());

        It should_add_the_new_items_with_and_without_a_modifier = () =>
            {
                result.ShouldContain("newValue1");
                result.ShouldContain("newValue2");
            };
    }

    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_the_user_removes_from_the_default_values
    {
        static List<Filter> result;
        static List<string> userValues;
        static Queue<string> queue;
        static readonly List<string> Messages = new List<string>();

        private static readonly List<Filter> DefaultValues = new List<Filter>
                                                                 {
                                                                     new Filter {Name = "value1"},
                                                                     new Filter {Name = "valueToRemove"}
                                                                 };

        Establish context = () =>
            {
                userValues = new List<string> { "-valueToRemove", Environment.NewLine };
                queue = new Queue<string>(userValues);
                UserInputHandler.Output = value => Messages.Add(value);
                UserInputHandler.Input = () => queue.Dequeue();
            };

        Because of = () =>
            result = UserInputHandler.GetUserValuesFor(DefaultValues, "The Prompt");

        It should_remove_the_correct_value = () =>
            result.ShouldNotContain("valueToRemove");

        It should_maintain_the_other_values = () =>
            result.ShouldContain("value1");       
    }

    [Subject(typeof(UserInputHandler))]
    public class when_getting_a_list_of_user_values_and_the_user_adds_and_removes_values_from_the_default_values
    {
        static List<Filter> result;
        static List<string> userValues;
        static Queue<string> queue;
        static readonly List<string> Messages = new List<string>();

        private static readonly List<Filter> DefaultValues = new List<Filter>
                                                                 {
                                                                     new Filter {Name = "value1"},
                                                                     new Filter {Name = "value2"},
                                                                     new Filter {Name = "value3"}
                                                                 };

        Establish context = () =>
        {
            userValues = new List<string> { "-value2", "+value4", Environment.NewLine };
            queue = new Queue<string>(userValues);
            UserInputHandler.Output = value => Messages.Add(value);
            UserInputHandler.Input = () => queue.Dequeue();
        };

        Because of = () =>
            result = UserInputHandler.GetUserValuesFor(DefaultValues, "The Prompt");

        It should_remove_the_correct_value = () =>
            result.ShouldNotContain("value2");

        It should_maintain_the_values_not_added_or_removed = () =>
            {
                result.ShouldContain("value1");
                result.ShouldContain("value3");
            };

        It should_add_the_new_value = () =>
            result.ShouldContain("value4");
    }
}