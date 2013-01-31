using System;
using Giles.Core.Runners;

namespace Giles.Core.UI
{
    public class ConsoleUserDisplay : IUserDisplay
    {
        readonly ConsoleColor defaultConsoleColor;

        public ConsoleUserDisplay()
        {
            defaultConsoleColor = Console.ForegroundColor;
        }

        public void DisplayMessage(string message, params object[] parameters)
        {
            Console.WriteLine(message.ScrubDisplayStringForFormatting(), parameters);
        }

        public void DisplayResult(ExecutionResult result)
        {
            try
            {
                Console.WriteLine("\n\n======= {0} TEST RUNNER RESULTS =======", result.Runner.RunnerName);

                using(new ChangedColorContext(GetPassedColor(result)))
                {
                    Console.Write(string.Format("Passed: {0}", result.Runner.Stats.Passed));
                }

                Console.Write(", ");

                using (new ChangedColorContext(GetFailedColor(result)))
                {
                    Console.Write(string.Format("Failed: {0}", result.Runner.Stats.Failed));
                }

                Console.Write(", ");

                using (new ChangedColorContext(GetIgnoredColor(result)))
                {
                    Console.Write(string.Format("Ignored: {0}", result.Runner.Stats.Ignored));
                }

                Console.Write(Environment.NewLine);
            }
            finally
            {
                Console.ForegroundColor = defaultConsoleColor;
            }
        }

        public ConsoleColor GetPassedColor(ExecutionResult result)
        {
            return result.Runner.Stats.Passed > 0 ? ConsoleColor.Green : defaultConsoleColor;
        }

        public ConsoleColor GetFailedColor(ExecutionResult result)
        {
            return result.Runner.Stats.Failed > 0 ? ConsoleColor.Red : defaultConsoleColor;
        }

        public ConsoleColor GetIgnoredColor(ExecutionResult result)
        {
            return result.Runner.Stats.Ignored > 0 ? ConsoleColor.Yellow : defaultConsoleColor;
        }

        private class ChangedColorContext : IDisposable
        {
            private readonly ConsoleColor defaultConsoleColor;

            public ChangedColorContext(ConsoleColor color)
            {
                defaultConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
            }

            public void Dispose()
            {
                Console.ForegroundColor = defaultConsoleColor;
            }
        }
    }
}