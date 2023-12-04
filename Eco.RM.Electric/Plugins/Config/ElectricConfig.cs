using Eco.Shared.Localization;

namespace Eco.RM.Configs
{
    public class ElectricConfig
    {
        [LocDescription("The name of some energy units")]        public string EnergyUnitDisplay        { get; set; } = "Kilowatts";
        [LocDescription("The name of 1k energy units")]          public string EnergyUnitKDisplay       { get; set; } = "Megawatts";
        [LocDescription("The name of some stored energy units")] public string StoredEnergyUnitDisplay  { get; set; } = "Kilowatt hours";
        [LocDescription("The name of 1k stored energy units")]   public string StoredEnergyUnitKDisplay { get; set; } = "Megawatt hours";

    }
}