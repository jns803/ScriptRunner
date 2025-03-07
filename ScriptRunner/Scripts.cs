using System.IO;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class Scripts
    {
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

        private ScriptDto MapToScriptDto(ScriptConfigDto config)
        {
            var workingDirectory = config.WorkingDirectory ?? Path.GetDirectoryName(config.ScriptPath);
            var scriptDto = new ScriptDto
            {
                Title = config.Name,
                SubTitle = config.ScriptPath,
                IconPath = "Images/ScriptRunner.light.png",
                Score = 0,
                Action = action => { return false; }
            };
            scriptDto.Action = action =>
            {
                if (!VerifyScript(config.ScriptPath))
                {
                    return false;
                }

                if (!VerifyWorkingDirectory(workingDirectory))
                {
                    return false;
                }

                // TODO: check if file exists
                // TODO: how to run different script types?
                ScriptWasSelected(scriptDto);
                RunBatchScript(scriptDto.SubTitle ?? "");
                return true;
            };
            return scriptDto;
        }

        private bool VerifyScript(string script)
        {
            if (string.IsNullOrWhiteSpace(script) ||
                !File.Exists(script))
            {
                PublicApi?.ShowMsg("Script not found", $"Configured script '{script}' does not exist.");
                return false;
            }
            return true;
        }

        private bool VerifyWorkingDirectory(string? workingDirectory)
        {
            if (string.IsNullOrWhiteSpace(workingDirectory) ||
                !Directory.Exists(workingDirectory))
            {
                PublicApi?.ShowMsg("Working directory not found", $"Configured working directory '{workingDirectory}' does not exist.");
                return false;
            }
            return true;
        }

        private static void RunBatchScript(string scriptPath)
        {
            // TODO: set working directory
            // TODO: pass arguments
            var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + scriptPath)
            {
                CreateNoWindow = false,
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(processInfo);
        }

        public IReadOnlyCollection<ScriptDto> FindScripts(string querySearch)
        {
            if (string.IsNullOrWhiteSpace(querySearch))
            {
                return [.. _scripts.Select(MapToScore)];
            }

            return [.. _scripts
                .Where(x => x.Title.Contains(querySearch, StringComparison.InvariantCultureIgnoreCase))
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
