using Community.PowerToys.Run.Plugin.ScriptRunner;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using ManagedCommon;
using Wox.Plugin;

namespace ScriptRunner.UnitTests
{
    public class ConfigFileTest
    {
        [Fact]
        public void GetScriptConfigs_ShouldReturnEmpty_WhenConfigFilePathIsNull()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var uut = new ConfigFile(mockFileSystem);

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
            var uut = new ConfigFile(mockFileSystem)
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
                { configFilePath, new MockFileData(@"[{
                ""ScriptPath"":""path1"",
                ""WorkingDirectory"":""dir1"",
                ""Arguments"":""args1"",
                ""Interpreter"":""interpreter1""
                }]") }
            });
            var uut = new ConfigFile(mockFileSystem)
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
            var uut = new ConfigFile(mockFileSystem);

            // act
            var result = uut.BuildOpenConfigFileResult();

            // assert
            result.SubTitle.ShouldBe("Please specify a config json in the plugin options");
        }

        [Fact]
        public void BuildOpenConfigFileResult_ShouldReturnResult_WithCorrectSubTitle_WhenConfigFileDoesNotExist()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var uut = new ConfigFile(mockFileSystem)
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

            var uut = new ConfigFile(mockFileSystem)
            {
                ConfigFilePath = configFilePath
            };

            // act
            var result = uut.BuildOpenConfigFileResult();

            // assert
            result.SubTitle.ShouldBe(configFilePath);
        }
    }
}
