using Eco.Core.PropertyHandling;
using Eco.Gameplay.Items;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;
using Eco.RM.Items;
using Eco.Shared.Items;
using Eco.Shared.Localization;

namespace Eco.RM.Tooltips
{
    [TooltipLibrary]
    public static class BatteryItemTooltipLibrary
    {
        public static void Initialize() { }

        [TooltipAffectedBy(nameof(BatteryItem.CurrentCharge))]
        [NewTooltip(cacheMode: CacheAs.Instance, flags: TTFlags.ForceInstantUpdate, overrideType: typeof(BatteryItem))]
        public static LocString CurrentCharge(BatteryItem batteryItem)
        {
            var s = new LocStringBuilder();
            if (batteryItem.CurrentCharge == 0) s.AppendLine(Localizer.DoStr("Charge Depleted"));
            if (batteryItem.CurrentCharge == batteryItem.MaxCharge) s.AppendLine(Localizer.DoStr("Fully Charged"));
            s.AppendLine(Localizer.DoStr($"{batteryItem.CurrentCharge} Watt Hours / {batteryItem.MaxCharge} Watt Hours"));
            s.AppendLine(Localizer.DoStr($"{Math.Floor(ElectricMath.WattHoursToWatts(batteryItem.CurrentCharge))} Watts / {Math.Floor(ElectricMath.WattHoursToWatts(batteryItem.MaxCharge))} Watts"));
            return new TooltipSection(Localizer.DoStr("Current Charge"), s.ToLocString());
        }
        [NewTooltip(CacheAs.SubType, 10, overrideType: typeof(BatteryItem))]
        public static LocString BatteryInformation(Type batteryItemType)
        {
            var batteryItem = (BatteryItem)Item.Get(batteryItemType);
            var s = new LocStringBuilder();
            s.AppendLine(Localizer.DoStr($"Max Charge: {batteryItem.MaxCharge}"));
            s.AppendLine(Localizer.DoStr($"Max Discharge Rate: {batteryItem.MaxDischargeRate}"));
            s.AppendLine(Localizer.DoStr($"Max Charge Rate: {batteryItem.MaxChargeRate}"));
            return new TooltipSection(Localizer.DoStr("Battery Information"), s.ToLocString());
        }
    }
}
