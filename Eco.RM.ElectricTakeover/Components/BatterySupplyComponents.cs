using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.IoC;
using Eco.RM.ElectricTakeover.Items;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
using Eco.Gameplay.Auth;
using System.ComponentModel;

#nullable enable

namespace Eco.RM.ElectricTakeover.Components
{
    // component that stores batteries.
    [Serialized, LocDescription("Battery storage for a object."), LocDisplayName("Battery Supply"), AutogenClass]
    [DefaultToUnlinked]
    [Priority(PriorityAttribute.Low)]
    [Ecopedia("[RM] Electric Takeover", "Batteries", true, true, "Battery Supply"), CreateComponentTabLoc("Battery Supply"), HasIcon("BatterySupplyComponent")]
    public class BatterySupplyComponent : WorldObjectComponent
    {
        public override LocString ComponentTabName => Localizer.DoStr("Battery Supply");
        public override string IconName => "BatterySupplyComponent";
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.UI;
        [SyncToView, Autogen] public string BatteryChargeDisplay => $"Battery Charge: {GetEnergy()}wh";
        [SyncToView, Autogen] public string TransferRateDisplay => $"Transfer Rate: {WattHoursToWatts(TransferRate)}w";
        [Serialized] public AuthorizationInventory Storage { get; set; }
        public static float WattsToWattHours(float watts) => (watts / 3600);
        public static float WattHoursToWatts(float wattHours) => (wattHours * 3600);
        void UpdateStatus() { Status.SetStatusMessage(Enabled, Localizer.DoStr($"Battery Transfer Rate {WattHoursToWatts(TransferRate)}w"), GetStatus()); }
        public float TransferRate                                    = 0;
        private float energyTransferedLastTick                       = 0;
        [SyncToView, Autogen] public BatteryItem? Battery => getBattery();

        public StatusElement Status { get; set; }

        public float GetEnergy()
        {
            if (Battery == null) return 0;
            return Battery.CurrentCharge;
        }
        public override bool Enabled => true;

        LocString GetStatus()
        { 
            if (Battery == null) return Localizer.DoStr("Missing Battery"); 
            if (Battery.CurrentCharge == Battery.MaxCharge) return Localizer.DoStr("Battery Full"); 
            if (Battery.CurrentCharge == 0) return Localizer.DoStr("Battery Charge Empty");
            return Localizer.DoStr("Low Rate");
        }

        public override void LateTick()
        {
            TransferRate             = energyTransferedLastTick / ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
            energyTransferedLastTick = 0;
        }

        public float ConsumeAsMuchAsPossible(float watts)
        {
            UpdateStatus();
            if (Battery == null) return 0;
            if (watts > Battery.MaxDischargeRate) watts = Battery.MaxDischargeRate;
            float wattHours = WattsToWattHours(watts) * ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
            if (Battery.CurrentCharge >= wattHours)
            {
                Battery.CurrentCharge    -= wattHours;
                energyTransferedLastTick -= wattHours;
                return watts;
            }

            if (Battery.CurrentCharge > 0)
            {
                energyTransferedLastTick -= Battery.CurrentCharge;
                watts = WattHoursToWatts(Battery.CurrentCharge);
                Battery.CurrentCharge = 0;
                return watts;
            }
            return 0;
        }

        BatteryItem? getBattery()
        {
            return (BatteryItem?)Storage.NonEmptyStacks.FirstOrDefault()?.Item;
        }

        public float ProduceAsMuchAsPossible(float watts)
        {
            if (Battery == null) return 0;
            if (watts > Battery.MaxChargeRate) watts = Battery.MaxChargeRate;
            float wattHours = WattsToWattHours(watts) * ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
            if (wattHours + Battery.CurrentCharge <= Battery.MaxCharge)
            {
                Battery.CurrentCharge    += wattHours;
                energyTransferedLastTick += wattHours;
                UpdateStatus();
                return watts;
            }

            if (wattHours +     Battery.CurrentCharge < Battery.MaxCharge)
            {
                energyTransferedLastTick += Battery.CurrentCharge + watts - Battery.MaxCharge;
                watts = WattHoursToWatts(Battery.CurrentCharge + watts - Battery.MaxCharge);
                Battery.CurrentCharge = Battery.MaxCharge;
                UpdateStatus();
                return watts;
            }

            UpdateStatus();
            return 0;
        }
        public override void Initialize()
        {
            Storage = new AuthorizationInventory(1);
            Storage.SetOwner(Parent);
            Status = Parent.GetComponent<StatusComponent>().CreateStatusElement();
            
        }
    }
}

