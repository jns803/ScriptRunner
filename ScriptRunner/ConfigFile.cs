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
        internal const string ConfigFilePathSettingKey = "config-file-path";

        internal IPublicAPI? PublicApi { get; set; }

        internal string DefaultConfigDir => _fileSystem.Path.Combine(_pluginDirectory, "DefaultConfig");
        internal string DefaultConfigFilePath => _fileSystem.Path.Combine(DefaultConfigDir, "script-runner-config.json");

        internal string ConfigFilePath { get; set; }

        private readonly IFileSystem _fileSystem;
        private readonly string _pluginDirectory;
        private string _iconPath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ConfigFile(IFileSystem fileSystem, string pluginDirectory)
        {
            _fileSystem = fileSystem;
            _pluginDirectory = pluginDirectory;

            ConfigFilePath = "";
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            UpdateIconPath(Theme.Light);
        }

        /// <summary>
        /// Creates a default config json which configures a hello world script.
        /// </summary>
        internal void CreateDefaultConfig()
        {
            var configContent = _fileSystem.File.ReadAllText(_fileSystem.Path.Combine(DefaultConfigDir, "script-runner-config-template.json"));

            var escapedPluginDirectory = DefaultConfigDir.Replace("\\", "\\\\");
            configContent = configContent.Replace("$DEFAULT_CONFIG_DIR$", escapedPluginDirectory);

            _fileSystem.File.WriteAllText(DefaultConfigFilePath, configContent);
        }

        internal bool DefaultConfigExists()
        {
            return _fileSystem.File.Exists(DefaultConfigFilePath);
        }

        internal Result BuildOpenConfigFileResult()
        {
            string subTitle;
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                subTitle = "Please specify a config json in the plugin options.";
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
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                PublicApi?.ShowMsg("Config not found", "Please specify a config json in the plugin options.");
                return false;
            }
            else if (!_fileSystem.File.Exists(ConfigFilePath))
            {
                PublicApi?.ShowMsg("Config not found", $"'{ConfigFilePath} does not exist'");
                return false;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = ConfigFilePath,
                UseShellExecute = true,
            });

            return true;
        }

        internal IEnumerable<ScriptConfigDto> GetScriptConfigs()
        {
            ArgumentNullException.ThrowIfNull(ConfigFilePath);

            if (!_fileSystem.File.Exists(ConfigFilePath))
            {
                return [];
            }
            var json = _fileSystem.File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<ConfigDto>(json, _jsonSerializerOptions)?.Scripts ?? [];
        }

        [MemberNotNull(nameof(_iconPath))]
        internal void UpdateIconPath(Theme newTheme)
        {
            _iconPath = newTheme switch
            {
                Theme.Light or Theme.HighContrastWhite => "Images/Config.light.png",
                _ => "Images/Config.dark.png"
            };
        }
    }
}
