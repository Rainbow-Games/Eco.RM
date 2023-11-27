using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.View;
using System.ComponentModel;

namespace Eco.RM.Components
{
    [Serialized]
    [AutogenClass]
    [RequireComponent(typeof(StatusComponent))]
    [LocDisplayName("Battery Charger")]
    [LocDescription("Charges batteries from it's storage")]
    [HasIcon("BatteryChargerComponentIcon")]
    [CreateComponentTabLoc("Battery Charger")]
    [Ecopedia("[RM] Stored Power", "Batteries", true, true, "Battery Charger")]
    public partial class BatteryChargerComponent : BatteryStorageComponent, INotifyPropertyChanged
    {
        [LocDisplayName("Max Transfer Rate"), LocDescription("The max rate that the charger can transfer energy at.")]
        [SyncToView(SyncFlags.UnreliableChangeNotification), Autogen, ReadOnly(true), Notify] protected string MaxTransferRateDisplay => Localizer.DoStr($"{Math.Floor(MaxTransferRate)} Watts");
        [Serialized] protected float MaxTransferRate { get; private set; }
        public override bool Enabled => EnergyTransferRate != 0;
        float lastEnergyTransferRate = 0;
        StatusElement status;
        public void Initialize(int slots, float chargeRate)
        {
            Initialize(slots);
            MaxTransferRate = chargeRate;
            status = Parent.GetOrCreateComponent<StatusComponent>().CreateStatusElement();
            UpdateStatus();
            Inventory.OnChanged.Add((User user) => UpdateStatus());
        }
        public override void Tick()
        {
            Charge(MaxTransferRate);

            if (lastEnergyTransferRate != EnergyTransferRate)
            {
                UpdateStatus();
                lastEnergyTransferRate = EnergyTransferRate;
            }
        }
        public void UpdateStatus() {
            var s = $"Unknown Status | Transfer Rate: {EnergyTransferRate}w";
            if (MaxCharge == CurrentCharge) s = "Batteries are full";
            if (Inventory.NonEmptyStacks.Count() == 0) s = "No battery inserted";
            status.SetStatusMessage(Enabled, Localizer.DoStr($"Charging Stored Batteries | Transfer Rate: {EnergyTransferRate}w"), Localizer.DoStr(s));
        }
        public override void Destroy()
        {
            Inventory.OnChanged.Remove((User user) => UpdateStatus());
            base.Destroy();
        }
    }
}
