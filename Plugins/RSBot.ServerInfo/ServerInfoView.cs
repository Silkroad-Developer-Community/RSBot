using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.ServerInfo
{
    public class ServerInfoView : IPluginView
    {
        public string InternalName => "RSBot.ServerInfo";
        public string DisplayName => "Server Information";
        public bool DisplayAsTab => false;
        public int Index => 100;
        public bool RequireIngame => false;
        public Control View => Views.View.Main;
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
