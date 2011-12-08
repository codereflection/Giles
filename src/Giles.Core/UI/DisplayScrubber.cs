namespace Giles.Core.UI
{
    public static class DisplayScrubber
    {
        public static string ScrubDisplayStringForFormatting(this string stringToScrub)
        {
            var scrubbed = stringToScrub.Replace("{", "{{");
            scrubbed = scrubbed.Replace("}", "}}");
            return scrubbed;
        }
    }
}