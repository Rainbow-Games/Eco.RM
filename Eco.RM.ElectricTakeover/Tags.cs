using Eco.Core.Plugins.Interfaces;
using Eco.Gameplay.Items;
using Eco.Shared.Localization;

namespace  Eco.RM.Tags
{
    public class StoredEnergyTagDefinitions : IModInit
    {
        private static readonly TagDefinition[] Definitions =
        {
            new TagDefinition("Battery") {PluralName = Localizer.DoStr("Batteries"), ShowInEcopedia = true, ShowInFilter = true, AutoHighlight = false}
        };
        public static void Initialize()
        {
            foreach (var definition in Definitions)
                TagDefinition.Register(definition);
        }
    }
}
