﻿using Eco.Shared.Serialization;

namespace Eco.RM.Interfaces;

[Serialized]
public interface IElectricProducer
{
    public double ProductionRate { get; }
}
