﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            IEnumerable<SessionResults> results = new List<SessionResults>();
            GilesAppDomainRunner runner;

            try
            {
                runner = SetupRunner(testAssemblyPath);
                results = runner.Run(testAssemblyPath);
            }
            catch(Exception e)
            {
                Console.WriteLine("Giles encountered an error while running the tests:\n\n{0}", e);
            }
            finally
            {
                CleanupRunner();
            }

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
                                 {
                                     ApplicationBase = testAssemblyFolder,
                                     PrivateBinPath = "Giles"
                                 };

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
            
            var gilesTargetAssemblyFolder = Path.Combine(testAssemblyFolder, @"Giles");
            if (!Directory.Exists(gilesTargetAssemblyFolder))
                Directory.CreateDirectory(gilesTargetAssemblyFolder);

            filesToCopy.Each(f =>
                                 {
                                     var sourcePath = f.Contains("\\") ? f : GetFileSourceLocation(f);
                                     var targetPath = Path.Combine(gilesTargetAssemblyFolder, fileSystem.GetFileName(f));
                                     
                                     if (fileOperationType == FileOperationType.Copy || fileOperationType == FileOperationType.Delete)
                                         if (fileSystem.FileExists(targetPath))
                                             fileSystem.DeleteFile(targetPath);

                                     if (fileOperationType == FileOperationType.Copy)
                                     {
                                         fileSystem.CopyFile(sourcePath, targetPath);
                                     }
                                 });
        }

        static string GetFileSourceLocation(string f)
        {
            var location = Path.GetDirectoryName(typeof(GilesAppDomainManager).Assembly.Location);
            return Path.Combine(location, f);
        }

        private static IEnumerable<string> GetGilesAssembliesToUse()
        {
            var runners = TestFrameworkResolver.GetNewInstancesByType<IFrameworkRunner>();

            var result = new List<string> { typeof(GilesAppDomainRunner).Assembly.Location };

            result.AddRange(runners.SelectMany(x => x.RequiredAssemblies()).Where(x => !string.IsNullOrWhiteSpace(x)));

            return result;
        }
    }
}