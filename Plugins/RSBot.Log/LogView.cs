
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Log
{
    public class LogView : IPluginView
    {
        public string InternalName => "RSBot.Log";        
        public string DisplayName => "Log";        
        public bool DisplayAsTab => true;        
        public int Index => 99;        
        public bool RequireIngame => false;        
        public Control View => Views.View.Instance;        
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
