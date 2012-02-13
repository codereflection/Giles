using System;
using System.Collections.Generic;
using System.Linq;
using Giles.Core.Utility;

namespace Giles
{
    public static class UserInputHandler
    {
        public static Action<string> Output = value => Console.WriteLine(value);
        public static Func<string> Input = Console.ReadLine;
        const string instructions = @"Enter one value on each line. Use a blank line save.
Modifiers:
    +[newValue] to add a value to the current settings
    -[newValue] to remove a value from the current settings";

        public static List<string> GetUserValuesFor(List<string> defaultValues, string description)
        {
            Output(description);
            Output("");
            Output(instructions);
            Output(string.Format("  Current settings: {0}{1}", GetLineSeparatedValueListFor(defaultValues), Environment.NewLine));
            var newValues = new List<string>();

            var modifyingDefaultValues = false;
            string newLine;
            do
            {
                newLine = Input();
                if (string.IsNullOrWhiteSpace(newLine)) continue;

                if (modifyingDefaultValues == false)
                    modifyingDefaultValues = ContainsAModifier(newLine);
                newValues.Add(newLine);
            }
            while (!string.IsNullOrWhiteSpace(newLine));

            if (newValues.Count == 0) return defaultValues;

            if (modifyingDefaultValues)
                return GetModifiedList(defaultValues, newValues).ToList();

            return newValues;
        }

        static IEnumerable<string> GetModifiedList(IEnumerable<string> defaultValues, IEnumerable<string> newValues)
        {
            var modifiedList = new List<string>(defaultValues);

            modifiedList.AddRange(RemoveModifiersFrom(AddedValues(newValues)));

            RemoveModifiersFrom(newValues.Where(x => x.StartsWith("-")))
                .Each(x => modifiedList.Remove(x));

            return modifiedList;
        }

        static IEnumerable<string> AddedValues(IEnumerable<string> newValues)
        {
            return newValues.Where(x => x.StartsWith("-") == false);
        }

        static IEnumerable<string> RemoveModifiersFrom(IEnumerable<string> newValues)
        {
            return newValues.Select(x => x.Replace("+", string.Empty).Replace("-", string.Empty));
        }

        static bool ContainsAModifier(string newLine)
        {
            return newLine.StartsWith("+") || newLine.StartsWith("-");
        }

        static string GetLineSeparatedValueListFor(List<string> defaultValues)
        {
            var result = "";
            defaultValues.ForEach(x => result += Environment.NewLine + "\t" + x);
            return result;
        }
    }
}