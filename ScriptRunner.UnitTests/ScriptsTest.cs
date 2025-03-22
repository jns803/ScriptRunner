using Community.PowerToys.Run.Plugin.ScriptRunner;
using System.IO.Abstractions.TestingHelpers;

namespace ScriptRunner.UnitTests
{
    public class ScriptsTest
    {
        [Fact]
        public void Reload_InitialReload()
        {
            // Arrange
            var scriptConfigs = new List<ScriptConfigDto>
            {
                new("name1", "path1", "dir1", "args1", "interpreter1"),
            };

            var mockFileSystem = new MockFileSystem();
            var uut = new Scripts(mockFileSystem);

            // Act
            uut.Reload(scriptConfigs);
            var scripts = uut.FindScripts("");

            // Assert
            scripts.Count.ShouldBe(1);
        }
    }
}
