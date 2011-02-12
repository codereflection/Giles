using System.Configuration;

namespace Giles.Core.Configuration
{
    public class Settings
    {
        public string MsBuild
        {
            get { return ConfigurationManager.AppSettings["MSBuild"]; }
        }
    }
}