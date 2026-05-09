using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Party
{
    public class PartyView : IPluginView
    {
        public string InternalName => "RSBot.Party";
        public string DisplayName => "Party";
        public bool DisplayAsTab => true;
        public int Index => 3;
        public bool RequireIngame => true;
        public Control View => Views.View.Instance;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
            LanguageManager.Translate(Views.View.PartyWindow, Kernel.Language);
        }
    }
}
