using System.Collections.Generic;
using Giles.Core.Runners;
using Giles.Core.UI;

namespace Giles.Specs.Core.Runners
{
    public class FakeUserDisplay : IUserDisplay
    {
        public IList<string> DisplayMessagesReceived = new List<string>();
        public IList<ExecutionResult> DisplayResultsReceived = new List<ExecutionResult>();

        public void DisplayMessage(string message, params object[] parameters)
        {
            DisplayMessagesReceived.Add(string.Format(message, parameters));
        }

        public void DisplayResult(ExecutionResult result)
        {
            DisplayResultsReceived.Add(result);
        }
    }
}