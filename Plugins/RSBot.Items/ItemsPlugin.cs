using RSBot.Core.Plugins;

namespace RSBot.Items
{
    public class ItemsPlugin : IPlugin
    {
        /// <inheritdoc />
        public string InternalName => "RSBot.Items";
        public static ItemsPlugin Instance { get; private set; }
        public ItemsManager Manager { get; private set; }

        /// <inheritdoc />
        public void Initialize() 
        {
            Instance = this;
            Manager = new ItemsManager();
        }

        /// <inheritdoc />
        public void OnLoadCharacter() { }

    }
}
