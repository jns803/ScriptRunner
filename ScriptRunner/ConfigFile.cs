using System;
using System.Diagnostics;
using System.IO;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    class ConfigFile
    {
        private readonly Func<string> _getIconPath;
        public const string ConfigFilePathSettingKey = "config-file-path";
        public string ConfigFilePath { get; set; }

        public ConfigFile(Func<string> getIconPath)
        {
            ConfigFilePath = "";
            _getIconPath = getIconPath ?? throw new ArgumentNullException(nameof(getIconPath));
        }

        public Result BuildOpenConfigFileResult()
        {
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
                IcoPath = _getIconPath(),
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
    }
}
