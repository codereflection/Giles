using System.Configuration;

namespace ClassLibraryWithTests
{
    public class SpiderMan : SuperHero
    {
        public override string SayCatchPhrase()
        {
            return ConfigurationManager.AppSettings["SpiderMan"];
        }
    }
}
