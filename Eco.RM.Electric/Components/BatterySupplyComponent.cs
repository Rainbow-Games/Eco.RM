using Eco.Core.Controller;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.Classes;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using System.ComponentModel;

namespace Eco.RM.Components;

[Serialized]
[AutogenClass, UITypeName("PropertyPage")]
[LocDisplayName("Battery Supply"), LocDescription("Supplies battery power to an object.")]
[HasIcon("BatterySupplyComponentIcon"), CreateComponentTabLoc("Battery Supply")]
public partial class BatterySupplyComponent : WorldObjectComponent, INotifyPropertyChanged
{
    [Serialized] public AuthorizationInventory Inventory { get; private set; }
    [Autogen, ClientInterfaceProperty, UITypeName("PropertyPage"), Notify] 
    [Serialized] public EnergyPool Pool { get; private set; } = new EnergyPool();

    [RPC] public void SetPool(EnergyPool pool) { }

    public override void Initialize() { Initialize(1); }
    public void Initialize(int slots)
    {
        Inventory = new(slots);
        Inventory.AddInvRestriction(new TagRestriction("Battery"));
        Inventory.SetOwner(Parent);
        base.Initialize();
    }
}