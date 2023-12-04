using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.Items;

[Serialized]
[RequireComponent(typeof(BatterySupplyComponent))]
[RequireComponent(typeof(PropertyAuthComponent))]
public partial class TestingObject : WorldObject, IRepresentsItem
{
    public virtual  Type      RepresentedItemType => typeof(TestingItem);
    public override LocString DisplayName         => Localizer.DoStr("Test Object");
}

[Serialized]
[LocDisplayName("Testing Item"), LocDescription("Object used for testing.")]
public partial class TestingItem : WorldObjectItem<TestingObject> { }