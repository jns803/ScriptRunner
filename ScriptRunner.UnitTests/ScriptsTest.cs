using Community.PowerToys.Run.Plugin.ScriptRunner;
using ManagedCommon;
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

        /// <summary>
        /// Test for UpdateIconPath method.
        /// </summary>
        /// <param name="theme">Pass theme as an int, otherwise some strange run time errors happened</param>
        [Theory]
        [InlineData((int)Theme.System, "Images/Script.dark.png")]
        [InlineData((int)Theme.Light, "Images/Script.light.png")]
        [InlineData((int)Theme.HighContrastWhite, "Images/Script.light.png")]
        [InlineData((int)Theme.Dark, "Images/Script.dark.png")]
        [InlineData((int)Theme.HighContrastOne, "Images/Script.dark.png")]
        [InlineData((int)Theme.HighContrastTwo, "Images/Script.dark.png")]
        [InlineData((int)Theme.HighContrastBlack, "Images/Script.dark.png")]
        public void UpdateIconPath_ShouldSetIconPath_WhenCalled(int theme, string iconPath)
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var uut = new Scripts(mockFileSystem);
            var scriptConfigs = new List<ScriptConfigDto>
            {
                new("name1", "path1", "dir1", "args1", "interpreter1"),
            };
            uut.Reload(scriptConfigs);

            // act
            uut.UpdateIconPath((Theme)theme);

            // assert
            var scripts = uut.FindScripts("");
            scripts.First().IconPath.ShouldBe(iconPath);
        }
    }
}
