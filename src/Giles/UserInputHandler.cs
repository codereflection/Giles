using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giles.Core.Configuration;
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

        public static List<Filter> GetUserValuesFor(List<Filter> defaultValues, string description)
        {
            Output(description);
            Output("");
            Output(instructions);
            Output(string.Format("  Current settings: {0}{1}", GetLineSeparatedValueListForFilters(defaultValues), Environment.NewLine));
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

            return modifyingDefaultValues 
                ? GetModifiedList(defaultValues, newValues).ToList() 
                : newValues.Select(x => new Filter(x)).ToList();
        }

        static IEnumerable<Filter> GetModifiedList(IEnumerable<Filter> defaultValues, IEnumerable<string> newValues)
        {
            var valuesList = newValues as IList<string> ?? newValues.ToList();
            var modifiedList = new List<Filter>(defaultValues);

            modifiedList.AddRange(ConvertToFilters(RemoveModifiersFrom(FilterValues(valuesList, false))));

            RemoveModifiersFrom(FilterValues(valuesList, true)).Each(x => modifiedList.Remove(new Filter(x)));

            return modifiedList;
        }

        static IEnumerable<string> FilterValues(IEnumerable<string> newValues, bool isStartsWithRemove)
        {
            return newValues.Where(x => x.Trim().StartsWith("-") == isStartsWithRemove).AsParallel();
        }

        static IEnumerable<string> RemoveModifiersFrom(IEnumerable<string> newValues)
        {
            foreach (var value in newValues.Select(x => x.Replace("+", string.Empty)).AsParallel())
            {
                if (value.Trim().StartsWith("-", StringComparison.OrdinalIgnoreCase))
                    yield return value.Substring(1, value.Length - 1).Trim();
                else
                    yield return value;
            }
        }

        static IEnumerable<Filter> ConvertToFilters(IEnumerable<string> newValues)
        {
            return newValues.Select(value => new Filter(value));
        }

        static bool ContainsAModifier(string newLine)
        {
            return newLine.StartsWith("+") || newLine.StartsWith("-");
        }

        static string GetLineSeparatedValueListForFilters(List<Filter> defaultValues)
        {
            var sb = new StringBuilder();

            if (defaultValues.Any())
                sb.Append(String.Format("{0}\t", Environment.NewLine));
            
            defaultValues.ForEach(x => sb.Append(String.Format("{0} ({1}) {2} \t", x.Name, x.Type.ToString(), Environment.NewLine)));
            return sb.ToString();
        }
    }
}