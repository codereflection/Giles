using NUnit.Framework;

namespace ClassLibraryWithTests
{
    public class SpiderManTests
    {
        [Test]
        public void SpiderMan_should_know_his_own_catch_phrase()
        {
            var spidy = new SpiderMan();

            var thePhrase = spidy.SayCatchPhrase();

            Assert.That(thePhrase, Is.EqualTo("It's your Friendly Neighborhood Spider-Man!"));
        }
    }
}
