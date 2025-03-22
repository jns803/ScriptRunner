using ManagedCommon;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class Scripts
    {
        private readonly IFileSystem _fileSystem;
        private readonly List<ScriptDto> _scripts = [];
        private string _iconPath;

        public Scripts(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            UpdateIconPath(Theme.Light);
        }

        [MemberNotNull(nameof(_iconPath))]
        public void UpdateIconPath(Theme newTheme)
        {
            _iconPath = newTheme switch
            {
                Theme.Light or Theme.HighContrastWhite => "Images/Script.light.png",
                _ => "Images/Script.dark.png"
            };

            foreach (var script in _scripts)
            {
                script.IconPath = _iconPath;
            }   
        }

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
            return new ScriptDto
            {
                Name = config.Name,
                ScriptPath = config.ScriptPath,
                WorkingDirectory = GetWorkingDirectory(config.WorkingDirectory, config.ScriptPath),
                Arguments = config.Arguments ?? "",
                Type = type,
                CustomInterpreter = config.Interpreter,
                IconPath = _iconPath,
                Score = 0
            };
        }

        private string GetWorkingDirectory(string? workingDirectory, string scriptPath)
        {
            return workingDirectory ?? _fileSystem.Path.GetDirectoryName(scriptPath) ?? "";
        }
    }
}
