using System;
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
        private static readonly IFileSystem FileSystem = new FileSystem();
        public Func<IFileSystem> GetFileSystem = () => FileSystem;


        public IEnumerable<SessionResults> Run(string testAssemblyPath)
        {
            IEnumerable<SessionResults> results = new List<SessionResults>();
            GilesAppDomainRunner runner;

            try
            {
                runner = SetupRunner(testAssemblyPath);
                results = runner.Run(testAssemblyPath);
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Contains("homogenous AppDomain"))
                    Console.WriteLine(
                        GilesResource.homogenousAppDomainErrorMessage);
                else
                    Console.WriteLine(GilesResource.GilesTestRunnerExceptionMessage, e);
            }
            catch (Exception e)
            {
                Console.WriteLine(GilesResource.GilesTestRunnerExceptionMessage, e);
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

            SetupAppDomain(testAssemblyPath);

            return GetRunner();
        }

        private GilesAppDomainRunner GetRunner()
        {
            var runType = typeof (GilesAppDomainRunner);

            return appDomain.CreateInstanceAndUnwrap(runType.Assembly.FullName, runType.FullName) as GilesAppDomainRunner;
        }

        private void SetupAppDomain(string testAssemblyPath)
        {
            Console.WriteLine("Setting up App Domain for test assembly: {0}", testAssemblyPath);
            var domainInfo = new AppDomainSetup
                                 {
                                     ApplicationBase = testAssemblyFolder,
                                     PrivateBinPath = "Giles",
                                     ConfigurationFile = GetConfigFile(testAssemblyPath)
                                 };

            Console.WriteLine("Using configuration file: {0}", domainInfo.ConfigurationFile);

            appDomain = AppDomain.CreateDomain("GilesAppDomainRunner", AppDomain.CurrentDomain.Evidence, domainInfo);
        }

        string GetConfigFile(string testAssemblyPath)
        {
            var configFile = testAssemblyPath + ".config";
            
            return GetFileSystem().FileExists(configFile) ? configFile : string.Empty;
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
            var runners = TypeLoader.GetNewInstancesByType<IFrameworkRunner>();

            var result = new List<string> { typeof(GilesAppDomainRunner).Assembly.Location };

            result.AddRange(runners.SelectMany(x => x.RequiredAssemblies()).Where(x => !string.IsNullOrWhiteSpace(x)));

            return result;
        }
    }
}