using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.Items
{
    [Serialized]
    [LocDisplayName("Large Battery")]
    public partial class LargeBatteryItem : BatteryItem
    {
        public override float MaxCharge => 120;
        public override float MaxChargeRate => 60;
        public override float MaxDischargeRate => 240;
        public override float InitialCharge => 0;
    }
}