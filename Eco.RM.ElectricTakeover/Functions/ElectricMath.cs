using Eco.Gameplay.Objects;
using Eco.Shared.IoC;

namespace Eco.RM.Functions
{
    public static class ElectricMath
    {
        public static float WattsToWattHours(float watts) => watts / (ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime * 3600);
        public static float WattHoursToWatts(float wattHours) => wattHours * (ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime * 3600);
    }
}
