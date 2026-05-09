using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Components.Command;
using System;

namespace RSBot.General;

public class StartClientCommand : ICLICommand
{
    public string Name => "start-client";
    public string Description => "Starts the Silkroad client.";

    public void Execute(string[] args)
    {
        GeneralPlugin.Instance.Manager.StartClientAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Log.Error($"Failed to start client: {task.Exception?.InnerException?.Message ?? task.Exception?.Message}");
            else
                Log.Notify("Client started");
        });
    }
}

public class ShowClientCommand : ICLICommand
{
    public string Name => "show";
    public string Description => "Shows the Silkroad client.";

    public void Execute(string[] args)
    {
        ClientManager.SetVisible(true);
    }
}

public class HideClientCommand : ICLICommand
{
    public string Name => "hide";
    public string Description => "Hides the Silkroad client.";

    public void Execute(string[] args)
    {
        ClientManager.SetVisible(false);
    }
}
