using Eco.Core.Items;
using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.RM.Functions;

namespace Eco.RM.Items
{
    [Serialized]
    [ItemGroup("Batteries")]
    [MaxStackSize(1)]
    [Tag("Battery")]
    [LocDescription("A device for storing power")]
    [Ecopedia("[RM] Stored Power", "Batteries", true, true, "Battery")]
    public abstract class BatteryItem : ToolItem
    {
        public override float GetDurability()
        {
            return CurrentCharge / MaxCharge * 100;
        }
        public static readonly ThreadSafeAction<BatteryItem> ChargeChanged = new();
        public virtual float MaxCharge => 0;    // The max watt hours the battery can store.
        public virtual float MaxChargeRate => 0;     // The max watts the battery can take in.
        public virtual float MaxDischargeRate => 0;     // The max watts the battery can output.
        public virtual float InitialCharge => 0;
        [Serialized] private float _CurrentCharge =  0;
        public float CurrentCharge
        {
            get => _CurrentCharge;

            set
            {
                if (value == _CurrentCharge) return;
                _CurrentCharge = value;
                ChargeChanged.Invoke(this);
            }
        }
        public BatteryItem()
        {
            CurrentCharge = InitialCharge;
        }
        public float Discharge(float watts)
        {
            watts = Math.Clamp(watts, 0, MaxDischargeRate);
            watts = Math.Clamp(watts, 0, ElectricMath.WattHoursToWatts(CurrentCharge));
            CurrentCharge -= ElectricMath.WattsToWattHours(watts);
            return watts;
        }
        public float Charge(float watts)
        {
            watts = Math.Clamp(watts, 0, MaxChargeRate);
            watts = Math.Clamp(watts, 0, ElectricMath.WattHoursToWatts(MaxCharge - CurrentCharge));
            CurrentCharge += ElectricMath.WattsToWattHours(watts);
            return watts;
        }
    }
}