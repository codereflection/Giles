using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Giles.Core.Runners;
using Giles.Core.UI;

namespace Giles.Core.Configuration
{
    public class GilesConfig : INotifyPropertyChanged
    {
        public IDictionary<string, RunnerAssembly> TestRunners = new Dictionary<string, RunnerAssembly>();

        public IEnumerable<IUserDisplay> UserDisplay = Enumerable.Empty<IUserDisplay>();
        
        public string TestAssemblyPath { get; set; }
        
        public string SolutionPath { get; set; }

        public string ProjectRoot { get; set; }

        long buildDelay = 500;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged == null) return;
            
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        public long BuildDelay
        {
            get
            {
                return buildDelay;
            }
            set
            {
                if (value == buildDelay) return;
                
                buildDelay = value;
                NotifyPropertyChanged("BuildDelay");
            }
        }

        public ICommandProcessExecutor Executor { get; set; }

        public Assembly TestAssembly { get; set; }
    }
}