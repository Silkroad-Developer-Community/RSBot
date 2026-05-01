using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Protection
{
    public class ProtectionView : IPluginView
    {
        public string InternalName => "RSBot.Protection";
        public string DisplayName => "Protection";
        public bool DisplayAsTab => true;
        public int Index => 2;
        public bool RequireIngame => true;
        public Control View => Views.View.Instance;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
