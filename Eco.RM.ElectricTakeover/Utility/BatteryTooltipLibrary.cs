using Eco.Gameplay.Systems.NewTooltip;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;
using Eco.RM.ElectricTakeover.Items;
using Eco.Shared.IoC;
using Eco.Shared.Items;
using Eco.Shared.Localization;

namespace Eco.RM.ElectricTakeover.Utility
{
    [TooltipLibrary] public static class BatteryTooltipLibrary
    {
        public static void Initialize() => BatteryItem.ChargeChanged.Add(MarkTooltipChargeDirty);
        static void MarkTooltipChargeDirty(BatteryItem item) => ServiceHolder<ITooltipSubscriptions>.Obj.MarkTooltipPartDirty(nameof(BatteryItem), instance: item);

        [NewTooltip(CacheAs.Instance | CacheAs.User, 11, TTCat.Controls, flags: TTFlags.ClearCacheForAllUsers)]
        public static LocString TooltipCharge(this BatteryItem item)
        {
            var s = new LocStringBuilder();
            s.Append(Localizer.DoStr($"Charge: %{item.CurrentCharge}%wh / %{item.MaxCharge}%wh"));
            s.AppendLine(Localizer.DoStr($"Max Discharge Rate: %{item.MaxDischargeRate}%"));
            s.AppendLine(Localizer.DoStr($"Max Charge Rate: %{item.MaxChargeRate}%"));
            return new TooltipSection(Localizer.DoStr("Battery Information"), s.ToLocString());
        }
    }
}
