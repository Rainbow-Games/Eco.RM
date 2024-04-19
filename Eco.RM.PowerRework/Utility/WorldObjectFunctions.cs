using Eco.Core.Systems;
using Eco.Gameplay.Objects;
using Eco.Shared.View;

namespace Eco.RM.PowerRework.Utility
{
    public static class WorldObjectFunctions
    {
        public static void DestroyComponent(WorldObject worldObject, WorldObjectComponent component)
        {
            worldObject.Components.Remove(component);
            UniversalIDs.Remove(component);
            component.Destroy();
            component.UnsubscribeAll(true);
        }
    }
}
