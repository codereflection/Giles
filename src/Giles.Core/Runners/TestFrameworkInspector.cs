using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Giles.Core.Runners
{
    /// <summary>
    /// A Giles Inspector provides a method to determine if the target assembly meets the requirements to be a valid test assembly for
    ///    the runner (i.e. - requiring that the assembly has a reference to nunit.framework)
    /// </summary>
    public abstract class TestFrameworkInspector
    {
        /// <summary>
        /// Passes a referenced assembly from the target assembly
        /// Must return true if the referenced assembly meets the requirements of the test framework runner (i.e. = having a reference to nunit.framework)
        /// </summary>
        public abstract Func<AssemblyName, bool> Requirement { get; }
    }
}