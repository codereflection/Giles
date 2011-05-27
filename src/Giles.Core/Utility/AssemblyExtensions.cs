using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Giles.Core.Runners;

namespace Giles.Core.Utility
{
    public static class AssemblyExtensions
    {
        internal static bool AnyMatchesRequirementFor(this AssemblyName[] assemblyNames,
                                                   Func<AssemblyName, bool> requirement)
        {
            return assemblyNames.Any(requirement.Invoke);
        }

        public static IEnumerable<T> FromAssemblyGetInstancesOfType<T>(string file) where T : class 
        {
            var assembly = Assembly.LoadFrom(file);

            var types = GetRelevantTypes<T>(assembly);

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

        static List<Type> GetRelevantTypes<T>(Assembly assembly)
        {
            var result = new List<Type>();

            assembly.GetTypes()
                .Each(x =>
                {
                    if (x.IsAbstract || x.IsInterface)
                        return;

                    if (IsSubclassOf<T>(x) || IsImplementationOf<T>(x))
                        result.Add(x);
                });
            return result;
        }

        static bool IsSubclassOf<T>(Type type)
        {
            return type.IsSubclassOf(typeof(T));
        }

        static bool IsImplementationOf<T>(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static IEnumerable<string> GetAssembliesFromExecutingPath()
        {
            var location = 
                Path.GetDirectoryName(typeof(TestFrameworkResolver).Assembly.Location);

            return Directory.GetFiles(location, "*.dll");
        }
    }
}