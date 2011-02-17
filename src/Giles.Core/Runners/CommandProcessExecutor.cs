using System;
using System.Diagnostics;

namespace Giles.Core.Runners
{
    public class CommandProcessExecutor
    {
        public Process SetupProcess(string fileName, string arguments)
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

        public ExecutionResult Execute(string executable, string arguments)
        {
            var output = string.Empty;
            var process = SetupProcess(executable, arguments);
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            process.Dispose();
            return new ExecutionResult {ExitCode = process.ExitCode, Output = output};
        }
    }

    public class ExecutionResult
    {
        public string Output { get; set; }
        public int ExitCode { get; set; }
    }
}