using Eco.Core.Controller;
using Eco.Core.PropertyHandling;
using Eco.RM.Electric.Utils;
using Eco.RM.Plugins;
using Eco.RM.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.Classes
{
    [Serialized]
    [AutogenClass]
    public class EnergyPool
    {
        // Display Strings
        [LocDisplayName("Stored Energy"), LocDescription("The ammount of energy the pool has.")]
        [Autogen, SyncToView, Notify, UITypeName("StringDisplay")] public string StoredEnergyDisplay => ElectricPlugin.Obj.Formater.StoredUnit(StoredEnergy);

        [LocDisplayName("Energy Input"), LocDescription("The ammount of energy the pool is reciving.")]
        [Autogen, SyncToView, Notify, UITypeName("StringDisplay")] public string EnergyInputDisplay => ElectricPlugin.Obj.Formater.Unit(EnergyInput);

        [LocDisplayName("Energy Output"), LocDescription("The ammount of energy the pool is sending.")]
        [Autogen, SyncToView, Notify, UITypeName("StringDisplay")] public string EnergyOutputDisplay => ElectricPlugin.Obj.Formater.Unit(EnergyOutput);

        [LocDisplayName("Status"), LocDescription("If supply does not meet demand then all consumers and producers will turn off until it does.")]
        [Autogen, SyncToView, Notify, UITypeName("StringDisplay")] public string StatusDisplay => Enabled ? $"System Operational: {status}" : $"System Error: {status}";

        // RPCs
        //[RPC] public void SetStoredEnergyDisplay(string value) => this.FirePropertyChanged(nameof(StoredEnergyDisplay));
        //[RPC] public void SetEnergyInputDisplay(string value)  => this.FirePropertyChanged(nameof(EnergyInputDisplay));
        //[RPC] public void SetEnergyOutputDisplay(string value) => this.FirePropertyChanged(nameof(EnergyOutputDisplay));
        //[RPC] public void SetStatusDisplay(string value)       => this.FirePropertyChanged(nameof(StatusDisplay));

        // Variables
        [Serialized] private double storedEnergy { get; set; }         = 0;
        [Serialized] public double  Capacity     { get; set; }         = 0;
        [Serialized] private double energyInput  { get; set; }         = 0;
        [Serialized] private double energyOutput { get; set; }         = 0;
        [Serialized] private bool   enabled      { get; set; }         = true;
        [Serialized] private string status       { get; set; }         = "System Operational";

        public double NetEnergyRate => EnergyInput - EnergyOutput;

        private double cachedInput  = 0;
        private double cachedOutput = 0;

        // Setters
        public double StoredEnergy
        { 
            get => storedEnergy; 
            set 
            {
                if (storedEnergy == value) return;
                storedEnergy = value;
                this.FirePropertyChanged(nameof(StoredEnergyDisplay));
            } 
        }

        public double EnergyInput
        {
            get => energyInput;
            set
            {
                if (energyInput == value) return;
                energyInput = value;
                this.FirePropertyChanged(nameof(EnergyInputDisplay));
            }
        }
        public double EnergyOutput
        {
            get => energyOutput;
            set
            {
                if (energyOutput == value) return;
                energyOutput = value;
                this.FirePropertyChanged(nameof(EnergyOutputDisplay));
            }
        }

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value) enabled = value;
                this.FirePropertyChanged(nameof(StatusDisplay));
            }
        }

        // Methods
        public void LateTick()
        {
            if (Enabled != CheckEnabled()) SetStatus("Energy shortage");
            if (!Enabled)
            {
                cachedInput = 0;
                cachedOutput = 0;
                return;
            }
            EnergyInput = cachedInput;
            EnergyOutput = cachedOutput;
            StoredEnergy += ElectricMathUtil.UnitToHour(cachedInput - cachedOutput);
            if (StoredEnergy > Capacity) StoredEnergy = Capacity;
            cachedInput = 0;
            cachedOutput = 0;
        }

        private bool CheckEnabled()
        {
            if (StoredEnergy + ElectricMathUtil.UnitToHour(cachedInput - cachedOutput) < 0) return false;
            return true;
        }
        private bool SetStatus(string error, string operational = "Energy demand met.")
        {
            var on = CheckEnabled();
            status = on ? operational : error;
            Enabled = on;
            return on;
        }
        public EnergyPool() { }
        public EnergyPool(double capacity)
        {
            Capacity = capacity;
        }
    }
}
