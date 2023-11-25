using Eco.Gameplay.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items
{
    [Serialized]
    [LocDisplayName("Small Battery")]
    public partial class SmallBatteryItem : BatteryItem
    {
        public override float MaxCharge => 40;
        public override float MaxChargeRate => 20;
        public override float MaxDischargeRate => 80;

    }
}