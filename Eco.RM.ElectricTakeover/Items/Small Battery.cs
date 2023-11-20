using Eco.Core.Items;
using Eco.EM.Framework.Resolvers;
using Eco.Gameplay.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items
{
    [Serialized]
    [LocDisplayName("Small Battery")]
    [Tag("Battery")]
    public partial class SmallBatteryItem : BatteryItem, IConfigurableCustoms
    {
        public override float InitialCharge    => (float)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "InitialCharge");
        public override float MaxCharge        => (float)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "MaxCharge");
        public override float MaxChargeRate    => (float)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "MaxChargeRate");
        public override float MaxDischargeRate => (float)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "MaxDischargeRate");
        public SmallBatteryItem()
        {
            Dictionary<string, object> defaults = new()
            {
                { "InitialCharge", 0 },
                { "MaxCharge", 40 },
                { "MaxChargeRate", 20 },
                { "MaxDischargeRate", 80 }
            };
            EMCustomsResolver.AddDefaults(new CustomsModel(typeof(SmallBatteryItem), defaults));
        }

    }
}