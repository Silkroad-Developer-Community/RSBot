using RSBot.Core.Plugins;
using RSBot.General.Components;
using RSBot.Core.Components.Command;

namespace RSBot.General
{
    public class GeneralPlugin : IPlugin
    {
        public string InternalName => "RSBot.General";
        public static GeneralPlugin Instance { get; private set; }
        public GeneralManager Manager { get; private set; }

        public void Initialize()
        {
            Instance = this;
            Manager = new GeneralManager();
            Accounts.Load();

            CLIManager.Register(new StartClientCommand());
            CLIManager.Register(new ShowClientCommand());
            CLIManager.Register(new HideClientCommand());
        }
        public void OnLoadCharacter() { }
    }
}
