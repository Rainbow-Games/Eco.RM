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

[Serialized, AutogenClass]
[NoIcon, CreateComponentTabLoc("Interaction Count")]
[LocDisplayName("Interaction Count"), LocDescription("A component that allows you to interact with a world object.")]
public partial class InteractableComponent : WorldObjectComponent, IHasEnvVars
{
    //UI Display (editable)
    [Eco, LocDisplayName("Count Variable")] public int  Count        { get; set; } = 0;
    [Eco, LocDisplayName("Can Interact")]   public bool Interactable { get; set; } = false;

    //RPC Buttons
    [Autogen, RPC] public void Enable(User user)  { Interactable = true;  this.Changed(nameof(Interactable)); }
    [Autogen, RPC] public void Disable(User user) { Interactable = false; this.Changed(nameof(Interactable)); }

    //Interactions
    //Takes in the player that is interacting, the trigger which allows you to see modifiers, and the target (object in this case).
    //The interaction attribute allows it to function as an interaction.
    //Takes in the trigger which is a input type, the string to display on the interaction, a optional modifier which is secondary keys, and an optional EnvVars.
    [Interaction(InteractionTrigger.LeftClick, $"Increase the count", RequiredEnvVars = new string[] {nameof(CanInteract)})]
    [Interaction(InteractionTrigger.LeftClick, $"Decrease the count", InteractionModifier.Shift, new string[] { nameof(CanInteract) })]
    public void ChangeCount(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
    {
        if (InteractionModifier.Shift == trigger.Modifier) Count--;
        else Count++;
        this.Changed(nameof(Count));
        NotificationManager.ServerMessageToPlayer(Localizer.DoStr($"Updated objects count to {Count}"), player.User, NotificationCategory.Auth, NotificationStyle.Warning);
    }

    //You can also add client side EnvVars to the display string like so
    [Interaction(InteractionTrigger.LeftClick, $"Do something with {ClientSideEnvVars.Carried}", RequiredEnvVars = new string[] { ClientSideEnvVars.Carried, nameof(CanInteract) })]
    public void DoSomething(Player player, InteractionTriggerInfo trigger, InteractionTarget target) { }

    //Interaction Verification.
    //Takes in the user of the player trying to interact.
    //Returns a boolean that repersents the clients ability to interact.
    //DependsOnMember Allows the interaction to update as the property changes.
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
