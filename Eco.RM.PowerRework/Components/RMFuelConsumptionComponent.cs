using Eco.Core.Controller;
using Eco.Gameplay.Objects;
using Eco.Shared.Serialization;

namespace Eco.RM.PowerRework.Components
{
    [Serialized]
    [ForceCreateView, AutogenClass]
    [CreateComponentTabLoc("Fuel Consumption"), NoIcon]
    public partial class RMFuelConsumptionComponent : WorldObjectComponent
    {
    }
}
