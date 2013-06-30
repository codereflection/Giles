using System;
using System.Collections.Generic;
using System.Linq;

namespace Giles.Core.Configuration
{
    [Serializable]
    public class Filter
    {
        public Filter() { }

        public Filter(string convertToFilter)
        {
            foreach (var entry in FilterLookUp.Where(entry => convertToFilter.Contains(string.Format(" {0}" ,entry.Key))))
            {
                Type = entry.Value;
                Name = convertToFilter.Replace(string.Format(" {0}", entry.Key), string.Empty).Trim();

                break;
            }

            if (!string.IsNullOrWhiteSpace(Name)) return;

            Name = convertToFilter.Trim();
            Type = FilterType.Inclusive;
        }

        public string Name { get; set; }
        public FilterType Type { get; set; }

        public string NameDll
        {
            get
            {
                return Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? Name : String.Format("{0}.dll", Name);
            }
        }

        public static readonly IDictionary<string, FilterType> FilterLookUp = new Dictionary<string, FilterType>
                                                                           {
                                                                               {"-i", FilterType.Inclusive},
                                                                               {"-e", FilterType.Exclusive}
                                                                           };

        public override bool Equals(object obj)
        {
            var compareTo = (Filter)obj;
            return Name == compareTo.Name && Type == compareTo.Type;
        }

        public override int GetHashCode()
        {
            var name = Enum.GetName(typeof (FilterType), Type);
            if (name != null)
                return Name.GetHashCode() ^ name.GetHashCode();

            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Type: {1}", Name, Enum.GetName(typeof(FilterType), Type));
        }
    }

    public enum FilterType
    {
        Inclusive,
        Exclusive
    }
}