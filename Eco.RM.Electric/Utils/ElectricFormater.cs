using Eco.RM.Configs;
using Eco.Shared.Localization;

namespace Eco.RM.Electric.Utils
{
    public class ElectricFormater
    {
        private ElectricConfig Config { get; }

        public string Unit(double units)       => Localizer.DoStr(units > 1000 ? $"{Math.Round(units / 1000, 2)} {Config.EnergyUnitKDisplay}"       : $"{Math.Round(units, 2)} {Config.EnergyUnitDisplay}");
        public string StoredUnit(double units) => Localizer.DoStr(units > 1000 ? $"{Math.Round(units / 1000, 2)} {Config.StoredEnergyUnitKDisplay}" : $"{Math.Round(units, 2)} {Config.StoredEnergyUnitDisplay}");

        public ElectricFormater(ElectricConfig config)
        {
            Config = config;
        }
    }
}
