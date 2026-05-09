using RSBot.Core;
using RSBot.Core.Event;
using RSBot.Core.Objects;

namespace RSBot.Chat
{
    public class ChatManager
    {
        string lastWhisper;
        public ChatManager() { SubscribeEvents(); }
        public static void Send(ChatType type, string message, string receiver = null)
        {
            if (type == ChatType.Global)
                Bundle.Chat.SendGlobalChatPacket(message);
            else
                Bundle.Chat.SendChatPacket(type, message, receiver);

            if (type == ChatType.Private)
                PlayerConfig.Set("RSBot.Chat.LastWhisper", receiver);
        }
        private void SubscribeEvents()
        {
            EventManager.SubscribeEvent("OnEnterGame", OnEnterGame);
        }
        private void OnEnterGame()
        {
            lastWhisper = PlayerConfig.Get<string>("RSBot.Chat.LastWhisper");
        }
    }
}
