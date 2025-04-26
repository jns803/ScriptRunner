using System.Diagnostics;
using System.IO.Abstractions;
using Wox.Plugin;
using System.Diagnostics.CodeAnalysis;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    internal class ResultActionBuilder(IFileSystem fileSystem)
    {
        internal IPublicAPI? PublicApi { get; set; }

        private readonly IFileSystem _fileSystem = fileSystem;

        internal Func<ActionContext, bool> BuildAction(ScriptDto scriptDto)
        {
            return (ActionContext ac) =>
            {
                if (!VerifyScript(scriptDto.ScriptPath))
                {
                    return false;
                }

                if (!VerifyWorkingDirectory(scriptDto.WorkingDirectory))
                {
                    return false;
                }

                switch (scriptDto.Type)
                {
                case ScriptType.Batch:
                    RunBatchScript(scriptDto);
                    break;
                case ScriptType.Powershell:
                    RunPowershellScript(scriptDto);
                    break;
                case ScriptType.Shell:
                    RunShellScript(scriptDto);
                    break;
                case ScriptType.CustomInterpreter:
                    RunCustomInterpreterScript(scriptDto);
                    break;
                default:
                    break;
                }
                return true;
            };
        }

        private bool VerifyScript([NotNullWhen(true)] string script)
        {
            if (string.IsNullOrWhiteSpace(script) ||
                !_fileSystem.File.Exists(script))
            {
                PublicApi?.ShowMsg("Script not found", $"Configured script '{script}' does not exist.");
                return false;
            }
            return true;
        }

        private bool VerifyWorkingDirectory([NotNullWhen(true)] string? workingDirectory)
        {
            if (string.IsNullOrWhiteSpace(workingDirectory) ||
                !_fileSystem.Directory.Exists(workingDirectory))
            {
                PublicApi?.ShowMsg("Working directory not found", $"Configured working directory '{workingDirectory}' does not exist.");
                return false;
            }
            return true;
        }

        private static bool RunBatchScript(ScriptDto script)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $@"/c ""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            Process.Start(processInfo);
            return true;
        }

        private static bool RunPowershellScript(ScriptDto script)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $@"-ExecutionPolicy Bypass -File ""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            Process.Start(processInfo);
            return true;
        }

        private static bool RunShellScript(ScriptDto script)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Git\git-bash.exe",
                Arguments = $@"--no-cd ""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            Process.Start(processInfo);
            return true;
        }



        private bool RunCustomInterpreterScript(ScriptDto script)
        {
            if (!VerifyCustomInterpreter(script.CustomInterpreter))
            {
                return false;
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = script.CustomInterpreter,
                Arguments = $@"""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            Process.Start(processInfo);
            return true;
        }
        private bool VerifyCustomInterpreter([NotNullWhen(true)] string? customInterpreter)
        {
            if (string.IsNullOrWhiteSpace(customInterpreter))
            {
                PublicApi?.ShowMsg("Interpreter not found", $"Configured interpreter is invalid.");
                return false;
            }

            // Handle absolute paths
            if (_fileSystem.Path.IsPathFullyQualified(customInterpreter))
            {
                if (_fileSystem.File.Exists(customInterpreter))
                {
                    return true;
                }
                else
                {
                    PublicApi?.ShowMsg("Interpreter not found", $"Configured interpreter '{customInterpreter}' does not exist.");
                    return false;
                }
            }

            // Maybe the interpreter can be found in PATH
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $@"/c where {customInterpreter}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrWhiteSpace(output))
            {
                PublicApi?.ShowMsg("Interpreter not found", $"Configured interpreter '{customInterpreter}' does not exist.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
