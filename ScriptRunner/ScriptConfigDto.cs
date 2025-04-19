namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public record ConfigDto(
        ScriptConfigDto[] Scripts
    );

    public record ScriptConfigDto(
        string Name,
        string ScriptPath,

        /// If WorkingDirectory is null or white space, the directory where the script is located is taken.
        string? WorkingDirectory,
        string Arguments,

        /// If Interpreter is null, the plugin tries to guess it by the file type.
        string? Interpreter
    );
}
