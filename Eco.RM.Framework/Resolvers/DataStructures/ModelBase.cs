using Eco.Shared.Localization;
using System.ComponentModel;

namespace Eco.RM.Framework.Resolvers
{
    [LocDisplayName("Base Model")]
    public class ModelBase
    {
        [LocDisplayName("Model Type"), ReadOnly(true)] public string ModelType { get; set; }
        [LocDisplayName("Qualified Assembly"), ReadOnly(true)] public string Assembly { get; set; }
    }
}