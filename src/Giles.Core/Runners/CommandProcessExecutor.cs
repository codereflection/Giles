using System;
using System.Diagnostics;
using System.IO;

namespace Giles.Core.Runners
{
    public static class CommandProcessExecutor
    {
        static CommandProcessExecutor()
        {
            Execute = (fileName, arguements) => RunExecutable(fileName, arguements);
        }

        /// <summary>
        /// Starts the executable passed with the command line arguments
        /// </summary>
        /// <param name="fileName">Executable to run</param>
        /// <param name="arguments">Arguements to pass to the executable</param>
        /// <returns>ExecutionResult</returns>
        public static Func<string, string, ExecutionResult> Execute;


        public static Process SetupProcess(string fileName, string arguments)
        {
            return new Process
            {
                StartInfo =
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
        }

        private static ExecutionResult RunExecutable(string fileName, string arguments)
        {
            var output = string.Empty;
            var process = SetupProcess(fileName, arguments);
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Close();
            process.Dispose();
            return new ExecutionResult
                       {
                           ExitCode = exitCode, 
                           Output = output, 
                           Runner = new FileInfo(fileName).Name.ToUpper().Replace(".EXE", string.Empty)
                       };
        }
    }

    public class ExecutionResult
    {
        public string Runner { get; set; }
        public string Output { get; set; }
        public int ExitCode { get; set; }
    }
}