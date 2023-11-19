using Eco.Core.Items;
using Eco.EM.Framework.Resolvers;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items
{
    [Serialized]
    [Currency]
    [MaxStackSize(1)]
    [Weight(200)]
    [LocDisplayName("Battery")]
    [Tag("Battery")]
    [Ecopedia("Electric Takeover", "Batterys", createAsSubPage: true)]
    [LocDescription("Stores energy for later use.")]
    public partial class SmallBatteryItem : BatteryItem, IConfigurableCustoms
    {
        public override int MaxChargeRate => (int)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "MaxChargeRate");
        public override int MaxDischargeRate => (int)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "MaxDischargeRate");
        public override int MaxCharge => (int)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "MaxCharge");
        [Serialized]
        public override float CurrentCharge => (float)EMCustomsResolver.GetCustom(typeof(SmallBatteryItem), "CurrentCharge");
        public SmallBatteryItem()
        {
            Dictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add("MaxCharge", 60);
            defaults.Add("MaxChargeRate", 30);
            defaults.Add("MaxDischargeRate", 20);
            defaults.Add("CurrentCharge", 0);
            EMCustomsResolver.AddDefaults(new CustomsModel(typeof(SmallBatteryItem), defaults));
        }
    }

    [RequiresSkill(typeof(MiningSkill), 5)]
    public partial class SmallBatteryRecipe : RecipeFamily, IConfigurableRecipe
    {
        static RecipeDefaultModel Defaults => new()
        {
            ModelType = typeof(SmallBatteryRecipe).Name,
            Assembly = typeof(SmallBatteryRecipe).AssemblyQualifiedName,
            
            HiddenName = "Small Battery",
            
            LocalizableName = Localizer.DoStr("Small Battery"),
            
            IngredientList = new()
            {
                new EMIngredient("Wood", true, 80, true),
                new EMIngredient(typeof(CoalItem).Name, false, 20, false),
            },
            ProductList = new()
            {
                new EMCraftable(typeof(BatteryItem).Name, 1),
                new EMCraftable(typeof(CharcoalItem).Name, 8),
            },
            BaseExperienceOnCraft = 30,
            BaseLabor = 1000,
            
            LaborIsStatic = false,
            
            BaseCraftTime = 15,
            
            CraftTimeIsStatic = false,
            
            CraftingStation = typeof(ArrastraItem).Name,
            
            RequiredSkillType = typeof(MiningSkill),
            RequiredSkillLevel = 5,
        };

        static SmallBatteryRecipe() { EMRecipeResolver.AddDefaults(Defaults); }

        public SmallBatteryRecipe()
        {
            this.Recipes = EMRecipeResolver.Obj.ResolveRecipe(this);
            this.LaborInCalories = EMRecipeResolver.Obj.ResolveLabor(this);
            this.CraftMinutes = EMRecipeResolver.Obj.ResolveCraftMinutes(this);
            this.ExperienceOnCraft = EMRecipeResolver.Obj.ResolveExperience(this);
            this.Initialize(Defaults.LocalizableName, GetType());
            CraftingComponent.AddRecipe(EMRecipeResolver.Obj.ResolveStation(this), this);
        }
    }
}
