using Eco.Gameplay.Objects;
using Eco.Shared.IoC;

namespace Eco.RM.Utils
{
    public static class ElectricMathUtil
    {
        public static double UnitToHour(double units) => units / 3600 / ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
        public static double HourToUnit(double hours) => hours * 3600 * ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime;
    }
}
