using Community.PowerToys.Run.Plugin.ScriptRunner.Properties;
using System.Windows.Controls;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ScriptRunner
{
    public class Main : IPlugin, IPluginI18n, IContextMenu, ISettingProvider, IReloadable, IDisposable, IDelayedExecutionPlugin
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

        private readonly ConfigFile _configFile;
        private readonly ResultBuilder _resultBuilder;

        private PluginInitContext? _context;
        private bool _disposed;

        private delegate void UdateIconPathDelegate(Theme theme);
        private UdateIconPathDelegate? _updateIconPath;

        public Main()
        {
            _configFile = new ConfigFile();
            _resultBuilder = new ResultBuilder();
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

            // empty query
            if (string.IsNullOrEmpty(query.Search))
            {
                return new List<Result>{
                    _configFile.BuildOpenConfigFileResult()
                };
            }
            else
            {
                var scriptDtos = _configFile.LoadScriptDtos();
                // TODO: filter configs
                return [.. _resultBuilder.BuildResults(scriptDtos)];

            }
        }



        // TODO: return delayed query results (optional)
        public List<Result> Query(Query query, bool delayedExecution)
        {
            ArgumentNullException.ThrowIfNull(query);

            var results = new List<Result>();

            // empty query
            if (string.IsNullOrEmpty(query.Search))
            {
                return results;
            }

            return results;
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;

            _updateIconPath += _configFile.UpdateIconPath;
            _updateIconPath(_context.API.GetCurrentTheme());
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
