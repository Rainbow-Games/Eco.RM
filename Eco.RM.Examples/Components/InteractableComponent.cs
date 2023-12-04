using Eco.Core.Controller;
using Eco.Core.PropertyHandling;
using Eco.Gameplay.Interactions.Interactors;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.EnvVars;
using Eco.Gameplay.Systems.Messaging.Notifications;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Shared.Services;
using Eco.Shared.SharedTypes;

namespace Eco.RM.Components;

[Serialized, AutogenClass, UITypeName("PropertyPage")]
[NoIcon, CreateComponentTabLoc("Roboto's Count")]
[LocDisplayName("MrRoboto"), LocDescription("Runs OPR Eco server.")]
public partial class InteractableComponent : WorldObjectComponent, IHasEnvVars
{
    //UI
    [Eco, LocDisplayName("Count Variable")] public int  Count        { get; set; } = 0;
    [Eco, LocDisplayName("Can Interact")]   public bool Interactable { get; set; } = false;

    //RPC
    [Autogen, RPC] public void Enable(User user)  { Interactable = true;  this.Changed(nameof(Interactable)); }
    [Autogen, RPC] public void Disable(User user) { Interactable = false; this.Changed(nameof(Interactable)); }

    //Interactions
    [Interaction(InteractionTrigger.LeftClick, $"Increase the count", RequiredEnvVars = new string[] {nameof(CanInteract)})]
    [Interaction(InteractionTrigger.LeftClick, $"Decrease the count", InteractionModifier.Shift, RequiredEnvVars = new string[] { nameof(CanInteract) })]
    public void ChangeCount(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
    {
        if (InteractionModifier.Shift == trigger.Modifier) Count--;
        else Count++;
        this.Changed(nameof(Count));
        NotificationManager.ServerMessageToPlayer(Localizer.DoStr($"Updated objects count to {Count}"), player.User, NotificationCategory.Auth, NotificationStyle.Warning);
    }

    [EnvVar, Notify, DependsOnMember(nameof(Interactable))]
    public bool CanInteract(User user)
    {
        if (!Interactable)
        {
            NotificationManager.ServerMessageToPlayer(Localizer.DoStr($"Cannot change count, object not enabled."), user, NotificationCategory.Auth, NotificationStyle.Warning);
            return false;
        }
        return true;
    }
}
