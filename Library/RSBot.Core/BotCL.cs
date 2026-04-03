using RSBot.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSBot.Core
{
    public class BotCL
    {
        public static void Initialize(string profile, string silkroadDir)
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
