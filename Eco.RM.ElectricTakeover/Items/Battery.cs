using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items
{
    [Serialized]
    [ItemGroup("Batteries")]
    [Tag("Battery")]
    public abstract class BatteryItem : DurabilityItem
    {
        public static readonly ThreadSafeAction<BatteryItem> ChargeChanged = new();
        public override float GetDurability()
        {
            return CurrentCharge / MaxCharge;
        }
        public abstract float MaxCharge       { get; }     // The max watt hours the battery can store.
        public virtual float MaxChargeRate    { get; }     // The max watts the battery can take in.
        public virtual float MaxDischargeRate { get; }     // The max watts the battery can output.
        public virtual float InitialCharge    { get; }     // The initial watt hours stored when the battery is created.
        [Serialized] private float currentCharge =  0;
        [Notify]
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
        public BatteryItem()
        {
            this.CurrentCharge = this.InitialCharge;
        }
        
    }
}