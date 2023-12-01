using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.RM.Framework.Resolvers
{
    [LocDisplayName("RM Customs Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class RMCustomsPlugin : Singleton<RMCustomsPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<RMCustomsConfig> config;
        public IPluginConfig PluginConfig => config;
        public static RMCustomsConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded RM Customs - Overrides Used: {RMCustomsResolver.Obj.LoadedCustomsOverrides?.Count: 0}";

        static RMCustomsPlugin()
        {
            config = new PluginConfig<RMCustomsConfig>("RMCustoms");
        }

        public static void PostInitialize()
        {
            RMCustomsResolver.Obj.Initialize();
            config.SaveAsync();
        }

        public string GetCategory() => "Raynbo Mods";
    }
}