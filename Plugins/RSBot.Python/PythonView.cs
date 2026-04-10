using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Python
{
    public class PythonView : IPluginView
    {
        public string InternalName => "RSBot.Python";
        public string DisplayName => "PyPlugins";
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
