using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Math;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Occupancy;
using Eco.RM.Components;

namespace Eco.RM.Items
{
    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(BatteryChargerComponent))]
    public partial class BatteryChargerObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(BatteryChargerItem);
        public override LocString DisplayName   => Localizer.DoStr("Battery Charger");

        static BatteryChargerObject()
        {
            AddOccupancy<BatteryChargerObject>(new List<BlockOccupancy>(){
                new(new Vector3i(0, 0, 0)),
            });
        }

        protected override void Initialize()
        {
            GetOrCreateComponent<BatteryChargerComponent>().Initialize(3, 50);
        }
    }

    [Serialized]
    [LocDisplayName("Battery Charger")]
    [LocDescription("Object used for charging batteries.")]
    public partial class BatteryChargerItem : WorldObjectItem<BatteryChargerObject> { }
}
