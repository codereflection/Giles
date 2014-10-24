using System;
using System.Drawing;
using System.Reflection;
using Giles.Core.Runners;
using Growl.Connector;
using Growl.CoreLibrary;

namespace Giles.Core.UI
{
    public class GrowlUserDisplay : IUserDisplay
    {
        private GrowlConnector growl;
        private NotificationType informationNotificationType;
        private NotificationType successNotificationType;
        private NotificationType failureNotificationType;
        private Application application;
        private const string successImage = "Giles.Core.Resources.checkmark.png";
        private const string failureImage = "Giles.Core.Resources.stop.png";

        public GrowlUserDisplay()
        {
            Initialize();
        }

        void Initialize()
        {
            informationNotificationType = new NotificationType("BUILD_RESULT_NOTIFICATION", "Sample Notification");
            successNotificationType = new NotificationType("SUCCESS_NOTIFICATION", "Success Notification");
            failureNotificationType = new NotificationType("FAILURE_NOTIFICATION", "Failure Notification");

            application = new Application("Giles");
            growl = new GrowlConnector
                        {
                            EncryptionAlgorithm = Cryptography.SymmetricAlgorithmType.PlainText
                        };


            growl.Register(application,
                new[] {
                    informationNotificationType,
                    successNotificationType,
                    failureNotificationType
                });
        }

        public void DisplayMessage(string message, params object[] parameters)
        {
            const string title = "Giles says...";
            var text = string.Format(message.ScrubDisplayStringForFormatting(), parameters);
            var notification = new Notification(application.Name, informationNotificationType.Name, DateTime.Now.Ticks.ToString(), title, text);
            growl.Notify(notification);
        }

        public void DisplayResult(ExecutionResult result)
        {
            var isSuccess = result.ExitCode == 0;

            var title = isSuccess ? "Success!" : "Failures!";
            var notifyType = isSuccess ? successNotificationType : failureNotificationType;

            Resource icon = result.ExitCode == 0 ? LoadImage(successImage) : LoadImage(failureImage);
            var notification = new Notification(application.Name, notifyType.Name, DateTime.Now.Ticks.ToString(), title, result.Runner.ToString()) { Icon = icon };
            growl.Notify(notification);
        }

        private static Image LoadImage(string resourceName)
        {
            var file = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            return Image.FromStream(file);
        }
    }
}