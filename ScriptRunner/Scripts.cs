namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class Scripts
    {
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
            var scriptDto = new ScriptDto
            {
                Title = config.Name,
                SubTitle = config.Path,
                IconPath = "Images/ScriptRunner.light.png",
                Score = 0,
                Action = action => { return false; }
            };
            scriptDto.Action = action =>
            {
                ScriptWasSelected(scriptDto);
                RunBatchScript(scriptDto.SubTitle ?? "");
                return true;
            };
            return scriptDto;
        }

        private static void RunBatchScript(string scriptPath)
        {
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
