using System.Collections.Generic;
using System.Linq;

namespace Giles.Core.Configuration
{
    public class Filter
    {
        public Filter() { }

        public Filter(string convertToFilter)
        {
            foreach (var entry in FilterLookUp.Where(entry => convertToFilter.Contains(entry.Key)))
            {
                Type = entry.Value;
                Name = convertToFilter.Replace(entry.Key, string.Empty);

                break;
            }
        }

        public string Name { get; set; }
        public FilterType Type { get; set; }

        public static readonly IDictionary<string, FilterType> FilterLookUp = new Dictionary<string, FilterType>
                                                                           {
                                                                               {"-i", FilterType.Inclusive},
                                                                               {"-e", FilterType.Exclusive}
                                                                           };
    }

    public enum FilterType
    {
        Inclusive,
        Exclusive
    }

    
}