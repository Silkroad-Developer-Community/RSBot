using RSBot.Core.Components;

namespace RSBot.Core
{
    public class BotCL
    {
        public static void Initialize(string profile)
        {
            ProfileManager.SetSelectedProfile(profile);
            GlobalConfig.Load();
            Kernel.Initialize();
            Game.Initialize();
            Game.InitializeArchiveFiles();
            Game.ReferenceManager.Load();
            Kernel.PluginManager.LoadAssemblies();
            Kernel.BotbaseManager.LoadAssemblies();
        }
    }
}
