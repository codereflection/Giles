using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Giles.Core.Runners;
using NUnit.Core;

namespace Giles.Runner.NUnit
{
    [Serializable]
    public class NUnitRunner : IFrameworkRunner
    {
        #region IFrameworkRunner Members

        public SessionResults RunAssembly( Assembly assembly )
        {
            using (var domainRunner = new DomainRunner())
            {
                TestPackage package = SetupTestPackager( assembly );
                domainRunner.Load( package );
                var listener = new GilesNUnitEventListener();
                domainRunner.Run( listener );
                return listener.SessionResults;
            }
        }

        public IEnumerable<string> RequiredAssemblies()
        {
            return new[]
                   {
                           Assembly.GetAssembly( typeof (NUnitRunner) ).Location,
                           "nunit.core.dll", "nunit.core.interfaces.dll", "nunit.util.dll"
                   };
        }

        #endregion

        private static TestPackage SetupTestPackager(Assembly assembly)
        {
            var package = new TestPackage(assembly.FullName, new[] { assembly.Location });

            package.BasePath = GetAssemblyDirectory(assembly).FullName;
            package.ConfigurationFile = GetTestAssemblyConfigurationFile(assembly);

            return package;
        }

        private static string GetTestAssemblyConfigurationFile( Assembly assembly )
        {
            string configFileName = GetTestAssemblyConfigurationFullName( assembly );
            configFileName = File.Exists( configFileName ) ? configFileName : null;
            return configFileName;
        }

        private static string GetTestAssemblyConfigurationFileName(Assembly assembly)
        {
            string assemblyPath = GetAssemblyFullPath(assembly);
            var fileName = Path.GetFileName(assemblyPath);
            string configFileName = string.Format("{0}.config", fileName);
            return configFileName;
        }

        private static string GetTestAssemblyConfigurationFullName(Assembly assembly)
        {
            string assemblyDirectory = GetAssemblyDirectory(assembly).FullName;
            string configFileName = GetTestAssemblyConfigurationFileName(assembly);
            configFileName = Path.Combine(assemblyDirectory, configFileName);
            return configFileName;
        }

        private static DirectoryInfo GetAssemblyDirectory(Assembly assembly)
        {
            string assemblyPath = GetAssemblyFullPath(assembly);
            var directory = new FileInfo(assemblyPath).Directory;
            return directory;
        }

        private static string GetAssemblyFullPath(Assembly assembly)
        {
            string assemblyPath = new Uri(assembly.CodeBase).LocalPath;
            return assemblyPath;
        }
    }
}