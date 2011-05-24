using System;
using System.Linq;
using System.Reflection;

namespace Giles.Core.Utility
{
    internal static class AssemblyExtensions
    {
        internal static bool AnyMatchesRequirementFor(this AssemblyName[] assemblyNames,
                                                   Func<AssemblyName, bool> requirement)
        {
            return assemblyNames.Any(requirement.Invoke);
        }
    }
}