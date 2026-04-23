using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Lure
{
    public class LureView : IBotbaseView
    {
        public string Name => "RSBot.Lure";

        public string DisplayName => "Lure";

        public string TabText => DisplayName;
        public Control View => Views.View.Main;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
