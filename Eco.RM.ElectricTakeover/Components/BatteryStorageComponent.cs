using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.RM.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Components.Storage;
using System.ComponentModel;
using Eco.Gameplay.Players;
using Eco.Shared.View;

namespace Eco.RM.Components
{
    [Serialized]
    [AutogenClass]
    [RequireComponent(typeof(PublicStorageComponent))]
    [LocDisplayName("Battery Storage")]
    [LocDescription("Battery storage for a object.")]
    [HasIcon("BatteryStorageComponentIcon")]
    [CreateComponentTabLoc("Battery Storage")]
    [Ecopedia("[RM] Stored Power", "Batteries", true, true, "Battery Storage")]
    public partial class BatteryStorageComponent : WorldObjectComponent, INotifyPropertyChanged
    {
        [LocDisplayName("Max Charge"), LocDescription("The total capacity of all batteries inserted.")]
        [SyncToView(SyncFlags.UnreliableChangeNotification), Autogen, ReadOnly(true), Notify] protected string MaxChargeDisplay => Localizer.DoStr($"{Math.Floor(MaxCharge)} Watt Hours");

        [LocDisplayName("Max Discharge Rate"), LocDescription("The max discharge rate of the storage. Add higher teir batteries to increase.")]
        [SyncToView(SyncFlags.UnreliableChangeNotification), Autogen, ReadOnly(true), Notify] protected string MaxDischargeRateDisplay => Localizer.DoStr($"{Math.Floor(MaxDischargeRate)} Watts");

        [LocDisplayName("Max Charge Rate"), LocDescription("The max charge rate of the storage. Add higher teir batteries to increase.")]
        [SyncToView(SyncFlags.UnreliableChangeNotification), Autogen, ReadOnly(true), Notify] protected string MaxChargeRateDisplay => Localizer.DoStr($"{Math.Floor(MaxChargeRate)} Watts");

        [LocDisplayName("Current Charge"), LocDescription("The current charge of the storage depending on inserted battery charges.")]
        [SyncToView(SyncFlags.UnreliableChangeNotification), Autogen, ReadOnly(true), Notify] protected string CurrentChargeDisplay => Localizer.DoStr($"{Math.Floor(CurrentCharge)} Watt Hours");

        [LocDisplayName("Transfer Rate"), LocDescription("The energy transfer rate of the storage in watts.")]
        [SyncToView(SyncFlags.UnreliableChangeNotification), Autogen, ReadOnly(true), Notify] protected string EnergyTransferRateDisplay => Localizer.DoStr($"{Math.Floor(EnergyTransferRate)} Watts");

        public float MaxCharge => Inventory.NonEmptyStacks.Sum((itemStack) => ((BatteryItem)itemStack.Item).MaxCharge);
        public float MaxDischargeRate => Inventory.NonEmptyStacks.Sum((itemStack) => ((BatteryItem)itemStack.Item).MaxDischargeRate);
        public float MaxChargeRate => Inventory.NonEmptyStacks.Sum((itemStack) => ((BatteryItem)itemStack.Item).MaxChargeRate);
        public float CurrentCharge => Inventory.NonEmptyStacks.Sum((itemStack) => ((BatteryItem)itemStack.Item).CurrentCharge);
        [Serialized] public float EnergyTransferRate { get; private set; }

        public Inventory Inventory => Parent.GetOrCreateComponent<PublicStorageComponent>().Inventory;
        float energyChangeLastTick;

        public ThreadSafeAction OnStateChanged { get; set; } = new ThreadSafeAction();

        public void Initialize(int slots)
        {
            Parent.GetOrCreateComponent<PublicStorageComponent>().Initialize(slots);
            Inventory.AddInvRestriction(new TagRestriction(new Tag[] {TagManager.GetOrMake("Battery")}));
            Inventory.SetOwner(Parent);
        }
        public float Charge(float watts)
        {
            var og_watts = watts;
            Inventory.NonEmptyStacks.ForEach((itemStack) => {
                watts -= ((BatteryItem)itemStack.Item).Charge(watts);
            });
            energyChangeLastTick += og_watts - watts;
            return og_watts - watts;
        }
        public float Discharge(float watts)
        {
            var og_watts = watts;
            Inventory.NonEmptyStacks.ForEach((itemStack) => {
                watts -= ((BatteryItem)itemStack.Item).Discharge(watts);
            });
            energyChangeLastTick -= og_watts - watts;
            return og_watts - watts;
        }
        public override void LateTick()
        {
            EnergyTransferRate = energyChangeLastTick;
            energyChangeLastTick = 0;
        }
    }
}
