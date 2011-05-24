using System;
using System.Reflection;

namespace Giles.Core.Runners
{
    internal class FrameworkRunnerLocator
    {
        internal Func<AssemblyName, bool> Requirement { get; set; }
        internal Func<IFrameworkRunner> GetTheRunner { get; set; }
    }
}