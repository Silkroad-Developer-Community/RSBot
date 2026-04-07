using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Quest
{
    public class QuestView : IPluginView
    {
        public string InternalName => "RSBot.QuestLog";
        public string DisplayName => "Quests";
        public bool DisplayAsTab => false;
        public int Index => 0;
        public bool RequireIngame => true;
        public Control View => Views.View.Main;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
