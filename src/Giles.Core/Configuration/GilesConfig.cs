using System.Collections.Generic;
using System.ComponentModel;
using Giles.Core.UI;

namespace Giles.Core.Configuration
{
    public class GilesConfig : INotifyPropertyChanged
    {
        public IDictionary<string, RunnerAssembly> TestRunners = new Dictionary<string, RunnerAssembly>();
        public List<IUserDisplay> UserDisplay = new List<IUserDisplay>();
        private long buildDelay = 500;
        public List<Filter> Filters { get; set; }
        public List<string> TestAssemblies { get; set; }
        public string SolutionPath { get; set; }

        public long BuildDelay
        {
            get { return buildDelay; }
            set
            {
                if (value == buildDelay) return;

                buildDelay = value;
                NotifyPropertyChanged("BuildDelay");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}