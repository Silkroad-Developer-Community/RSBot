using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Alchemy
{
    public class AlchemyView : IBotbaseView
    {
        public string Name => "RSBot.Alchemy";
        public string DisplayName => "Alchemy";
        public string TabText => DisplayName;
        public Control View => Globals.View;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
