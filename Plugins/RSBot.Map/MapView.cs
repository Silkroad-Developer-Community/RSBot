using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Map
{
    public class MapView : IPluginView
    {
        public string InternalName => "RSBot.Map";
        public string DisplayName => "Map";
        public bool DisplayAsTab => true;
        public int Index => 6;
        public bool RequireIngame => true;
        public Control View => Views.View.Instance;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
