using Community.PowerToys.Run.Plugin.ScriptRunner;

namespace ScriptRunner.UnitTests
{
    public class ConfigFileTest
    {
        [Fact]
        public void GetScriptConfigs()
        {
            // arrange
            var uut = new ConfigFile();

            // act
            // assert
            uut.ShouldNotBeNull();

        }
    }
}