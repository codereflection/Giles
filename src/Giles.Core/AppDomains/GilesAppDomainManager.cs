using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Giles.Core.IO;
using Giles.Core.Runners;
using Giles.Core.Utility;

namespace Giles.Core.AppDomains
{
    public class GilesAppDomainManager
    {
        private AppDomain appDomain;
        private string testAssemblyFolder;
        public SessionResults SessionResults { get; set; }

        public IEnumerable<SessionResults> Run(string testAssemblyPath)
        {
            var runner = SetupRunner(testAssemblyPath);

            var results = runner.Run(testAssemblyPath);

            CleanupRunner();

            return results;
        }

        private void CleanupRunner()
        {
            AppDomain.Unload(appDomain);

            RemoveGilesFromTheTestAssemblyFolder(testAssemblyFolder);
        }

        private GilesAppDomainRunner SetupRunner(string testAssemblyPath)
        {
            testAssemblyFolder = Path.GetDirectoryName(testAssemblyPath);

            CopyGilesToTheTestAssemblyFolder(testAssemblyFolder);

            SetupAppDomain();

            return GetRunner();
        }

        private GilesAppDomainRunner GetRunner()
        {
            var runType = typeof (GilesAppDomainRunner);

            return appDomain.CreateInstanceAndUnwrap(runType.Assembly.FullName, runType.FullName) as GilesAppDomainRunner;
        }

        private void SetupAppDomain()
        {
            var domainInfo = new AppDomainSetup
                                 { ApplicationBase = testAssemblyFolder };

            appDomain = AppDomain.CreateDomain("GilesAppDomainRunner", AppDomain.CurrentDomain.Evidence, domainInfo);
        }

        private static void CopyGilesToTheTestAssemblyFolder(string testAssemblyFolder)
        {
            GilesAssemblyFileOperation(testAssemblyFolder, FileOperationType.Copy);
        }

        private static void RemoveGilesFromTheTestAssemblyFolder(string testAssemblyFolder)
        {
            GilesAssemblyFileOperation(testAssemblyFolder, FileOperationType.Delete);
        }

        private static void GilesAssemblyFileOperation(string testAssemblyFolder, FileOperationType fileOperationType)
        {
            var fileSystem = new FileSystem();
            var filesToCopy = GetGilesAssembliesToUse();
            
            filesToCopy.Each(file =>
                                 {
                                     var targetPath = Path.Combine(testAssemblyFolder, fileSystem.GetFileName(file));
                                     
                                     if (fileOperationType == FileOperationType.Copy || fileOperationType == FileOperationType.Delete)
                                         if (fileSystem.FileExists(targetPath))
                                             fileSystem.DeleteFile(targetPath);

                                     if (fileOperationType == FileOperationType.Copy)
                                         fileSystem.CopyFile(file, targetPath);
                                 });
        }

        private static IEnumerable<string> GetGilesAssembliesToUse()
        {
            return new[]
                       {
                           typeof(GilesAppDomainRunner).Assembly.Location,
                           Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Giles.Runner.Machine.Specifications.dll"),
                           Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Giles.Runner.NUnit.dll"),
                           Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "nunit.core.dll"),
                           Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "nunit.core.interfaces.dll"),
                       };
        }
    }
}