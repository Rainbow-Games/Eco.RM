using Eco.Core.Controller;
using Eco.Gameplay.Objects;
using Eco.RM.PowerRework.PowerNetworks;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.RM.PowerRework.Components
{
    [Serialized]
    [AutogenClass]
    [LocDisplayName("Power Network Component")]
    [LocDescription("Displays the current power network data for the world object.")]
    [CreateComponentTabLoc("Power Network"), NoIcon]
    public partial class RMPowerNetworkComponent : WorldObjectComponent
    {
        #region AutogenUI
        [Autogen, SyncToView, LocDisplayName("Network ID"), LocDescription("The ID of the power network this component belongs to.")]
        public string NetworkIDView => $"{NetworkID}";
        [RPC] public void SetNetworkIDView(string id)
        {
            if (id.ToInt(-1) < 0) { this.Changed(nameof(NetworkID)); return; }
            NetworkID = id.ToInt(-1);
        }
        [Autogen, SyncToView, LocDisplayName("Channel ID"), LocDescription("The ID of the power network channel this component belongs to.")]
        public string ChannelIDView => $"{ChannelID}";
        [RPC] public void SetChannelIDView(string id) { }
        #endregion AutogenUI
        #region Fakes
        [Serialized] private int _NetworkID = -1;
        [Serialized] private int _ChannelID = -1;
        #endregion Fakes
        public int NetworkID { get => _NetworkID; set { if (_NetworkID != value) { _NetworkID = value; this.Changed(nameof(NetworkIDView)); } } }
        public int ChannelID { get => _ChannelID; set { if (_ChannelID != value) { _ChannelID = value; this.Changed(nameof(ChannelIDView)); } } }
        public RMPowerNetworkComponent() 
        {
            if (!RMPowerNetworkManager.Components.Contains(this)) RMPowerNetworkManager.QueueComponentAdded(this);
        }
        public override void Destroy()
        {
            RMPowerNetworkManager.QueueComponentRemoved(this);
        }
    }
}
