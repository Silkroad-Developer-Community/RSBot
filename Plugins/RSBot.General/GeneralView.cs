using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.General
{
    public class GeneralView : IPluginView
    {
        /// <inheritdoc />
        public string InternalName => "RSBot.General";

        /// <inheritdoc />
        public string DisplayName => "General";

        /// <inheritdoc />
        public bool DisplayAsTab => true;

        /// <inheritdoc />
        public int Index => 0;

        /// <inheritdoc />
        public bool RequireIngame => false;

        /// <inheritdoc />
        public Control View => Views.View.Instance;

        /// <inheritdoc />
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
            LanguageManager.Translate(Views.View.PendingWindow, Kernel.Language);
            LanguageManager.Translate(Views.View.AccountsWindow, Kernel.Language);
        }
    }
}
