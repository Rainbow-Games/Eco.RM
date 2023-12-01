using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.Components
{
    [Serialized]
    [AutogenClass]
    [LocDisplayName("Battery Charger")]
    [LocDescription("Charges batteries from it's storage")]
    [HasIcon("BatteryChargerComponentIcon")]
    [CreateComponentTabLoc("Battery Charger")]
    [Ecopedia("[RM] Stored Power", "Batteries", true, true, "Battery Charger")]
    public partial class BatteryChargerComponent : BatteryStorageComponent
    {
        public float TargetTransferRate { get; set; }
        public void Initialize(int slots, float chargeRate)
        {
            Initialize(slots);
            TargetTransferRate = chargeRate;
        }
        public override void Tick()
        {
            Charge(TargetTransferRate);
        }
    }
}
