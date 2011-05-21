using System;
using System.Reflection;

namespace Giles.Core.Runners
{
    internal class FrameworkRunnerLocator
    {
        internal Func<AssemblyName, bool> CheckReference { get; set; }
        internal Func<IFrameworkRunner> GetTheRunner { get; set; }
    }
}