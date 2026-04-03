using RSBot.Core;
using RSBot.Core.Client;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.General.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSBot.General
{
    public class GeneralManager
    {
        private bool _clientVisible;
        private static int _reloginSeq;
        public GeneralManager()
        {
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            ClientlessManager.RegionalAuthHandler = HandleRegionalAuth;

            EventManager.SubscribeEvent("OnLoadVersionInfo", new Action<VersionInfo>(OnLoadVersionInfo));
            //EventManager.SubscribeEvent("OnAgentServerConnected", OnAgentServerConnected);
            //EventManager.SubscribeEvent("OnAgentServerDisconnected", OnAgentServerDisconnected);
            //EventManager.SubscribeEvent("OnGatewayServerDisconnected", OnGatewayServerDisconnected);
            //EventManager.SubscribeEvent("OnClientConnected", OnClientConnected);
            //EventManager.SubscribeEvent("OnEnterGame", OnEnterGame);
            //EventManager.SubscribeEvent("OnStartClient", OnStartClient);
            //EventManager.SubscribeEvent("OnExitClient", OnExitClient);
            //EventManager.SubscribeEvent("OnCharacterListReceived", OnCharacterListReceived);
            //EventManager.SubscribeEvent("OnInitialized", OnInitialized);
            //EventManager.SubscribeEvent("OnProfileChanged", OnProfileChanged);
        }
        private static async Task<bool> HandleRegionalAuth()
        {
            if (Game.ClientType == GameClientType.RuSro)
                return await RuSroAuthService.Auth();
            else if (Game.ClientType == GameClientType.Japanese)
                return await JSROAuthService.GetTokenAsync();
            return true;
        }
        private void OnLoadVersionInfo(VersionInfo info)
        {
            //lblVersion.Text = "v" + ((1000f + info.Version) / 1000f).ToString("0.000", CultureInfo.InvariantCulture);
        }
        private void OnAgentServerConnected()
        {
            // Cancel any pending relogin timers once we successfully connected.
            Interlocked.Increment(ref _reloginSeq);
        }

    }
}
