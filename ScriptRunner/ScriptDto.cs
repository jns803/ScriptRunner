using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public enum ScriptType
    {
        Unknown,
        Batch,
        Powershell,
        Shell,
        CustomInterpreter,
    }

    public class ScriptDto
    {
        public required string Name {get; set;}
        public required string ScriptPath { get; set; }
        public required string WorkingDirectory { get; set; }
        public required string Arguments { get; set; }
        public required ScriptType Type { get; set; }

        public required string? CustomInterpreter { get; set; }
        public required string IconPath {get; set;}

        public required int Score {get; set;}

        public required Func<ActionContext, bool> ExecuteScript { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is ScriptDto other)
            {
                return Name == other.Name &&
                    ScriptPath == other.ScriptPath &&
                    WorkingDirectory == other.WorkingDirectory &&
                    Arguments == other.Arguments &&
                    Type == other.Type &&
                    CustomInterpreter == other.CustomInterpreter &&
                    IconPath == other.IconPath;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ScriptPath, WorkingDirectory, Arguments, Type, CustomInterpreter, IconPath);
        }
    }
}
