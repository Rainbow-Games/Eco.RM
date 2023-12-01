using Eco.RM.Framework.Utility;
using Eco.Shared.Localization;
using System.Text.Json.Serialization;

namespace Eco.RM.Framework.Resolvers
{
    public class CustomsModel : ModelBase
    {
        [LocDisplayName("Customs")]
        public Dictionary<string, object> Customs { get; set; }
        public CustomsModel(Type type, Dictionary<string, object> customs)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            Customs = customs;

        }
        [JsonConstructor]
        public CustomsModel(string modelType, string assembly, Dictionary<string, object> customs)
        {
            ModelType = modelType;
            Assembly = assembly;
            Customs = customs;
        }

        public override string ToString() => $"{StringUtility.GetAssemblyNameFromAssemblyString(Assembly)} - {ModelType}";
    }
}