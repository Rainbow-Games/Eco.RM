using Eco.Gameplay.Systems.NewTooltip;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;
using Eco.RM.ElectricTakeover.Items;
using Eco.Shared.IoC;
using Eco.Shared.Items;
using Eco.Shared.Localization;

namespace Eco.RM.ElectricTakeover.Utility
{
    [TooltipLibrary] public static class BatteryItemTooltipLibrary
    {
        public static void Initialize() => BatteryItem.ChargeChanged.Add(MarkTooltipChargeDirty);
        static void MarkTooltipChargeDirty(BatteryItem item) => ServiceHolder<ITooltipSubscriptions>.Obj.MarkTooltipPartDirty(nameof(CurrentCharge), instance: item);

        [NewTooltip(CacheAs.Instance | CacheAs.User, 1, TTCat.Controls, flags: TTFlags.ClearCacheForAllUsers)]
        public static LocString CurrentCharge(this BatteryItem item)
        {
            var s = new LocStringBuilder();
            if (item.CurrentCharge == 0) s.AppendLine(Localizer.DoStr("Charge Depleted"));
            if (item.CurrentCharge == item.MaxCharge) s.AppendLine(Localizer.DoStr("Fully Charged"));
            s.AppendLine(Localizer.DoStr($"{item.CurrentCharge}wh / {item.MaxCharge}wh"));
            s.AppendLine(Localizer.DoStr($"{item.CurrentCharge}w / {item.MaxCharge}w"));
            return new TooltipSection(Localizer.DoStr("Current Charge"), s.ToLocString());
        }
        [NewTooltip(CacheAs.SubType, 1, overrideType: typeof(BatteryItem))]
        public static LocString BatteryInformation(BatteryItem BatteryItem)
        {
            var s = new LocStringBuilder();
            s.AppendLine(Localizer.DoStr($"Max Charge: {BatteryItem.MaxCharge}"));
            s.AppendLine(Localizer.DoStr($"Max Discharge Rate: {BatteryItem.MaxDischargeRate}"));
            s.AppendLine(Localizer.DoStr($"Max Charge Rate: {BatteryItem.MaxChargeRate}"));
            return new TooltipSection(Localizer.DoStr("Battery Information"), s.ToLocString());
        }
    }
}
