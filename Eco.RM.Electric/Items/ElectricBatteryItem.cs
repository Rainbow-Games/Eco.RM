using Eco.Core.Controller;
using Eco.Gameplay.Items;
using Eco.Shared.Serialization;

namespace Eco.RM.Items;

public abstract class BatteryItem : DurabilityItem
{
    public virtual int MaxCharge        { get; }
    public virtual int InitialCharge    { get; }
    public virtual int MaxChargeRate    { get; }
    public virtual int MaxDischargeRate { get; }

    [Serialized] private float currentCharge = 0;
    public float CurrentCharge
    {
        get => currentCharge;
        set
        {
            if (currentCharge == value) return;
            currentCharge = value;
            this.Changed(nameof(CurrentCharge));
        }
    }
    BatteryItem()
    {
        CurrentCharge = InitialCharge;
    }
}
