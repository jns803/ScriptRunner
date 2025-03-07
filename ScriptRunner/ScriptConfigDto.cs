namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public record ScriptConfigDto(
        string Name,
        string ScriptPath,

        /// If WorkingDirectory is null or white space, the directory where the script is located is taken.
        string? WorkingDirectory,
        string Arguments,

        /// If Interpreter is null, the used interpreter is guessed by the scritps file type.
        string? Interpreter

    );
}
