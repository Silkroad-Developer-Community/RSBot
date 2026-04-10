
using RSBot.Core.Plugins;
using RSBot.Skills.Subscriber;

namespace RSBot.Skills
{
    public class SkillsPlugin : IPlugin
    {
        public string InternalName => "RSBot.Skills";
        public static SkillsPlugin Instance { get; private set; }
        public SkillsManager Manager { get; private set; }
        public void Initialize()
        {
            Instance = this;
            Manager = new SkillsManager();
            LoadCharacterSubscriber.SubscribeEvents();
        }
        public void OnLoadCharacter() {}
    }
}
