using RSBot.Core;
using RSBot.Core.Plugins;

namespace RSBot.ServerInfo;

public class ServerInfoPlugin : IPlugin
{
    public string InternalName => "RSBot.ServerInfo";
    public void Initialize()
    {
        Log.Notify("[Server Information] Plugin initialized!");
    }
    public void OnLoadCharacter() { }
}
