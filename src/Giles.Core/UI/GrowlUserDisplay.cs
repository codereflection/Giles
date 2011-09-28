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
        private NotificationType notificationType;
        private Application application;
        private const string successImage = "Giles.Core.Resources.checkmark.png";
        private const string failureImage = "Giles.Core.Resources.stop.png";

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
            Resource icon = result.ExitCode == 0 ? LoadImage(successImage) : LoadImage(failureImage);
            var notification = new Notification(application.Name, notificationType.Name, DateTime.Now.Ticks.ToString(), title, result.Output) { Icon = icon };
            growl.Notify(notification);
        }

        private static Image LoadImage(string resourceName)
        {
            var file = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            return Image.FromStream(file);
        }
    }
}