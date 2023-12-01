using Eco.Core.Utils;
using Eco.Shared.Utils;

namespace Eco.RM.Framework.Resolvers
{
    public interface IConfigurableCustoms { }
    public class RMCustomsResolver : AutoSingleton<RMCustomsResolver>
    {
        public Dictionary<string, CustomsModel> DefaultCustomsOverrides { get; private set; } = new();
        public Dictionary<string, CustomsModel> LoadedCustomsOverrides { get; private set; } = new();
        public static void AddDefaults(CustomsModel defaults)
        {
            Obj.DefaultCustomsOverrides.Add(defaults.ModelType, defaults);
        }
        public static object GetCustom(Type objectType, string key)
        {
            var objectModel = Obj.LoadedCustomsOverrides.GetValueOrDefault(objectType.Name);
            if (objectModel == null) return null;
            return objectModel.Customs.GetValueOrDefault(key);
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<CustomsModel> newModels = new();
            var previousModels = newModels;
            try
            {
                previousModels = RMCustomsPlugin.Config.RMCustoms;
            }
            catch
            {
                previousModels = new();
            }

            foreach (var type in typeof(IConfigurableCustoms).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultCustomsOverrides.Values.ToList();
            foreach (var lModel in loadtypes)
            {
                var m = previousModels.SingleOrDefault(x => x.ModelType == lModel.ModelType);

                if (m != null) newModels.Add(m);
                else newModels.Add(lModel);
            }

            RMCustomsPlugin.Config.RMCustoms = newModels;
            foreach (var model in newModels)
            {
                if (!LoadedCustomsOverrides.ContainsKey(model.ModelType))
                    LoadedCustomsOverrides.Add(model.ModelType, model);
            }
        }
    }
}
