using System;
using System.ComponentModel;
using System.Windows.Forms;
using RSBot.Core.Event;
using SDUI.Controls;

namespace RSBot.ServerInfo.Views;

[ToolboxItem(false)]
public partial class Main : DoubleBufferedControl
{
    public Main()
    {
        InitializeComponent();
        SubscribeEvents();
        UpdateServerInfo();
    }
    private void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnServerListUpdated", UpdateServerInfo);
    }
    private void UpdateServerInfo()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(UpdateServerInfo));
            return;
        }
        lvServerInfo.Items.Clear();

        var servers = ServerInfoManager.GetServers();

        if (servers == null)
            return;

        foreach (var server in servers)
        {
            var toInsert = new ListViewItem(new[] { server.Name, server.State });
            lvServerInfo.Items.Add(toInsert);
        }
    }
}
