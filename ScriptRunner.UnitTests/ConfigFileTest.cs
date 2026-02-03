using Community.PowerToys.Run.Plugin.ScriptRunner;
using System.IO.Abstractions.TestingHelpers;
using ManagedCommon;

namespace ScriptRunner.UnitTests
{
    public class ConfigFileTest
    {
        [Fact]
        public void GetScriptConfigs_ShouldReturnEmpty_WhenConfigFilePathIsNull()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var tempDir = mockFileSystem.Path.GetTempPath();
            var uut = new ConfigFile(mockFileSystem, tempDir);

            // act
            var configs = uut.GetScriptConfigs();

            // assert
            configs.ShouldBeEmpty();
        }

        [Fact]
        public void GetScriptConfigs_ShouldReturnEmpty_WhenConfigFileDoesNotExist()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var tempDir = mockFileSystem.Path.GetTempPath();
            var uut = new ConfigFile(mockFileSystem, tempDir)
            {
                ConfigFilePath = "nonexistent.json"
            };

            // act
            var configs = uut.GetScriptConfigs();

            // assert
            configs.ShouldBeEmpty();
        }

        [Fact]
        public void GetScriptConfigs_ShouldReturnConfigs_WhenConfigFileExists()
        {
            // arrange
            var configFilePath = "testConfig.json";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { configFilePath, new MockFileData(@"{
                ""scripts"": [{
                    ""ScriptPath"":""path1"",
                    ""WorkingDirectory"":""dir1"",
                    ""Arguments"":""args1"",
                    ""Interpreter"":""interpreter1""
                }]}") }
            });
            var tempDir = mockFileSystem.Path.GetTempPath();
            var uut = new ConfigFile(mockFileSystem, tempDir)
            {
                ConfigFilePath = configFilePath
            };

            // act
            var configs = uut.GetScriptConfigs();

            // assert
            configs.ShouldHaveSingleItem();
            var config = configs.First();
            config.ScriptPath.ShouldBe("path1");
            config.WorkingDirectory.ShouldBe("dir1");
            config.Arguments.ShouldBe("args1");
            config.Interpreter.ShouldBe("interpreter1");
        }

        [Fact]
        public void BuildOpenConfigFileResult_ShouldReturnResult_WithCorrectSubTitle_WhenConfigFilePathIsNull()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var tempDir = mockFileSystem.Path.GetTempPath();
            var uut = new ConfigFile(mockFileSystem, tempDir);

            // act
            var result = uut.BuildOpenConfigFileResult();

            // assert
            result.SubTitle.ShouldBe("Please specify a config json in the plugin options.");
        }

        [Fact]
        public void BuildOpenConfigFileResult_ShouldReturnResult_WithCorrectSubTitle_WhenConfigFileDoesNotExist()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var tempDir = mockFileSystem.Path.GetTempPath();
            var uut = new ConfigFile(mockFileSystem, tempDir)
            {
                ConfigFilePath = "nonexistent.json"
            };

            // act
            var result = uut.BuildOpenConfigFileResult();

            // assert
            result.SubTitle.ShouldBe("nonexistent.json does not exist");
        }

        [Fact]
        public void BuildOpenConfigFileResult_ShouldReturnResult_WithCorrectSubTitle_WhenConfigFileExists()
        {
            // arrange
            var configFilePath = "testConfig.json";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { configFilePath, new MockFileData("{}") }
            });
            var tempDir = mockFileSystem.Path.GetTempPath();

            var uut = new ConfigFile(mockFileSystem, tempDir)
            {
                ConfigFilePath = configFilePath
            };

            // act
            var result = uut.BuildOpenConfigFileResult();

            // assert
            result.SubTitle.ShouldBe(configFilePath);
        }

        /// <summary>
        /// Test for UpdateIconPath method.
        /// </summary>
        /// <param name="theme">Pass theme as an int, otherwise some strange run time errors happened</param>
        [Theory]
        [InlineData((int)Theme.System, "Images/Config.dark.png")]
        [InlineData((int)Theme.Light, "Images/Config.light.png")]
        [InlineData((int)Theme.HighContrastWhite, "Images/Config.light.png")]
        [InlineData((int)Theme.Dark, "Images/Config.dark.png")]
        [InlineData((int)Theme.HighContrastOne, "Images/Config.dark.png")]
        [InlineData((int)Theme.HighContrastTwo, "Images/Config.dark.png")]
        [InlineData((int)Theme.HighContrastBlack, "Images/Config.dark.png")]
        public void UpdateIconPath_ShouldSetIconPath_WhenCalled(int theme, string iconPath)
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var tempDir = mockFileSystem.Path.GetTempPath();
            var uut = new ConfigFile(mockFileSystem, tempDir);

            // act
            uut.UpdateIconPath((Theme)theme);
            var result = uut.BuildOpenConfigFileResult();

            // assert
            result.IcoPath.ShouldBe(iconPath);
        }
    }
}
