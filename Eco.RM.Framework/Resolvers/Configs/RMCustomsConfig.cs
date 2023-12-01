using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.RM.Framework.Resolvers
{
    public class RMCustomsConfig
    {
        [LocDisplayName("Custom Configuration Values")]
        [LocDescription("Custom Configuration Values by modules. ANY change to this list will require a server restart to take effect.")]
        public SerializedSynchronizedCollection<CustomsModel> RMCustoms { get; set; } = new();
    }
}