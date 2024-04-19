using Eco.Gameplay.Players;
using Eco.RM.PowerRework.Components;
using Eco.Shared.Serialization;

namespace Eco.RM.PowerRework.PowerNetworks
{
    //Power networks act as a cotantainer for linkable channels under the same permissions.
    [Serialized]
    public partial class RMPowerNetwork
    {
        [Serialized] public readonly int ID = RMPowerNetworkManager.GetNextNetworkID();

        [Serialized, ThreadSafe] public List<User> Owners              = new(); //The person who initialy created the network and any fully authorized users.
        [Serialized, ThreadSafe] public List<User> AuthorizedConsumers = new(); //People who can utilize the network.
        public List<RMPowerChannel> Channels => RMPowerNetworkManager.Channels.Where(channel => channel.NetworkID == ID).ToList();
        public List<RMPowerNetworkComponent> Components => RMPowerNetworkManager.Components.Where(component => component.NetworkID == ID).ToList();

        public RMPowerNetwork(User creator)
        {

            RMPowerNetworkManager.Networks.Add(this);
        }
        public void Destroy()
        {
            RMPowerNetworkManager.Networks.Remove(this);
        }
        public void Add(RMPowerNetworkComponent component)
        {

        }
    }
}
