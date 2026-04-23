using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Trade
{
    public class TradeView : IBotbaseView
    {
        public string Name => "RSBot.Trade";
        public string DisplayName => "Trade";
        public string TabText => DisplayName;
        public Control View => Views.View.Main;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
