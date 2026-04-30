using RSBot.Core;

namespace RSBot.Quest
{
    public class QuestManager
    {
        public static void AbandonQuest(uint questId)
        {
            if (Game.Player == null || Game.Player.QuestLog == null)
                return;

            if (Game.Player.QuestLog.ActiveQuests.TryGetValue(questId, out var activeQuest))
            {
                Game.Player.QuestLog.AbandonQuest(questId);
            }
        }
    }
}
