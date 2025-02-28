using ManagedCommon;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class ConfigFile
    {
        private string? _iconPath;
        private JsonSerializerOptions _jsonSerializerOptions;
        public const string ConfigFilePathSettingKey = "config-file-path";
        public string ConfigFilePath { get; set; }

        public ConfigFile()
        {
            ConfigFilePath = "";
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public Result BuildOpenConfigFileResult()
        {
            ArgumentNullException.ThrowIfNull(_iconPath);

            string subTitle;
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                subTitle = "Please specify a config json in the plugin options";
            }
            else if (!File.Exists(ConfigFilePath))
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
            if (string.IsNullOrEmpty(ConfigFilePath) || !File.Exists(ConfigFilePath))
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

        public void UpdateIconPath(Theme newTheme)
        {
            _iconPath = newTheme switch
            {
                Theme.Dark or Theme.HighContrastBlack or Theme.HighContrastWhite => "Images/ScriptRunner.dark.png",
                _ => "Images/ScriptRunner.light.png",
            };
        }

        public IEnumerable<ScriptDto> LoadScriptDtos()
        {
            ArgumentNullException.ThrowIfNull(ConfigFilePath);

            if (!File.Exists(ConfigFilePath))
            {
                return [];
            }
            var json = File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<IEnumerable<ScriptDto>>(json, _jsonSerializerOptions) ?? [];
        }
    }
}
