using RSBot.Core.Plugins;
using RSBot.Party.Bundle;
using RSBot.Party.Subscribers;

namespace RSBot.Party
{
    public class PartyPlugin : IPlugin
    {
        public string InternalName => "RSBot.Party";
        public static PartyPlugin Instance { get; private set; }
        public PartyManager Manager { get; private set; }

        public void Initialize()
        {
            PartySubscriber.SubscribeEvents();
            Instance = this;
            Manager = new PartyManager();
        }
        public void OnLoadCharacter() { }
    }
}
