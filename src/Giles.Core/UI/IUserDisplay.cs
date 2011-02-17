using Giles.Core.Runners;

namespace Giles.Core.UI
{
    public interface IUserDisplay
    {
         void DisplayMessage(string message, params object[] parameters);
        void DisplayResult(ExecutionResult result);
    }
}