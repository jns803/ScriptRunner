using ManagedCommon;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text.Json;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class ConfigFile
    {
        public IPublicAPI? PublicApi { get; set; }
        public string? PluginDirectory { get; set; }

        private readonly IFileSystem _fileSystem;
        private string _iconPath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public const string ConfigFilePathSettingKey = "config-file-path";
        public string ConfigFilePath { get; set; }

        public ConfigFile(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            ConfigFilePath = "";
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            UpdateIconPath(Theme.Light);
        }

        public Result BuildOpenConfigFileResult()
        {
            string subTitle;
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                subTitle = "Please specify a config json in the plugin options";
            }
            else if (!_fileSystem.File.Exists(ConfigFilePath))
            {
                subTitle = $"{ConfigFilePath} does not exist";
            }
            else
            {
                subTitle = ConfigFilePath;
            }

            return new Result
            {
                Title = "Open config file",
                SubTitle = subTitle,
                QueryTextDisplay = string.Empty,
                IcoPath = _iconPath,
                Action = action => OpenConfigFile(),
            };
        }

        private bool OpenConfigFile()
        {
            if (string.IsNullOrEmpty(ConfigFilePath) || !_fileSystem.File.Exists(ConfigFilePath))
            {
                return false;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = ConfigFilePath,
                UseShellExecute = true,
            });

            return true;
        }

        [MemberNotNull(nameof(_iconPath))]
        public void UpdateIconPath(Theme newTheme)
        {
            _iconPath = newTheme switch
            {
                Theme.Light or Theme.HighContrastWhite => "Images/Config.light.png",
                _ => "Images/Config.dark.png"
            };
        }

        public IEnumerable<ScriptConfigDto> GetScriptConfigs()
        {
            ArgumentNullException.ThrowIfNull(ConfigFilePath);

            if (!_fileSystem.File.Exists(ConfigFilePath))
            {
                return [];
            }
            var json = _fileSystem.File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<ConfigDto>(json, _jsonSerializerOptions)?.Scripts ?? [];
        }

        /// <summary>
        /// Creates a default config json which configures a hello world script.
        /// </summary>
        internal string CreateDefaultConfig()
        {
            if (string.IsNullOrEmpty(PluginDirectory))
            {
                return string.Empty;
            }

            try
            {
                var defaultConfigPath = _fileSystem.Path.Combine(PluginDirectory, "DefaultConfig");

                var configContent = _fileSystem.File.ReadAllText(_fileSystem.Path.Combine(defaultConfigPath, "script-runner-config.json"));

                var escapedPluginDirectory = PluginDirectory.Replace("\\", "\\\\");
                configContent = configContent.Replace("$BASE_DIR$", escapedPluginDirectory);

                var configFilePath = _fileSystem.Path.Combine(PluginDirectory, $"script-runner-config-{Guid.NewGuid()}.json");

                _fileSystem.File.WriteAllText(configFilePath, configContent);

                return configFilePath;
            }
            catch (Exception ex)
            {
                PublicApi?.ShowMsg("ScriptRunner", $"Could not setup default config (error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
