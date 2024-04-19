using Eco.RM.PowerRework.Components;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.RM.PowerRework.PowerNetworks
{
    [Serialized]
    public partial class RMPowerChannel
    {
        [Serialized] public  int ID;
        [Serialized] private int _NetworkID;

        [Serialized, ThreadSafe] public readonly Dictionary<string, PowerData> PowerData = new();
        public List<RMPowerNetworkComponent> Components => RMPowerNetworkManager.Components.Where(component => component.ChannelID == ID && component.NetworkID == NetworkID).ToList();

        public int NetworkID {  get => _NetworkID; set { if (value != _NetworkID) { _NetworkID = value; ID = RMPowerNetworkManager.GetNextChannelID(value); } } }

        public RMPowerChannel(int networkID)
        {
            NetworkID = networkID;
            //Makes new PowerData from each power type
            Enum.GetNames(typeof(PowerTypes)).ForEach(powerType =>
            {
                var data = new PowerData(powerType);
                data.Changed += PowerDataChanged;
                PowerData.Add(powerType, data);
            });
            RMPowerNetworkManager.Channels.Add(this);
        }
        //Handles PowerData changes and updates the views on components
        public void PowerDataChanged(PowerData data, string propertyName, double newValue)
        {

        }
        public void Destroy()
        {
            RMPowerNetworkManager.Channels.Remove(this);
        }
    }
}
