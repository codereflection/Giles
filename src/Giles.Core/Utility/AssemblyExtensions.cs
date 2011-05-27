using System;
using System.Collections.Generic;
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

        public static IEnumerable<T> FromAssemblyGetInstancesOfType<T>(string file) where T : class 
        {
            var assembly = Assembly.LoadFrom(file);
            var types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T))).ToList();

            if (types.Count == 0)
                return Enumerable.Empty<T>();

            var result = new List<T>();
            types.Each(type =>
                           {
                               var instance = Activator.CreateInstance(type);
                               result.Add((T)instance);
                           });
            return result;
        }
    }
}