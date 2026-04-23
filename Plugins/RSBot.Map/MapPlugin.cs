using RSBot.Core.Plugins;

namespace RSBot.Map
{
    public class MapPlugin : IPlugin
    {
        public string InternalName => "RSBot.Map";
        public static MapPlugin Instance { get; private set; }
        public MapManager Manager { get; private set; }
        public void Initialize() 
        {
            Instance = this;
            Manager = new MapManager();
        }
        public void OnLoadCharacter()
        {
            Views.View.Instance?.InitUniqueObjects();
        }
    }
}
