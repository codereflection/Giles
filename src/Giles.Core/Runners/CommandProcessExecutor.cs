using System;
using System.Diagnostics;

namespace Giles.Core.Runners
{
    public interface ICommandProcessExecutor
    {
        ExecutionResult Execute(string executable, string arguments);
    }

    public class CommandProcessExecutor : ICommandProcessExecutor
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
            var exitCode = process.ExitCode;
            process.Close();
            process.Dispose();
            return new ExecutionResult {ExitCode = exitCode, Output = output};
        }
    }

    public class ExecutionResult
    {
        public string Output { get; set; }
        public int ExitCode { get; set; }
    }
}