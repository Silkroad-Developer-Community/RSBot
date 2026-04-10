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
            Kernel.PluginManager.LoadAssemblies(true);
            Kernel.BotbaseManager.LoadAssemblies(true);
            LoadExtensions();
        }
        public static void LoadExtensions()
        {
            foreach (var plugin in Kernel.PluginManager.Extensions.Values)
                plugin.Initialize();
        }
    }
}
