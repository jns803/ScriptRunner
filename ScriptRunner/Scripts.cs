using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class Scripts(IFileSystem fileSystem)
    {
        private readonly IFileSystem _fileSystem = fileSystem;
        public IPublicAPI? PublicApi { get; set; }
        private readonly List<ScriptDto> _scripts = [];

        public void Reload(IEnumerable<ScriptConfigDto> scriptConfigs)
        {
            var scripts = scriptConfigs.Select(MapToScriptDto).ToList();

            var newScripts = scripts.Where(x => !_scripts.Contains(x)).ToList();
            var oldScripts = _scripts.Where(x => !scripts.Contains(x)).ToList();

            foreach (var oldScript in oldScripts)
            {
                _scripts.Remove(oldScript);
            }

            _scripts.AddRange(newScripts);
        }

        private ScriptType GuessScriptType(string scriptPath, string? interpreter)
        {
            if (!string.IsNullOrWhiteSpace(interpreter))
            {
                return ScriptType.CustomInterpreter;
            }

            var types = new Dictionary<string, ScriptType>()
            {
                { ".bat", ScriptType.Batch },
                { ".ps1", ScriptType.Powershell },
                { ".sh", ScriptType.Shell }
            };

            if (types.TryGetValue(_fileSystem.Path.GetExtension(scriptPath), out var type))
            {
                return type;
            }
            return ScriptType.Unknown;
        }

        private ScriptDto MapToScriptDto(ScriptConfigDto config)
        {
            var type = GuessScriptType(config.ScriptPath, config.Interpreter);
            var scriptDto = new ScriptDto
            {
                Name = config.Name,
                ScriptPath = config.ScriptPath,
                WorkingDirectory = GetWorkingDirectory(config.WorkingDirectory, config.ScriptPath),
                Arguments = config.Arguments ?? "",
                Type = type,
                CustomInterpreter = config.Interpreter,
                IconPath = GetIconPath(type),
                Score = 0,
                ExecuteScript = action => { return false; }
            };
            scriptDto.ExecuteScript = action =>
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
                ScriptWasSelected(scriptDto);
                return true;
            };
            return scriptDto;
        }

        private static string GetIconPath(ScriptType type)
        {
            return "Images/Script.light.png";
        }

        private string GetWorkingDirectory(string? workingDirectory, string scriptPath)
        {
            return workingDirectory ?? _fileSystem.Path.GetDirectoryName(scriptPath) ?? "";
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
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $@"/c ""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(processInfo);
            return true;
        }

        private bool RunCustomInterpreterScript(ScriptDto script)
        {
            if (!VerifyCustomInterpreter(script.CustomInterpreter))
            {
                return false;
            }

            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = script.CustomInterpreter,
                Arguments = $@"""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(processInfo);
            return true;
        }

        private static bool RunShellScript(ScriptDto script)
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = @"C:\Program Files\Git\git-bash.exe",
                Arguments = $@"--no-cd ""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(processInfo);
            return true;
        }

        private static bool RunPowershellScript(ScriptDto script)
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $@"-ExecutionPolicy Bypass -File ""{script.ScriptPath}"" {script.Arguments}",
                WorkingDirectory = script.WorkingDirectory,
                CreateNoWindow = false,
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(processInfo);
            return true;
        }

        public IReadOnlyCollection<ScriptDto> FindScripts(string querySearch)
        {
            if (string.IsNullOrWhiteSpace(querySearch))
            {
                return [.. _scripts.Select(MapToScore)];
            }

            return [.. _scripts
                .Where(x => x.Name.Contains(querySearch, StringComparison.InvariantCultureIgnoreCase))
                .Select(MapToScore)];
        }

        private ScriptDto MapToScore(ScriptDto script, int i)
        {
            script.Score = _scripts.Count + 1 - i;
            return script;
        }

        public void ScriptWasSelected(ScriptDto script)
        {
            var index = _scripts.IndexOf(script);
            if (index == -1)
            {
                return;
            }

            _scripts.RemoveAt(index);
            _scripts.Insert(0, script);
        }
    }
}
