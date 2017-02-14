using System.Diagnostics;

namespace NugetPackagerAssistant.Common
{
    public static class ProcessHelper
    {
        public static Process StartNewProcess(string processFullPath, string workingDirectory, string arguments = null)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = processFullPath,
                    WorkingDirectory = workingDirectory,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            if (!string.IsNullOrEmpty(arguments))
                process.StartInfo.Arguments = arguments;

            return process;
        }
    }
}
