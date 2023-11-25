using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Math;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Components.Storage;

namespace Eco.RM.ElectricTakeover.WorldObjects
{
    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(BatteryChargingComponent))]
    [RequireComponent(typeof(BatterySupplyComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(OnOffComponent))]
    public partial class TestingObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(TestingItem);
        public override LocString DisplayName   => Localizer.DoStr("Test Object");

        static TestingObject()
        {
            AddOccupancy<TestingObject>(new List<BlockOccupancy>(){
                new(new Vector3i(0, 0, 0)),
            });
        }

        protected override void Initialize()
        {
            this.GetComponent<BatteryChargingComponent>().Initialize(90);
        }
    }

    [Serialized]
    [LocDisplayName("Testing Item")]
    [LocDescription("Object used for testing.")]
    public partial class TestingItem : WorldObjectItem<TestingObject> { }
}
