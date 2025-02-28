using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class ResultBuilder
    {
        public IEnumerable<Result> BuildResults(IEnumerable<ScriptDto> scriptDtos)
        {
            foreach (var scriptDto in scriptDtos)
            {
                yield return new Result
                {
                    Title = scriptDto.Name ?? "",
                    SubTitle = scriptDto.Path ?? "",
                    IcoPath = "Images/ScriptRunner.light.png",
                    Action = action =>
                    {
                        RunBatchScript(scriptDto.Path ?? "");
                        return true;
                    },
                };
            }
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
    }
}
