using Eco.Shared.Serialization;

namespace Eco.RM.Interfaces;

[Serialized]
public interface IElectricConsumer
{
    public double ConsumptionRate { get; }
}
