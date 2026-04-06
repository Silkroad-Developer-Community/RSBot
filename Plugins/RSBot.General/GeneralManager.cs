using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.General.Components;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            EventManager.SubscribeEvent("OnAgentServerConnected", OnAgentServerConnected);
            EventManager.SubscribeEvent("OnAgentServerDisconnected", OnAgentServerDisconnected);
            EventManager.SubscribeEvent("OnGatewayServerDisconnected", OnGatewayServerDisconnected);
            EventManager.SubscribeEvent("OnEnterGame", OnEnterGame);
            EventManager.SubscribeEvent("OnExitClient", OnExitClient);
            EventManager.SubscribeEvent("OnProfileChanged", OnProfileChanged);
        }
        private static async Task<bool> HandleRegionalAuth()
        {
            if (Game.ClientType == GameClientType.RuSro)
                return await RuSroAuthService.Auth();
            else if (Game.ClientType == GameClientType.Japanese)
                return await JSROAuthService.GetTokenAsync();
            return true;
        }
        private void OnAgentServerConnected()
        {
            Interlocked.Increment(ref _reloginSeq);
        }
        private async void OnAgentServerDisconnected()
        {
            Kernel.Bot.Stop();

            // Skiped: Cuz managing from ClientlessManager
            if (Game.Clientless)
                return;

            // If user disconnected with manual from clientless, we dont need open the client automatically again.
            //if (!Kernel.Proxy.ClientConnected)
            //return;

            ClientManager.Kill();

            if (GlobalConfig.Get<bool>("RSBot.General.EnableAutomatedLogin"))
            {
                var reloginSeq = Interlocked.Increment(ref _reloginSeq);

                EventManager.FireEvent("OnBtnStartClientChanged", false);
                EventManager.FireEvent("OnBtnStartClientlessChanged", false);

                int delay = 10000;
                if (GlobalConfig.Get("RSBot.General.EnableWaitAfterDC", false))
                    delay = GlobalConfig.Get<int>("RSBot.General.WaitAfterDC") * 60 * 1000;

                Log.Warn($"Attempting relogin in {delay / 1000} seconds...");
                await Task.Delay(delay);

                if (reloginSeq != Volatile.Read(ref _reloginSeq))
                    return;

                var userAuthenticated = await HandleRegionalAuth();
                if (!userAuthenticated)
                {
                    Log.Warn("Regional auth failed!");
                    return;
                }

                await StartClientProcess();
                return;
            }
            EventManager.FireEvent("OnBtnGoClientlessChanged", false);
            EventManager.FireEvent("OnBtnStartClientChanged", true);
            EventManager.FireEvent("OnBtnStartClientlessChanged", true);
        }
        private async void OnGatewayServerDisconnected()
        {
            AutoLogin.Pending = false;
            EventManager.FireEvent("OnAutoLoginAborted");

            var wasClientless = Game.Clientless;

            if (!Kernel.Proxy.IsConnectedToAgentserver && !Kernel.Proxy.IsSwitchingToAgentserver)
            {
                if (GlobalConfig.Get<bool>("RSBot.General.EnableAutomatedLogin"))
                {
                    var reloginSeq = Interlocked.Increment(ref _reloginSeq);

                    EventManager.FireEvent("OnBtnStartClientChanged", false);
                    EventManager.FireEvent("OnBtnStartClientlessChanged", false);

                    // Gateway disconnect can happen briefly during Gateway -> Agent switch (or the switch can start late
                    // due to thread scheduling). Give it a short grace period before we decide it's a real DC.
                    await Task.Delay(2000);

                    if (reloginSeq != Volatile.Read(ref _reloginSeq))
                        return;

                    if (Kernel.Proxy.IsConnectedToAgentserver || Kernel.Proxy.IsSwitchingToAgentserver)
                        return;

                    EventManager.FireEvent("OnBtnStartClientChanged", true);
                    EventManager.FireEvent("OnBtnStartClientlessChanged", true);
                    EventManager.FireEvent("OnBtnStartClientTextChanged", LanguageManager.GetLang("Start") + " Clientless");
                    Log.StatusLang("Ready");
                    Kernel.Proxy.Shutdown();

                    int delay = 10000;
                    if (GlobalConfig.Get("RSBot.General.EnableWaitAfterDC", false))
                        delay = GlobalConfig.Get<int>("RSBot.General.WaitAfterDC") * 60 * 1000;

                    Log.Warn($"Attempting relogin in {delay / 1000} seconds...");
                    await Task.Delay(delay);

                    // Prevent double-start if the disconnect event fires multiple times.
                    if (reloginSeq != Volatile.Read(ref _reloginSeq))
                        return;

                    var userAuthenticated = await HandleRegionalAuth();
                    if (!userAuthenticated)
                    {
                        Log.Warn("Regional auth failed!");
                        return;
                    }

                    if (wasClientless)
                    {
                        // Clientless relogin: restart proxy flow only.
                        Game.Clientless = true;
                        Game.Start();
                    }
                    else
                    {
                        // Client relogin: restart client process.
                        ClientManager.Kill();
                        await StartClientProcess();
                    }

                    return;
                }

                EventManager.FireEvent("OnBtnStartClientChanged", true);
                EventManager.FireEvent("OnBtnStartClientlessChanged", true);
                EventManager.FireEvent("OnBtnStartClientTextChanged", LanguageManager.GetLang("Start") + " Clientless");

                Log.StatusLang("Ready");
                Kernel.Proxy.Shutdown();

                Game.Clientless = false;
            }
        }
        private async void OnEnterGame()
        {
            if (!Game.Clientless)
            {
                EventManager.FireEvent("OnBtnClientHideShowChanged", true);
                EventManager.FireEvent("OnBtnClientHideShowTextChanged", LanguageManager.GetLang("Hide") + " Client");
                EventManager.FireEvent("OnBtnStartClientChanged", true);
                EventManager.FireEvent("OnBtnStartClientTextChanged", LanguageManager.GetLang("Kill") + " Client");
                EventManager.FireEvent("OnBtnGoClientlessChanged", true);
            }

            while (!Game.Ready)
                await Task.Delay(100);

            var startBot = GlobalConfig.Get<bool>("RSBot.General.StartBot");
            var useReturnScroll = GlobalConfig.Get<bool>("RSBot.General.UseReturnScroll");

            if (useReturnScroll)
                Game.Player.UseReturnScroll();

            if (startBot)
                Kernel.Bot.Start();
        }
        private void OnExitClient()
        {
            Log.StatusLang("Ready");
            _clientVisible = false;
            EventManager.FireEvent("OnBtnStartClientTextChanged", LanguageManager.GetLang("Start") + " Client");

            if (Game.Clientless)
                return;

            EventManager.FireEvent("OnBtnStartClientChanged", true);
            EventManager.FireEvent("OnBtnStartClientlessChanged", true);
            EventManager.FireEvent("OnBtnClientHideShowChanged", false);

            if (!GlobalConfig.Get<bool>("RSBot.General.StayConnected"))
            {
                Kernel.Proxy.Shutdown();
            }
            else
            {
                if (!Kernel.Proxy.IsConnectedToAgentserver)
                    return;

                EventManager.FireEvent("OnBtnStartClientChanged", false);

                ClientlessManager.GoClientless();

                EventManager.FireEvent("OnBtnGoClientlessChanged", false);
                EventManager.FireEvent("OnBtnStartClientlessTextChanged", LanguageManager.GetLang("Disconnect"));

                Log.NotifyLang("ClientlessModeActivated");
            }
        }        
        private void OnProfileChanged()
        {
            Accounts.Load();
        }
        private async Task StartClientProcess()
        {
            EventManager.FireEvent("OnBtnStartClientChanged", false);
            Game.Start();

            await Task.Run(async () =>
            {
                var startedResult = await ClientManager.Start();
                if (!startedResult)
                {
                    OnExitClient();
                    Log.WarnLang("ClientStartingError");
                }
            });
        }
        public static void BrowseSilkroadPath()
        {
            using (var dialog = new OpenFileDialog())
            {
                var title = LanguageManager.GetLang("BrowseSilkroadPathDialogTitle");

                var msgBoxTitle = LanguageManager.GetLang("BrowseSilkroadPathMsgBoxTitle");
                var msgBoxContent = LanguageManager.GetLang("BrowseSilkroadPathMsgBoxContent");

                dialog.Title = title;
                dialog.Filter = "App (*.exe)|*.exe";
                dialog.FileName = "sro_client.exe";

                var result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                    return;

                EventManager.FireEvent("OnTxtSilkroadPathChanged", dialog.FileName);
                GlobalConfig.Set("RSBot.SilkroadDirectory", Path.GetDirectoryName(dialog.FileName));
                GlobalConfig.Set("RSBot.SilkroadExecutable", Path.GetFileName(dialog.FileName));

                result = MessageBox.Show(msgBoxContent, msgBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;

                GlobalConfig.Save();
            }

            Process.Start(Application.ExecutablePath);
            Application.Exit();
        }
        public static void GoClientless()
        {
            if (Game.Clientless)
                return;

            var msgBoxTitle = LanguageManager.GetLang("GoClientlessMsgBoxTitle");
            var msgBoxContent = LanguageManager.GetLang("GoClientlessMsgBoxContent");

            if (
                MessageBox.Show(msgBoxContent, msgBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes
            )
                return;

            ClientlessManager.GoClientless();
            ClientManager.Kill();

            EventManager.FireEvent("OnBtnStartClientlessTextChanged", LanguageManager.GetLang("Disconnect"));
            EventManager.FireEvent("OnBtnGoClientlessChanged", false);
            EventManager.FireEvent("OnBtnStartClientChanged", true);
            EventManager.FireEvent("OnBtnStartClientlessChanged", false);
            EventManager.FireEvent("OnBtnClientHideShowChanged", false);
        }
        public static async Task StartClientlessAsync()
        {
            await Task.Run(async () =>
            {
                if (!Game.Clientless)
                {                    
                    Game.Clientless = true;
                    Log.StatusLang("StartingClientless");
                    EventManager.FireEvent("OnBtnStartClientlessTextChanged", LanguageManager.GetLang("Disconnect"));

                    var userAuthenticated = await HandleRegionalAuth();

                    if (userAuthenticated)
                    {
                        Game.Start();
                    }
                }
            });
        }
            
        public static async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                Game.Clientless = false;

                EventManager.FireEvent("OnBtnStartClientChanged", true);
                EventManager.FireEvent("OnBtnStartClientlessChanged", true);
                EventManager.FireEvent("OnBtnStartClientlessTextChanged", LanguageManager.GetLang("Start") + " Clientless");
                
                Kernel.Proxy.Shutdown();                
            });
        }
        public async Task StartClientAsync()
        {
            var userAuthenticated = await HandleRegionalAuth();

            if (userAuthenticated)
            {
                await StartClientProcess();
            }
        }
        public static void KillClient()
        {
            if (!Game.Clientless && Kernel.Proxy != null && Kernel.Proxy.IsConnectedToAgentserver)
            {
                var extraStr = LanguageManager.GetLang("KillClientWarnMsgBoxSplit1");
                if (!GlobalConfig.Get<bool>("RSBot.General.StayConnected"))
                    extraStr = LanguageManager.GetLang("KillClientWarnMsgBoxSplit2");

                var title = LanguageManager.GetLang("Warning");
                var content = LanguageManager.GetLang("KillClientWarnMsgBoxContent", extraStr);

                if (MessageBox.Show(content, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    ClientManager.Kill();

                return;
            }
        }
    }
}
