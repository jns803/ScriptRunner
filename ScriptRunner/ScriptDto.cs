namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    internal enum ScriptType
    {
        Unknown,
        Batch,
        Powershell,
        Shell,
        CustomInterpreter,
    }

    internal class ScriptDto
    {
        internal required string Name {get; set;}
        internal required string ScriptPath { get; set; }
        internal required string WorkingDirectory { get; set; }
        internal required string Arguments { get; set; }
        internal required ScriptType Type { get; set; }

        internal required string? CustomInterpreter { get; set; }
        internal required string IconPath {get; set;}

        internal required int Score {get; set;}

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
