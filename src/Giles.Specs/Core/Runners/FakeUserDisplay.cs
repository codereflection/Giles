using System.Collections.Generic;
using Giles.Core.Runners;
using Giles.Core.UI;

namespace Giles.Specs.Core.Runners
{
    public class FakeUserDisplay : IUserDisplay
    {
        public IList<string> messagesReceived = new List<string>();
        public IList<ExecutionResult> executionResultsReceived = new List<ExecutionResult>();

        public void DisplayMessage(string message, params object[] parameters)
        {
            messagesReceived.Add(string.Format(message, parameters));
        }

        public void DisplayResult(ExecutionResult result)
        {
            executionResultsReceived.Add(result);
        }
    }
}