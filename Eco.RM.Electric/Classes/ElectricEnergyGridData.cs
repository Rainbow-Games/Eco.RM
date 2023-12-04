using Eco.Core.Controller;
using Eco.Shared.Serialization;
using Eco.RM.Interfaces;

namespace Eco.RM.Classes
{
    [Serialized]
    [AutogenClass]
    public class ElectricEnergyGridData
    {
        // Variables
        [Serialized, ThreadSafe] public List<IElectricConsumer>  Consumers  { get; private set; } = new();
        [Serialized, ThreadSafe] public List<IElectricProducer>  Producers  { get; private set; } = new();
        [Serialized, ThreadSafe] public List<IElectricThoughput> Thoughputs { get; private set; } = new();
        [Serialized] public EnergyPool Pool { get; private set; } = new();
    }
}
