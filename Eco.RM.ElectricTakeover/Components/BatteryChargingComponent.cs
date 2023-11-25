using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System.ComponentModel;

namespace Eco.RM.ElectricTakeover.Components
{

    [Serialized, LocDescription("Charges batterys."), LocDisplayName("Battery Charger"), AutogenClass]
    [RequireComponent(typeof(BatterySupplyComponent))]
    [Ecopedia("[RM] Electric Takeover", "Batteries", true, true, "Battery Charger"), CreateComponentTabLoc("Battery Charger"), HasIcon("BatteryChargingComponent")]
    public class BatteryChargingComponent : WorldObjectComponent
    {
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.UI;
        [SyncToView, Autogen] public string MaxChargeRate => $"{ChargeRate}w";
        [SyncToView, Autogen] public string CurrentChargeRate             => $"Charge Rate: {currentChargeRate}w";
        [SyncToView, Autogen] public string TotalPowerCharged => $"Total Power Charged: {totalPowerCharged}wh";
        public float totalPowerCharged = 0;
        public float ChargeRate = 0;
        public float currentChargeRate = 0;
        public BatterySupplyComponent Supply => Parent.GetComponent<BatterySupplyComponent>();
        public void Initialize(float chargeRate)
        {
            ChargeRate = chargeRate;
            Supply.Initialize();
        }

        public override bool Enabled => true;

        public override void Tick()
        {
            float Consumed = Supply.ProduceAsMuchAsPossible(ChargeRate);
            if (Consumed > 0) totalPowerCharged += Consumed;
            currentChargeRate = Consumed;
        }
    }
}