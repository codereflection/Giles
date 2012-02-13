using System;
using System.Collections.Generic;

namespace Giles
{
    public static class UserInputHandler
    {
        public static Action<string> Output = value => Console.WriteLine(value);
        public static Func<string> Input = Console.ReadLine;

        public static List<T> GetUserValuesFor<T>(List<T> defaultValues, string description)
        {
            Output(description);
            Output(string.Format(@"  Current settings: {0}", GetLineSeparatedValueListFor(defaultValues)));
            var newValues = new List<T>();

            string newLine;
            do
            {
                newLine = Input();
                if (!string.IsNullOrWhiteSpace(newLine))
                    newValues.Add((T)Convert.ChangeType(newLine, typeof(T)));
            }
            while (!string.IsNullOrWhiteSpace(newLine));
            return newValues.Count == 0 ? defaultValues : newValues;
        }

        static string GetLineSeparatedValueListFor<T>(List<T> defaultValues)
        {
            var result = "";
            defaultValues.ForEach(x => result += Environment.NewLine + x.ToString());
            return result;
        }
    }
}