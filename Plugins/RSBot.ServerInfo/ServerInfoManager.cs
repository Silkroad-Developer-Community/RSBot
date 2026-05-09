using RSBot.General.Components;
using RSBot.General.Models;
using System.Collections.Generic;

namespace RSBot.ServerInfo
{
    public class ServerInfoManager
    {
        public static List<Server> GetServers()
        {
            return Serverlist.Servers;
        }
    }
}
