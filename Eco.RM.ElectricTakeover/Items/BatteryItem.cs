using Eco.Core.Items;
using Eco.Gameplay.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System.ComponentModel;

namespace Eco.RM.ElectricTakeover.Items
{
    [Serialized]
    [MaxStackSize(1)]
    [Weight(200)]
    [LocDisplayName("Battery")]
    [Tag("Battery")]
    [Category("Hidden")]
    [LocDescription("Stores energy for later use.")]
    public partial class BatteryItem : ToolItem
    {
        public virtual int MaxChargeRate => 0;
        public virtual int MaxDischargeRate => 0;
        public virtual int MaxCharge => 0;
        public virtual float CurrentCharge => 0;
        public override float GetDurability()
        {
            return CurrentCharge / MaxCharge;
        }
        static BatteryItem()
        {

        }
    }
}
