using RSBot.Core.Plugins;
using RSBot.Inventory.Subscriber;

namespace RSBot.Inventory
{
    public class InventoryPlugin : IPlugin
    {
        public string InternalName => "RSBot.Inventory";
        public static InventoryPlugin Instance { get; private set; }
        public InventoryManager Manager { get; private set; }
        public void Initialize()
        {
            Instance = this;
            Manager = new InventoryManager();
            BuyItemSubscriber.SubscribeEvents();
            InventoryUpdateSubscriber.SubscribeEvents();
            UseItemAtTrainplaceSubscriber.SubscribeEvents();
        }
        public void OnLoadCharacter() { }
    }
}
