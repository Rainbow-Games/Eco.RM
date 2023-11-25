using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.RM.ElectricTakeover.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using System.Collections.Generic;

namespace Eco.RM.ElectricTakeover.Components
{
    [Serialized, AutogenClass, Priority(PriorityAttribute.Low)]
    [LocDescription("Battery storage for a object."), LocDisplayName("Battery Storage")]
    [Ecopedia("[RM] Electric Takeover", "Batteries", true, true, "Battery Storage"), CreateComponentTabLoc("Battery Storage"), HasIcon("BatterySupplyComponent")]
    public abstract class BatteryStorageComponent : PublicStorageComponent
    {
        [LocDisplayName("Capacity"), LocDescription("The total capacity of all batteries inserted.")]
        [Serialized, Autogen] public float TotalBatteryMaxCharge { get; private set; } = 0;

        [LocDisplayName("Charge"), LocDescription("The current charge of the storage depending on inserted battery charges.")]
        [Autogen] public float TotalBatteryCharge => GetCharge();

        [LocDisplayName("Max Charge Rate"), LocDescription("The max charge rate of the storage. Add higher teir batteries to increase.")]
        [Serialized, Autogen] public float TotalBatteryMaxChargeRate { get; private set; } = 0;

        [LocDisplayName("Max Discharge Rate"), LocDescription("The max discharge rate of the storage. Add higher teir batteries to increase.")]
        [Serialized, Autogen] public float TotalBatteryMaxDischargeRate { get; private set; } = 0;

        [Serialized] public Dictionary<BatteryItem, ItemStack> Batteries { get; private set; } = new Dictionary<BatteryItem, ItemStack>();

        public static readonly ThreadSafeAction<BatterySupplyComponent> ChargeChanged = new(); 

        public new void Initialize(int slots)
        {
            base.Initialize(slots);
            base.Inventory.AddInvRestriction(new TagRestriction("Battery"));
            base.Inventory.SetOwner(Parent);
            base.Inventory.OnChanged.Add(OnInventoryChanged);
        }
        public void OnInventoryChanged(User user)
        {
            if (Batteries.Count > base.Inventory.NonEmptyStacks.Count())
            {
                var batteries = Batteries;
                base.Inventory.NonEmptyStacks.ForEach(battery => batteries.Remove((BatteryItem)battery.Item));
                if (!batteries.Any()) return;
                OnBatteryRemoved(batteries.First());
            }
            if (Batteries.Count < base.Inventory.NonEmptyStacks.Count())
            {
                var batteries = base.Inventory.NonEmptyStacks;
            }
        }
        public void OnBatteryAdded(ItemStack batteryStack)
        {
            var battery = (BatteryItem)batteryStack.Item;
            Batteries.Add((BatteryItem)batteryStack.Item, batteryStack);
            TotalBatteryMaxCharge += battery.MaxCharge;
            TotalBatteryMaxChargeRate += battery.MaxChargeRate;
            TotalBatteryMaxDischargeRate += battery.MaxDischargeRate;
        }
        public void OnBatteryRemoved(KeyValuePair<BatteryItem, ItemStack> battery)
        {
            TotalBatteryMaxCharge -= battery.Key.MaxCharge;
            TotalBatteryMaxChargeRate -= battery.Key.MaxChargeRate;
            TotalBatteryMaxDischargeRate -= battery.Key.MaxDischargeRate;
            Batteries.Remove(battery.Key);
        }
        public float GetCharge()
        {
            var totalCharge = 0f;
            Batteries.ForEach(battery => totalCharge += battery.Key.CurrentCharge);
            return totalCharge;
        }
        public float ChangeCharge(float value)
        {
            if (value == 0) return 0;
            if (value < 0)
            {
                value = Math.Abs(value);
                if (value > TotalBatteryMaxDischargeRate) value = TotalBatteryMaxDischargeRate;
                if (value > TotalBatteryCharge) value = TotalBatteryCharge;
            }
            if (value > 0)
            {
                if (value > TotalBatteryMaxChargeRate) value = TotalBatteryMaxChargeRate;
                if (value > TotalBatteryCharge) value = TotalBatteryCharge;
            }
        }
    }
}
