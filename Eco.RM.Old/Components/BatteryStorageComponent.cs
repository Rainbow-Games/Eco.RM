using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Notifications;
using Eco.RM.Items;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Shared.Services;
using Eco.Shared.Utils;

namespace Eco.RM.Components
{
    [Serialized]
    [AutogenClass]
    [LocDisplayName("Battery Storage")]
    [LocDescription("Battery storage for a object.")]
    [HasIcon("BatteryStorageComponentIcon")]
    [CreateComponentTabLoc("Battery Storage")]
    [Ecopedia("[RM] Stored Power", "Batteries", true, true, "Battery Storage")]
    public partial class BatteryStorageComponent : WorldObjectComponent
    {
        public float MaxCharge => Inventory.NonEmptyStacks.Sum((ItemStack battery) => ((BatteryItem)battery.Item).MaxCharge);
        public float CurrentCharge => Inventory.NonEmptyStacks.Sum((ItemStack battery) => ((BatteryItem)battery.Item).CurrentCharge);
        public float MaxChargeRate => Inventory.NonEmptyStacks.Sum((ItemStack battery) => ((BatteryItem)battery.Item).MaxChargeRate);
        public float MaxDischargeRate => Inventory.NonEmptyStacks.Sum((ItemStack battery) => ((BatteryItem)battery.Item).MaxDischargeRate);
        [Serialized] public float LastCurrentCharge = 0;
        [Serialized] public float TransferRate = 0;
        [Serialized] public AuthorizationInventory Inventory { get; set; }

        [LocDescription("Inserts one battery from your toolbar into the storage.")]
        [RPC, Autogen]
        public void InsertBattery(Player player)
        {
            MoveBatteriesFromPlayer(player);
        }

        [LocDescription("Inserts all batteries from your toolbar into the storage.")]
        [RPC, Autogen]
        public void InsertAllBatteries(Player player)
        {
            MoveBatteriesFromPlayer(player, -1);
        }

        [LocDescription("Inserts all batteries that are the same teir as the one in your hand from your toolbar into the storage.")]
        [RPC, Autogen]
        public void InsertFilteredBatteries(Player player)
        {
            MoveBatteriesFromPlayer(player);
        }

        [LocDescription("Takes all batteries from storage and puts them in your inventory.")]
        [RPC, Autogen]
        public void TakeBatteries(Player player)
        {
            MoveBatteriesToPlayer(player);
        }

        [LocDisplayName("Max Charge"), LocDescription("The max charge of the storage.")]
        [SyncToView, Autogen, Notify, AutoRPC]
        public string MaxChargeDisplay { get => Localizer.DoStr($"{Math.Round(MaxCharge, 2)}wh"); set { } }

        [LocDisplayName("Current Charge"), LocDescription("The current charge of the storage.")]
        [SyncToView, Autogen, Notify, AutoRPC]
        public string CurrentChargeDisplay { get => Localizer.DoStr($"{Math.Round(CurrentCharge, 2)}wh"); set { } }

        [LocDisplayName("Max Charge Rate"), LocDescription("The max charge rate of the storage.")]
        [SyncToView, Autogen, Notify, AutoRPC]
        public string MaxChargeRateDisplay { get => Localizer.DoStr($"{Math.Round(MaxChargeRate)}w"); set { } }

        [LocDisplayName("Max Discharge Rate"), LocDescription("The max discharge rate of the storage.")]
        [SyncToView, Autogen, Notify, AutoRPC]
        public string MaxDischargeRateDisplay { get => Localizer.DoStr($"{Math.Round(MaxDischargeRate)}w"); set { } }

        [LocDisplayName("Energy Transfer Rate"), LocDescription("The net transfer rate of the storage.")]
        [SyncToView, Autogen, Notify, AutoRPC]
        public string TransferRateDisplay
        {
            get
            {
                if (!Inventory.NonEmptyStacks.Any()) return Localizer.DoStr($"Waiting for batteries");
                if (TransferRate > 0) return Localizer.DoStr($"+{Math.Abs(TransferRate)}w.");
                if (TransferRate < 0) return Localizer.DoStr($"-{Math.Abs(TransferRate)}w.");
                if (CurrentCharge == 0) return Localizer.DoStr($"Storage depleted.");
                if (CurrentCharge == MaxCharge) return Localizer.DoStr($"Storage full.");
                return Localizer.DoStr($"Inactive.");
            }
            set { }
        }

        public void updateStats()
        {
            this.Changed(nameof(MaxChargeDisplay));
            this.Changed(nameof(CurrentChargeDisplay));
            this.Changed(nameof(MaxChargeRateDisplay));
            this.Changed(nameof(MaxDischargeRateDisplay));
            this.Changed(nameof(TransferRateDisplay));
        }

        public void Initialize(int slots)
        {
            Inventory = new(slots);
            Inventory.AddInvRestriction(new TagRestriction(new Tag[] { TagManager.GetOrMake("Battery") }));
            Inventory.SetOwner(Parent);
            Inventory.OnChanged.Add((User user) => updateStats());
            updateStats();
        }
        public override void Destroy()
        {
            Inventory.OnChanged.Remove((User user) => updateStats());
            base.Destroy();
        }
        public float Charge(float watts)
        {
            var og_watts = watts;
            Inventory.NonEmptyStacks.ForEach((itemStack) =>
            {
                watts -= ((BatteryItem)itemStack.Item).Charge(watts);
            });
            return og_watts - watts;
        }
        public float Discharge(float watts)
        {
            var og_watts = watts;
            Inventory.NonEmptyStacks.ForEach((itemStack) =>
            {
                watts -= ((BatteryItem)itemStack.Item).Discharge(watts);
            });
            return og_watts - watts;
        }

        public override void LateTick()
        {
            if (TransferRate != ElectricMath.WattHoursToWatts(CurrentCharge - LastCurrentCharge))
            {
                TransferRate = ElectricMath.WattHoursToWatts(CurrentCharge - LastCurrentCharge);
                this.Changed(nameof(TransferRateDisplay));
            }
            if (CurrentCharge != LastCurrentCharge)
            {
                LastCurrentCharge = CurrentCharge;
                this.Changed(nameof(CurrentChargeDisplay));
            }
        }

        public void MoveBatteriesFromPlayer(Player player, int quantity = 1)
        {
            var c = 0;
            player.User.Inventory.Toolbar.Stacks.ForEach((stack) =>
            {
                if (c == quantity) return;
                if (Inventory.IsFull) return;
                if (stack.Item.Type.BaseType != typeof(BatteryItem)) return;
                player.User.Inventory.Toolbar.MoveItems(player: player, sourceStack: stack, target: Inventory);
                c++;
            });
            if (c == 0) NotificationManager.ServerMessageToPlayerLocStr($"Storage full.", player.User, category: NotificationCategory.Notifications, style: NotificationStyle.InfoBox);
            else if (Inventory.IsFull) NotificationManager.ServerMessageToPlayerLocStr($"Moved {c} batteries into storage. Storage is now full.", player.User, category: NotificationCategory.Notifications, style: NotificationStyle.InfoBox);
            else if (c == quantity || quantity == -1) NotificationManager.ServerMessageToPlayerLocStr($"Moved {c} batteries into storage.", player.User, category: NotificationCategory.Notifications, style: NotificationStyle.InfoBox);
        }
        public void MoveBatteryFromPlayerFiltered(Player player, int quantity = 1)
        {
            if (player.User.Inventory.Toolbar.SelectedItem.Type != typeof(BatteryItem))
            {
                NotificationManager.ServerMessageToPlayerLocStr($"Invalid filter type {player.User.Inventory.Toolbar.SelectedItem.DisplayName}. Filter type must be a {typeof(BatteryItem).GetLocDisplayName()}", player.User, category: NotificationCategory.Notifications, style: NotificationStyle.Error);
                return;
            }
            var c = 0;
            player.User.Inventory.Toolbar.Stacks.ForEach((stack) =>
            {
                if (c == quantity) return;
                if (Inventory.IsFull) return;
                if (stack.Item.Type.BaseType != typeof(BatteryItem)) return;
                if (stack.Item.Type != player.User.Inventory.Toolbar.SelectedItem.Type) return;
                player.User.Inventory.Toolbar.MoveItems(player: player, sourceStack: stack, target: Inventory);
                c++;
            });
            if (c == 0) NotificationManager.ServerMessageToPlayerLocStr($"Storage full.", player.User, category: NotificationCategory.Notifications, style: NotificationStyle.Warning);
            else if (Inventory.IsFull) NotificationManager.ServerMessageToPlayerLocStr($"Moved {c} batteries into storage. Storage is now full.", player.User, category: NotificationCategory.Notifications, style: NotificationStyle.InfoBox);
            else if (c == quantity || quantity == -1) NotificationManager.ServerMessageToPlayerLocStr($"Moved {c} batteries into storage.", player.User, category: NotificationCategory.Notifications, style: NotificationStyle.InfoBox);
        }
        public void MoveBatteriesToPlayer(Player player) => Inventory.MoveItems(player: player, source: Inventory, target: player.User.Inventory, Item.Get(typeof(BatteryItem)));
    }
}
