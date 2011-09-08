using System;
using System.IO;
using System.Reflection;

namespace Giles.Specs
{
    public class TestResources
    {
        /// <param name="resourcePath">The full namespace and file name.</param>
        public static string ReadAllText(string resourcePath)
        {
            using (var reader = Read(resourcePath))
            using (var sr = new StreamReader(reader))
            {
                return sr.ReadToEnd();
            }
        }

        /// <param name="resourcePath">The full namespace and file name.</param>
        public static Stream Read(string resourcePath)
        {
            var stream = TestAssembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
                throw new ArgumentException("Invalid resource path: " + resourcePath, "resourcePath");
            return stream;
        }

        private static readonly Assembly TestAssembly = typeof(TestResources).Assembly;
    }
}