namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public record ConfigDto(
        ScriptConfigDto[] Scripts
    );

    /// <summary>
    /// Represents the configuration for a script that can be executed by the plugin.
    /// </summary>
    /// <param name="Name">
    /// The name of the script. This is used to identify the script in the configuration.
    /// </param>
    /// <param name="ScriptPath">
    /// The full path to the script file.
    /// This is required and must point to a valid script file.
    /// </param>
    /// <param name="WorkingDirectory">
    /// The working directory for the script execution.
    /// If null or whitespace, the directory where the script is located will be used.
    /// </param>
    /// <param name="Arguments">
    /// The arguments to pass to the script during execution.
    /// This can be null or empty string if no arguments are required.
    /// </param>
    /// <param name="Interpreter">
    /// The interpreter to use for executing the script (e.g., Python, Bash).
    /// If null, the plugin will attempt to infer the interpreter based on the script file type.
    /// </param>
    public record ScriptConfigDto(
        string Name,
        string ScriptPath,
        string? WorkingDirectory,
        string? Arguments,
        string? Interpreter
    );
}
