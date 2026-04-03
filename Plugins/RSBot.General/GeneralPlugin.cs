using RSBot.Core.Plugins;
using RSBot.General.Components;

namespace RSBot.General
{
    public class GeneralPlugin : IPlugin
    {
        public string InternalName => "RSBot.General";
        public static GeneralPlugin Instance { get; private set; }
        public GeneralManager Manager { get; private set; }

        /// <inheritdoc />
        public void Initialize()
        {
            Instance = this;
            Manager = new GeneralManager();
            Accounts.Load();
        }
        /// <inheritdoc />
        public void OnLoadCharacter() { }
    }
}
