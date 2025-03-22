using Community.PowerToys.Run.Plugin.ScriptRunner.Properties;
using System.Windows.Controls;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin;
using System.IO.Abstractions;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public class Main : IPlugin, IPluginI18n, IContextMenu, ISettingProvider, IReloadable, IDisposable
    {
        // Note:
        // Methods are basically called in the following order:
        // - Constructor --> logical
        // - UpdateSettings()  --> unexpected
        // - Init()
        // - Query()


        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "41b5ed86908f4210a41520596cf4e533";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => Resources.plugin_name;

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => Resources.plugin_description;

        private readonly Scripts _scripts;
        private readonly ConfigFile _configFile;

        private PluginInitContext? _context;
        private bool _disposed;

        private delegate void UdateIconPathDelegate(Theme theme);
        private readonly UdateIconPathDelegate _updateIconPath;

        public Main()
        {
            var fileSystem = new FileSystem();
            _scripts = new Scripts(fileSystem);
            _configFile = new ConfigFile(fileSystem);
            _updateIconPath += _configFile.UpdateIconPath;
            _updateIconPath += _scripts.UpdateIconPath;
        }

        /// <summary>
        /// Additional options for the plugin.
        /// </summary>
        public IEnumerable<PluginAdditionalOption> AdditionalOptions =>
        [
            new PluginAdditionalOption()
            {
                PluginOptionType= PluginAdditionalOption.AdditionalOptionType.Textbox,
                Key = ConfigFile.ConfigFilePathSettingKey,
                DisplayLabel = Resources.config_file_setting_title,
                DisplayDescription = Resources.config_file_setting_description,
            },
        ];

        /// <summary>
        /// Updates settings.
        /// </summary>
        /// <param name="settings">The plugin settings.</param>
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            var configFilePath = settings?
                .AdditionalOptions?
                .FirstOrDefault(x => x.Key == ConfigFile.ConfigFilePathSettingKey)?
                .TextValue;
            _configFile.ConfigFilePath = configFilePath ?? "";
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>
        /// <remarks>probably I dont need this...</remarks>
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            return [];
        }

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);

            _scripts.Reload(_configFile.GetScriptConfigs());

            var scripts = _scripts.FindScripts(query.Search);

            var results = scripts.Select(MapToResult).ToList();

            if (results.Count == 0)
            {
                results.Add(_configFile.BuildOpenConfigFileResult());
            }
            else if (query.Search.Length == 0)
            {
                results.Insert(0, _configFile.BuildOpenConfigFileResult());
            }
            return results;
        }

        private static Result MapToResult(ScriptDto script)
        {
            return new Result
            {
                Title = script.Name,
                SubTitle = script.ScriptPath,
                IcoPath = script.IconPath,
                Score = script.Score,
                Action = script.ExecuteScript,
            };
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;

            _updateIconPath(_context.API.GetCurrentTheme());
            _scripts.PublicApi = _context.API;
        }

        public string GetTranslatedPluginTitle()
        {
            return Resources.plugin_name;
        }

        public string GetTranslatedPluginDescription()
        {
            return Resources.plugin_description;
        }

        private void OnThemeChanged(Theme oldTheme, Theme newTheme)
        {
            _updateIconPath?.Invoke(newTheme);
        }

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

        public void ReloadData()
        {
            if (_context is null)
            {
                return;
            }

            _updateIconPath?.Invoke(_context.API.GetCurrentTheme());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_context != null && _context.API != null)
                {
                    _context.API.ThemeChanged -= OnThemeChanged;
                }

                _disposed = true;
            }
        }
    }
}
