using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Core.Utils.Threading;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Objects;
using Eco.RM.PowerRework.Components;
using Eco.RM.PowerRework.Utility;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using System.Collections.Concurrent;
using System.Text;

namespace Eco.RM.PowerRework.PowerNetworks
{
    //Manages all power networks and channels and the creation of new components.
    [Serialized]
    [Priority(-1)]
    public partial class RMPowerNetworkManager : Singleton<RMPowerNetworkManager>, IThreadedPlugin, IDisplayablePlugin, IModKitPlugin
    {
        [Serialized, ThreadSafe] public static List<RMPowerNetwork>                  Networks          = new();
        [Serialized, ThreadSafe] public static List<RMPowerChannel>                  Channels          = new();
        [Serialized, ThreadSafe] public static List<RMPowerNetworkComponent>         Components        = new();

        readonly PluginTickTimer tickTimer = new("RMPowerNetworkManager");

        public static ConcurrentQueue<(RMPowerNetworkComponent, bool)> queuedChangedComponents = new();
        public static ConcurrentQueue<WorldObject> queuedObjectsAdded = new();
        public static AutoResetEvent OnQueuedEvent { get; private set; } = new(false);
        public static void QueueObjectAdded(WorldObject worldObject)
        {
            queuedObjectsAdded.Enqueue(worldObject);
            OnQueuedEvent.Set();
        }
        public static void QueueComponentAdded(RMPowerNetworkComponent component)
        {
            queuedChangedComponents.Enqueue((component, true));
            OnQueuedEvent.Set();
        }

        public static void QueueComponentRemoved(RMPowerNetworkComponent component)
        {
            queuedChangedComponents.Enqueue((component, false));
            OnQueuedEvent.Set();
        }

        EventDrivenWorker tickWorker;
   
        public void     Run()             => tickWorker.Start(ThreadPriorityTaskFactory.BelowNormal);
        public Task     ShutdownAsync()   => tickWorker.ShutdownAsync();
        public string   GetCategory()     => Localizer.DoStr("Raynbo Mods");
        public string   GetStatus()       => Localizer.Do($"Running");
        public override string ToString() => Localizer.DoStr("Power Grids");

        public RMPowerNetworkManager()
        {
            Console.WriteLine("Adding WorldObjectAddedEvent and editing components");
            WorldObjectManager.WorldObjectAddedEvent.Add(QueueObjectAdded);
            WorldObjectManager.ForEach(QueueObjectAdded);
            tickWorker = new(OnQueuedEvent, Tick);
        }

        public static int GetNextNetworkID()              { var ids = Networks.Select(network => network.ID); var id = 1; while (ids.Contains(id)) id++; return id; }
        public static int GetNextChannelID(int networkID) { var ids = Channels.Where(channel => channel.NetworkID == networkID).Select(channel => channel.ID); var id = 1; while (ids.Contains(id)) id++; return id; }

        public string GetDisplayText()
        {
            var sb = new StringBuilder();
            sb.AppendLine(tickTimer.ToString().TrimEnd());

            return sb.ToString();
        }
        public int Tick()
        {
            tickTimer.BeginTick();
            ProcessNewObjects();
            tickTimer.EndTick();

            return GetSleepTime();
        }

        // Gets the time to wait between ticks.
        int GetSleepTime()
        {
            const double targetCpuUsage = 0.05;
            const double minSleepTimeMs = 1000;
            const double maxSleepTimeMs = 5000;

            var duration = tickTimer.LastTickTotalTime;
            return (int)Math.Round((duration / targetCpuUsage) - duration).Clamp(minSleepTimeMs, maxSleepTimeMs);
        }
        public void ProcessNewObjects()
        {
            var worldObjects = queuedObjectsAdded.Consume();
            foreach (var worldObject in worldObjects)
            {
                if (!worldObject.Initialized)
                {
                    Console.WriteLine($"WorldObject: {worldObject.Name}, has not been initialized yet");
                    QueueObjectAdded(worldObject);
                    continue;
                }
                EditComponents(worldObject);
            }
        }
        // Pulls data from components and replaces them with there reworked versions.
        public static void EditComponents(WorldObject worldObject)
        {
            Console.WriteLine(worldObject.Name);
            if (worldObject.HasComponent<FuelConsumptionComponent>())
            {
                var fuelConsumptionComponent = worldObject.GetComponent<FuelConsumptionComponent>();
                var fuelSupplyComponent = worldObject.GetComponent<FuelSupplyComponent>();
                var newComponent = worldObject.GetOrCreateComponent<RMFuelConsumptionComponent>();
                //TODO: component initialization with old values
                WorldObjectFunctions.DestroyComponent(worldObject, fuelConsumptionComponent);
                WorldObjectFunctions.DestroyComponent(worldObject, fuelSupplyComponent);
            }
            if (worldObject.HasComponent<PowerConsumptionComponent>())
            {
                var powerConsumptionComponent = worldObject.GetComponent<PowerConsumptionComponent>();
                var newComponent = worldObject.GetOrCreateComponent<RMPowerConsumptionComponent>();
                //TODO: component initialization with old values
                WorldObjectFunctions.DestroyComponent(worldObject, powerConsumptionComponent);
            }
            if (worldObject.HasComponent<PowerGeneratorComponent>())
            {
                var powerGeneratorComponent = worldObject.GetComponent<PowerGeneratorComponent>();
                var newComponent = worldObject.GetOrCreateComponent<RMPowerGenerationComponent>();
                //TODO: component initialization with old values
                WorldObjectFunctions.DestroyComponent(worldObject, powerGeneratorComponent);
            }
            if (worldObject.HasComponent<PowerGridComponent>())
            {
                WorldObjectFunctions.DestroyComponent(worldObject, worldObject.GetComponent<PowerGridComponent>());
                worldObject.GetOrCreateComponent<RMPowerNetworkComponent>();
            }
            if (worldObject.HasComponent<PowerGridNetworkComponent>())
            {
                WorldObjectFunctions.DestroyComponent(worldObject, worldObject.GetComponent<PowerGridNetworkComponent>());
                worldObject.GetOrCreateComponent<RMPowerNetworkComponent>();
            }
        }
        public static void ProcessAddedComponent(RMPowerNetworkComponent component)
        {
        }
        public static void ProcessRemovedComponent(RMPowerNetworkComponent component)
        {
        }
        public static bool TryChangeNetwork(RMPowerNetworkComponent component, int newNetworkID)
        {
            var networks = Networks.Where(network => network.ID == component.NetworkID);
            if (!networks.Any()) return false;
            return true;
        }
    }
}
