using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.Items
{
    [Serialized]
    [LocDisplayName("Small Battery")]
    public partial class SmallBatteryItem : BatteryItem
    {
        public override float MaxCharge => 40;
        public override float MaxChargeRate => 20;
        public override float MaxDischargeRate => 80;
        public override float InitialCharge => 10;
    }
}