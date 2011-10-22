using System.Collections.Generic;

namespace Giles.Core.Utility
{
    public class TypeLoader
    {
        /// <summary>
        /// Gets a list of new instances of classes which implement, extend or are of type T
        /// from all of the assemblies in the executing path
        /// </summary>
        /// <returns>List of new instances of classes which implement, extend or are of type T</returns>
        public static IEnumerable<T> GetNewInstancesByType<T>() where T : class
        {
            var files = AssemblyExtensions.GetAssembliesFromExecutingPath();

            var result = new List<T>();

            files.Each( file => result.AddRange( AssemblyExtensions.FromAssemblyGetInstancesOfType<T>( file ) ) );

            return result;
        }
    }
}