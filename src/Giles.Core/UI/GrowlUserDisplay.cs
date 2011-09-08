using System;
using Giles.Core.Runners;
using Growl.Connector;

namespace Giles.Core.UI
{
    public class GrowlUserDisplay : IUserDisplay
    {
        private GrowlConnector growl;
        private NotificationType notificationType;
        private Application application;

        public GrowlUserDisplay()
        {
            Initialize();
        }

        void Initialize()
        {
            notificationType = new NotificationType("BUILD_RESULT_NOTIFICATION", "Sample Notification");
            application = new Application("Giles");
            growl = new GrowlConnector
                        {
                            EncryptionAlgorithm = Cryptography.SymmetricAlgorithmType.PlainText
                        };
            

            growl.Register(application, new[] { notificationType });
        }

        public void DisplayMessage(string message, params object[] parameters)
        {
            const string title = "Giles says...";
            var text = string.Format(message, parameters);
            var notification = new Notification(application.Name, notificationType.Name, DateTime.Now.Ticks.ToString(), title, text);
            growl.Notify(notification);
        }

        public void DisplayResult(ExecutionResult result)
        {
            var title = result.ExitCode == 0 ? "Success!" : "Failures!";
            var notification = new Notification(application.Name, notificationType.Name, DateTime.Now.Ticks.ToString(), title, result.Output);
            growl.Notify(notification);
        }
    }
}