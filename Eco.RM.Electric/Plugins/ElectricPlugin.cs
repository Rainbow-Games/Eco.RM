using Eco.Core.Plugins.Interfaces;
using Eco.Core.Plugins;
using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Eco.RM.Configs;
using Eco.RM.Electric.Utils;

namespace Eco.RM.Plugins
{
    [LocDisplayName("RM Electric Plugin")]
    public class ElectricPlugin : Singleton<ElectricPlugin>, IInitializablePlugin, IModKitPlugin, IConfigurablePlugin, IShutdownablePlugin
    {
        private readonly PluginConfig<ElectricConfig> config;

        // Variables
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();
        public ElectricFormater Formater { get; }

        // Setter Variables
        public IPluginConfig PluginConfig => config;
        public ElectricConfig Config => config.Config;

        // Methods
        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Enabled";
        public string GetCategory() => "Raynbo Mods";
        public Task ShutdownAsync() => Task.Factory.StartNew(() => { });

        public void Initialize(TimedTask timer)
        {
            Log.WriteWarningLine(Localizer.DoStr("Eco.RM.Electric: Initializing ElectricPlugin"));
            this.SaveConfig();
        }

        public override string ToString()
        {
            return Localizer.DoStr("RM Electric Plugin");
        }

        public ElectricPlugin()
        {
            config = new PluginConfig<ElectricConfig>("RM Electric Plugin");
            Formater = new ElectricFormater(config.Config);
        }
    }
}
