using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System.ComponentModel;

namespace Eco.RM.ElectricTakeover.Items
{
    [Serialized]
    [ItemGroup("Batteries")]
    [MaxStackSize(1)]
    [Tag("Battery")]
    [LocDescription("A device for storing power")]
    public abstract class BatteryItem : ToolItem
    {
        public override float GetDurability()
        {
            return base.GetDurability();
        }
        public static readonly ThreadSafeAction<BatteryItem> ChargeChanged = new();
        public virtual float MaxCharge => 0;    // The max watt hours the battery can store.
        public virtual float MaxChargeRate => 0;     // The max watts the battery can take in.
        public virtual float MaxDischargeRate => 0;     // The max watts the battery can output.
        [Serialized] private float currentCharge =  0;
        public float CurrentCharge
        {
            get => currentCharge;

            set
            {
                if (value == currentCharge) return;
                currentCharge = value;
                ChargeChanged.Invoke(this);
            }
        }
    }
}