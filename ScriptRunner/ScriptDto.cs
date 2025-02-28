namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public record ScriptDto(
            string Name,
            string Path,
            string WorkingDirectory,
            string Arguments
        );
}
