using Eco.Core.Controller;
using Eco.Core.PropertyHandling;
using Eco.RM.PowerRework.Utility;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using System.ComponentModel;

namespace Eco.RM.PowerRework.PowerNetworks
{
    [Eco]
    public enum PowerTypes
    {
        Heat = 0,
        Electric = 1,
        Mechanical = 2,
    }
    //Holds the electrical data for the power grid
    [Serialized]
    public partial class PowerData
    {
        //The max and current stats of all machines on the channel.
        [Serialized] private double _MaxProduction      = 0;
        [Serialized] private double _MaxConsumption     = 0;
        [Serialized] private double _MaxStorage         = 0;
        [Serialized] private double _CurrentProduction  = 0;
        [Serialized] private double _CurrentConsumption = 0;
        [Serialized] private double _CurrentStorage     = 0;

        public double MaxProduction      { get => _MaxProduction;      set { if (_MaxProduction      != value) { _MaxProduction      = value; OnChanged(nameof(MaxProduction),      value); } } }
        public double MaxConsumption     { get => _MaxConsumption;     set { if (_MaxConsumption     != value) { _MaxConsumption     = value; OnChanged(nameof(MaxConsumption),     value); } } }
        public double MaxStorage         { get => _MaxStorage;         set { if (_MaxStorage         != value) { _MaxStorage         = value; OnChanged(nameof(MaxStorage),         value); } } }
        public double CurrentProduction  { get => _CurrentProduction;  set { if (_CurrentProduction  != value) { _CurrentProduction  = value; OnChanged(nameof(CurrentProduction),  value); } } }
        public double CurrentConsumption { get => _CurrentConsumption; set { if (_CurrentConsumption != value) { _CurrentConsumption = value; OnChanged(nameof(CurrentConsumption), value); } } }
        public double CurrentStorage     { get => _CurrentStorage;     set { if (_CurrentStorage     != value) { _CurrentStorage     = value; OnChanged(nameof(CurrentStorage),     value); } } }

        [Serialized] public string PowerType;
        public event PowerDataChangedEventHandler? Changed;

        //Fires the Changed event when a property changes.
        protected virtual void OnChanged(string propertyName, double newValue)
        {
            Changed?.Invoke(this, propertyName, newValue);
        }
        public PowerData(string powerType)
        {
            PowerType = powerType;
        }
    }
}
