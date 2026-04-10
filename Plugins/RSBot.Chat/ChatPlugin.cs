using RSBot.Core.Plugins;

namespace RSBot.Chat
{
    public class ChatPlugin : IPlugin
    {
        public string InternalName => "RSBot.Chat";
        public static ChatPlugin Instance { get; private set; }
        public ChatManager Manager { get; private set; }
        public void Initialize()
        {
            Instance = this;
            Manager = new ChatManager();
        }
        public void OnLoadCharacter() { } 
    }
}
