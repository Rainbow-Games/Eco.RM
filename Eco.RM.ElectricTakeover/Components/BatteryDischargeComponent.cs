using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.RM.ElectricTakeover.Components
{

    [Serialized, LocDescription("Battery power for an electric object."), LocDisplayName("Battery Discharger"), AutogenClass]
    [RequireComponent(typeof(BatterySupplyComponent))]
    [Ecopedia("[RM] Electric Takeover", "Batteries", true, true, "Battery Discharger"), CreateComponentTabLoc("Battery Discharger"), HasIcon("BatteryDischargeComponent")]
    public class BatteryDischargeComponent : BatterySupplyComponent
    {
        public override WorldObjectComponentClientAvailability Availability          => WorldObjectComponentClientAvailability.UI;
        [SyncToView, Autogen] public string RequiredTransferRate     => $"{WattsRequired}w";
        [SyncToView, Autogen] public string TotalPowerDischarged => $"{totalPowerConsumed}wh";
        public float totalPowerConsumed = 0;
        public float WattsRequired      = 0;
        public BatterySupplyComponent Supply => Parent.GetComponent<BatterySupplyComponent>(); 

        public void Initialize(float wattsRequired)
        {
            WattsRequired = wattsRequired;
            Supply.Initialize();
        }

        public override bool Enabled => enabled;
        private bool enabled         =  true;

        public override void Tick()
        {
            float Consumed = Supply.ConsumeAsMuchAsPossible(WattsRequired);
            if (Consumed > 0) totalPowerConsumed += Consumed;
            if (Consumed < WattsRequired)
            {
                enabled = false;
                return;
            }
            if (!enabled) enabled = true;
        }
    }
}