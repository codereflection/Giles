using System;
using System.Collections.Generic;
using System.IO;
using Giles.Core.Encryption;

namespace Giles.Core.IO
{
    /// <summary>
    /// Stolen from http://www.codeplex.com/CodePlexClient/SourceControl/changeset/view/18337#59622
    /// </summary>
    public interface IFileSystem
    {
        string UserDataPath { get; }

        void AppendAllText(string path,
                           string contents);

        void CombineAttributes(string path,
                               FileAttributes attributes);

        string CombinePath(params string[] pathElements);

        void CopyFile(string sourcePath,
                      string targetPath);

        void CopyFile(string sourcePath,
                      string targetPath,
                      bool overwrite);

        void MoveFile(string sourcePath,
                      string targetPath);

        void MoveFile(string sourcePath,
                      string targetPath,
                      bool overwrite);

        void CreateDirectory(string path);

        void DeleteDirectory(string path,
                             bool force);

        void DeleteFile(string path);

        bool DirectoryExists(string path);

        void EnsurePath(string path);

        bool FileExists(string path);

        FileAttributes GetAttributes(string path);

        string[] GetDirectories(string path);

        string GetDirectoryName(string path);

        byte[] GetFileHash(string path);

        byte[] GetFileHash(byte[] contents);

        string GetFileHashAsString(string path);

        string GetFileName(string path);

        string[] GetFiles(string path);

        string[] GetFiles(string path,
                          string searchPattern);

        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

        long GetFileSize(string path);

        string GetFullPath(string path);

        DateTime GetLastWriteTime(string path);

        string GetName(string path);

        Stream OpenFile(string path,
                        FileMode mode,
                        FileAccess access,
                        FileShare share);

        TextReader Read(string path);

        byte[] ReadAllBytes(string path);

        string[] ReadAllLines(string path);

        string ReadAllText(string path);

        void RemoveAttributes(string path,
                              FileAttributes attributes);

        void SetAttributes(string path,
                           FileAttributes attributes);

        void WriteAllBytes(string path,
                           byte[] contents);

        void WriteAllLines(string path,
                           string[] contents);

        void WriteAllText(string path,
                          string contents);

        FileSystemWatcher SetupFileWatcher(string filePath, string filter, FileSystemEventHandler changeAction, FileSystemEventHandler createdAction, FileSystemEventHandler deletedAction, RenamedEventHandler renamedAction, ErrorEventHandler errorAction);

        bool BlockUntilFileAvailable(string path);

        bool BlockUntilFileAvailable(string path, TimeSpan timeout);
    }

    public class FileSystem : IFileSystem
    {
        string userDataPath = null;

        public string UserDataPath
        {
            get
            {
                if (userDataPath == null)
                {
                    userDataPath = CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "CodePlex Client");
                    EnsurePath(userDataPath);
                }
                return userDataPath;
            }
            set { userDataPath = value; }
        }

        public void AppendAllText(string path,
                                  string contents)
        {
            File.AppendAllText(path, contents);
        }

        public void CombineAttributes(string path,
                                      FileAttributes attributes)
        {
            FileAttributes existing = GetAttributes(path);

            existing |= attributes;

            SetAttributes(path, existing);
        }

        public string CombinePath(params string[] pathElements)
        {
            string result = "";

            foreach (string pathElement in pathElements)
                result = Path.Combine(result, pathElement);

            return result;
        }

        public void CopyFile(string sourcePath,
                             string targetPath)
        {
            CopyFile(sourcePath, targetPath, false);
        }

        public void CopyFile(string sourcePath,
                             string targetPath,
                             bool overwrite)
        {
            try
            {
                File.Copy(sourcePath, targetPath, overwrite);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to copy {0}\n\tto\n\t\t{1} file: {0}\n\tError: {2}", sourcePath, targetPath, e.Message);
            }
        }

        public void MoveFile(string sourcePath,
                             string targetPath)
        {
            MoveFile(sourcePath, targetPath, false);
        }

        public void MoveFile(string sourcePath,
                             string targetPath,
                             bool overwrite)
        {
            if (File.Exists(targetPath) &&
                overwrite)
                File.Delete(targetPath);
            File.Move(sourcePath, targetPath);
        }


        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path,
                                    bool force)
        {
            if (!Directory.Exists(path))
                return;

            if (force)
                RecursiveMakeNormal(path);

            Directory.Delete(path, force);
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                SetAttributes(path, FileAttributes.Normal);
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to delete file: {0}\n\tError: {1}", path, e.Message);
                }
            }
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            char ending = path[path.Length - 1];

            if (ending == Path.DirectorySeparatorChar || ending == Path.AltDirectorySeparatorChar)
                path = path.Substring(0, path.Length - 1);

            return Path.GetDirectoryName(path);
        }

        public byte[] GetFileHash(string path)
        {
            using (var stream = OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                return EncryptionHelper.GetHash(stream);
        }

        public byte[] GetFileHash(byte[] contents)
        {
            using (var stream = new MemoryStream(contents, false))
                return EncryptionHelper.GetHash(stream);
        }

        public string GetFileHashAsString(string path)
        {
            return Convert.ToBase64String(GetFileHash(path));
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] GetFiles(string path,
                                 string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        public long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public string GetName(string path)
        {
            return Path.GetFileName(path);
        }

        public Stream OpenFile(string path,
                               FileMode mode,
                               FileAccess access,
                               FileShare share)
        {
            return new FileStream(path, mode, access, share);
        }

        public TextReader Read(string path)
        {
            return File.OpenText(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public string[] ReadAllLines(string path)
        {
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var reader = new StreamReader(file);
                var result = new List<string>();

                string line;
                while ((line = reader.ReadLine()) != null)
                    result.Add(line);

                return result.ToArray();
            }
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        void RecursiveMakeNormal(string path)
        {
            foreach (string dir in GetDirectories(path))
            {
                RecursiveMakeNormal(dir);
                SetAttributes(dir, FileAttributes.Normal | FileAttributes.Directory);
            }

            foreach (string file in GetFiles(path))
                SetAttributes(file, FileAttributes.Normal);
        }

        public void RemoveAttributes(string path,
                                     FileAttributes attributes)
        {
            FileAttributes existing = GetAttributes(path);

            existing &= ~attributes;

            SetAttributes(path, existing);
        }

        public void SetAttributes(string path,
                                  FileAttributes attributes)
        {
            File.SetAttributes(path, attributes);
        }

        public void WriteAllBytes(string path,
                                  byte[] contents)
        {
            File.WriteAllBytes(path, contents);
        }

        public void WriteAllLines(string path,
                                  string[] contents)
        {
            File.WriteAllLines(path, contents);
        }

        public void WriteAllText(string path,
                                 string contents)
        {
            File.WriteAllText(path, contents);
        }

        public FileSystemWatcher SetupFileWatcher(string filePath, string filter, FileSystemEventHandler changeAction, FileSystemEventHandler createdAction, FileSystemEventHandler deletedAction, RenamedEventHandler renamedAction, ErrorEventHandler errorAction)
        {
            var fileWatcher = new FileSystemWatcher(filePath, filter);
            fileWatcher.Changed += changeAction;
            fileWatcher.Created += createdAction;
            fileWatcher.Deleted += deletedAction;
            fileWatcher.Renamed += renamedAction;
            fileWatcher.Error += errorAction;

            fileWatcher.Error += delegate(object sender, ErrorEventArgs e)
            {
                fileWatcher.EnableRaisingEvents = false;

                while (!fileWatcher.EnableRaisingEvents)
                {
                    try
                    {
                        fileWatcher.Changed -= changeAction;
                        fileWatcher.Created -= createdAction;
                        fileWatcher.Deleted -= deletedAction;
                        fileWatcher.Renamed -= renamedAction;
                        fileWatcher.Error -= errorAction;

                        fileWatcher = new FileSystemWatcher(filePath, filter);

                        fileWatcher.Changed += changeAction;
                        fileWatcher.Created += createdAction;
                        fileWatcher.Deleted += deletedAction;
                        fileWatcher.Renamed += renamedAction;
                        fileWatcher.Error += errorAction;

                        fileWatcher.EnableRaisingEvents = true;
                    }
                    catch
                    {
                        fileWatcher.EnableRaisingEvents = false;
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            };
            return fileWatcher;
        }

        public bool BlockUntilFileAvailable(string path)
        {
            return BlockUntilFileAvailable(path, TimeSpan.FromMinutes(5));
        }

        public bool BlockUntilFileAvailable(string path, TimeSpan timeout)
        {
            bool FileAvailable = false;
            DateTime StartDateTime = DateTime.Now;
            do
            {
                FileStream fs = null;
                try
                {
                    fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    FileAvailable = true;
                }
                catch
                {
                    System.Threading.Thread.Sleep(1000);
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            } while (!FileAvailable && DateTime.Now.Subtract(StartDateTime).CompareTo(timeout) < 0);
            return FileAvailable;
        }

    }
}