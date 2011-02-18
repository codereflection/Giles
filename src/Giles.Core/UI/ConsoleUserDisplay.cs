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
            Console.WriteLine(message, parameters);
        }

        public void DisplayResult(ExecutionResult result)
        {
            Console.WriteLine("\n\n======= {0} TEST RUNNER RESULTS =======", result.Runner);
            Console.ForegroundColor = result.ExitCode != 0 ?
                                      ConsoleColor.Red : defaultConsoleColor;

            Console.WriteLine(result.Output);

            Console.ForegroundColor = defaultConsoleColor;
        }
    }
}