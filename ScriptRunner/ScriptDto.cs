using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public class ScriptDto
    {
        public required string Title {get; set;}
        public required string SubTitle { get; set; }

        public required string IconPath {get; set;}

        public required int Score {get; set;}

        public required Func<ActionContext, bool> Action { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is ScriptDto other)
            {
                return Title == other.Title &&
                    SubTitle == other.SubTitle &&
                    IconPath == other.IconPath;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, SubTitle, IconPath);
        }
    }
}
