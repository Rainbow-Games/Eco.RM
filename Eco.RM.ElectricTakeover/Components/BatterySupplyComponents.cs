using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Systems.EnvVars;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System.ComponentModel;
using Eco.Core.PropertyHandling;
using Eco.Gameplay.Interactions.Interactors;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Shared.IoC;
using Eco.Shared.SharedTypes;
using static Eco.Gameplay.Items.AuthorizationInventory;
using Eco.Shared.Utils;
using Eco.RM.ElectricTakeover.Items;

#nullable enable

namespace Eco.RM.ElectricTakeover.Components
{
    // component that stores fuel.
    [Serialized, LocDescription("Battery storage for a object."), LocDisplayName("Battery Supply")]
    [RequireComponent(typeof(StatusComponent))]
    [DefaultToUnlinked]
    [Priority(PriorityAttribute.Low)]
    [Ecopedia(null, "Power Component"), CreateComponentTabLoc("Power"), HasIcon("PowerComponent")]
    public class BatterySupplyComponent : WorldObjectComponent, INotifyPropertyChanged, IHasEnvVars
    {

        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.UI;
        [Serialized] public float Energy => GetEnergy();
        [SyncToView("Battery Charge"), Notify] public string BatteryChargeDisplay => $"%{Energy}%wh";
        [Serialized] public float TransferRate { get; private set; }
        [Serialized] private Inventory BatterySupply { get; set; }
        [Serialized] private BatteryItem? Battery { get; set; }
        [SyncToView] public override string IconName => "BatteryComponent";
        [Serialized] private float energyTransferedLastTick = 0;
        public override bool Enabled => this.Energy > 0;

        readonly StatusElement status;

        public BatterySupplyComponent() 
        {
            this.BatterySupply ??= new AuthorizationInventory(1, AuthorizationFlags.AuthedMayAdd | AuthorizationFlags.AuthedMayRemove);

            this.BatterySupply.SetOwner(this.Parent);
            this.BatterySupply.AddInvRestriction(new TagRestriction("Battery"));

            this.status = this.Parent.GetComponent<StatusComponent>().CreateStatusElement();
        }

        //For fuel supply, we define a separate put, so it doesn't interfere with puts to other storage component. We dont define a 'take', that can be done manually by the player.
        [Interaction(InteractionTrigger.RightClick, $"Insert %{ClientSideEnvVars.SelectedNonTool}% into battery slot", InteractionModifier.Shift, requiredEnvVars: new[] { ClientSideEnvVars.SelectedNonTool, nameof(CanInsertBattery) })]
        public void InsertBattery(Player player)
        {
            if (player.User.Inventory.CarriedItem != null && this.BatterySupply.AcceptsItem(player.User.Inventory.CarriedItem))
            {
                player.User.Inventory.Carried.MoveAsManyItemsAsPossible(this.BatterySupply, player.User);
            } 
            else if (player.User.Inventory.Toolbar.SelectedItem != null && this.BatterySupply.AcceptsItem(player.User.Inventory.Toolbar.SelectedItem))
            {
                player.User.Inventory.Toolbar.MoveAsManyItemsAsPossible(player.User.Inventory.Toolbar.SelectedItem.GetType(), player.User.Inventory.Toolbar.SelectedStack.Quantity, this.BatterySupply);
            }
        }
        [Interaction(InteractionTrigger.LeftClick, $"Take battery from battery slot", InteractionModifier.None, requiredEnvVars: new[] { ClientSideEnvVars.SelectedNonTool, nameof(CanTakeBattery) })]
        public void TakeBattery(Player player)
        {
            if (player.User.Inventory.CarriedItem == null && this.BatterySupply.NonEmptyStacks.Any() == true && this.BatterySupply.NonEmptyStacks.First().Item.IsCarried == true)
            {
                this.BatterySupply.MoveAsManyItemsAsPossible(player.User.Inventory.Carried, player.User);
            }
            else if (player.User.Inventory.Toolbar.SelectedItem == null && this.BatterySupply.NonEmptyStacks.Any() == true && this.BatterySupply.NonEmptyStacks.First().Item.IsCarried == false)
            {
                this.BatterySupply.MoveAsManyItemsAsPossible(this.BatterySupply.NonEmptyStacks.First().GetType(), this.BatterySupply.NonEmptyStacks.First().Quantity, player.User.Inventory.Toolbar);
            }
        }

        [EnvVar, Notify, DependsOnMember(nameof(Inventory))]
        public bool CanInsertBattery(User user) => (user.Inventory.CarriedItem != null && this.BatterySupply.AcceptsItem(user.Inventory.CarriedItem)) ||
                                                   (user.Inventory.Toolbar.SelectedItem != null && this.BatterySupply.AcceptsItem(user.Inventory.Toolbar.SelectedItem));
        [EnvVar, Notify, DependsOnMember(nameof(Inventory))]
        public bool CanTakeBattery(User user)   => (user.Inventory.CarriedItem == null && this.BatterySupply.NonEmptyStacks.Any() == true && this.BatterySupply.NonEmptyStacks.First().Item.IsCarried == true) ||
                                                   (user.Inventory.Toolbar.SelectedItem == null && this.BatterySupply.NonEmptyStacks.Any() == true && this.BatterySupply.NonEmptyStacks.First().Item.IsCarried == false);
        public float GetEnergy()
        {
            if (this.Battery == null) return 0;
            return this.Battery.CurrentCharge;
        }

        public override void Initialize()
        {
            this.BatterySupply.OnChanged.Add(this.OnBatteryAdded);
            this.UpdateStatus();
            base.Initialize();
        }

        public override void Destroy()
        {
            base.Destroy();

            this.BatterySupply.OnChanged.Remove(this.OnBatteryAdded);
        }

        void OnBatteryAdded(User user)
        {
            if (this.Battery != null) return;
            this.LoadBattery();
            this.UpdateStatus();
        }

        void LoadBattery()
        {
            this.BatterySupply.Modify(changeSet =>
            {
                BatteryItem? firstItem = (BatteryItem?)this.BatterySupply.NonEmptyStacks.FirstOrDefault()?.Item;
                if (firstItem != null)
                {
                    this.Battery = firstItem;

                    changeSet.RemoveItem(firstItem.Type);
                }
                else
                {
                    this.Battery = null;
                }
            });
        }

        void UpdateStatus() => this.status.SetStatusMessage(this.Enabled, Localizer.DoStr($"Battery Transfer Rate %{TransferRate}%w"), GetStatus());

        LocString GetStatus()
        { 
            if (Battery == null) return Localizer.DoStr("Missing Battery"); 
            if (Battery.CurrentCharge == Battery.MaxCharge) return Localizer.DoStr("Battery Full"); 
            if (Battery.CurrentCharge == 0) return Localizer.DoStr("Battery Charge Empty");
            return Localizer.DoStr("Unknown Status");
        }

        public override void LateTick()
        {
            this.TransferRate = this.energyTransferedLastTick / ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
            this.energyTransferedLastTick = 0;
        }

        public bool CanUseEnergy(float watts) => this.Energy >= WattsToWattHours(watts);
        public static float WattsToWattHours(float watts) => (watts / 3600) * ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
        public static float WattHoursToWatts(float wattHours) => (wattHours * 3600) / ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;

        public float ConsumeAsMuchAsPossible(float watts)
        {
            if (watts > this.Battery?.MaxDischargeRate) watts = this.Battery.MaxDischargeRate;
            float wattHours = WattsToWattHours(watts);
            if (this.Battery?.CurrentCharge >= wattHours)
            {
                this.Battery.CurrentCharge -= wattHours;
                this.energyTransferedLastTick -= wattHours;
                this.UpdateStatus();
                return watts;
            }

            if (this.Battery?.CurrentCharge > 0)
            {
                this.energyTransferedLastTick -= this.Battery.CurrentCharge;
                watts = WattHoursToWatts(this.Battery.CurrentCharge);
                this.Battery.CurrentCharge = 0;
                this.UpdateStatus();
                return watts;
            }

            this.UpdateStatus();
            return 0;
        }

        public float ProduceAsMuchAsPossible(float watts)
        {
            if (watts > this.Battery?.MaxChargeRate) watts = this.Battery.MaxChargeRate;
            float wattHours = WattsToWattHours(watts);
            if (wattHours + this.Battery?.CurrentCharge <= this.Battery?.MaxCharge)
            {
                this.Battery.CurrentCharge += wattHours;
                this.energyTransferedLastTick += wattHours;
                this.UpdateStatus();
                return watts;
            }

            if (wattHours + this.Battery?.CurrentCharge < this.Battery?.MaxCharge)
            {
                this.energyTransferedLastTick += this.Battery.CurrentCharge + watts - this.Battery.MaxCharge;
                watts = WattHoursToWatts(this.Battery.CurrentCharge + watts - this.Battery.MaxCharge);
                this.Battery.CurrentCharge = this.Battery.MaxCharge;
                this.UpdateStatus();
                return watts;
            }

            this.UpdateStatus();
            return 0;
        }
    }
}

