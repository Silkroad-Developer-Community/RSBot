using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Statistics
{
    public class StatisticsView : IPluginView
    {
        public string InternalName => "RSBot.Statistics";
        public string DisplayName => "Statistics";
        public bool DisplayAsTab => true;
        public int Index => 97;
        public bool RequireIngame => true;
        public Control View => Views.View.Instance;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
