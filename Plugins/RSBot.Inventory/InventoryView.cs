using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Inventory
{
    public class InventoryView : IPluginView
    {
        /// <inheritdoc />
        public string InternalName => "RSBot.Inventory";

        /// <inheritdoc />
        public string DisplayName => "Inventory";

        /// <inheritdoc />
        public bool DisplayAsTab => true;

        /// <inheritdoc />
        public int Index => 4;

        /// <inheritdoc />
        public bool RequireIngame => true;

        /// <inheritdoc />
        public Control View => Views.View.Instance;

        /// <inheritdoc />
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
