using System.Windows.Forms;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Plugins;
using RSBot.Quest.Views.Sidebar;

namespace RSBot.Quest;

public class QuestPlugin : IPlugin
{
    public string InternalName => "RSBot.QuestLog";
    public static QuestPlugin Instance { get; private set; }
    public QuestManager Manager { get; private set; }
    public void Initialize()
    {
        Instance = this;
        Manager = new QuestManager();
    }
    public void OnLoadCharacter() { }
}
