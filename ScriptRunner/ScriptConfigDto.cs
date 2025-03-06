namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public record ScriptConfigDto(
        string Name,
        string Path,
        string WorkingDirectory,
        string Arguments
    );
}
