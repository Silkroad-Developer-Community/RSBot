using RSBot.Core.Components.Command;
using System;

namespace RSBot.Core.Components.Command;

public class StartBotCLICommand : ICLICommand
{
    public string Name => "start";
    public string Description => "Starts the bot.";

    public void Execute(string[] args)
    {
        Kernel.Bot?.Start();
        Log.Notify("Bot started");
    }
}

public class StopBotCLICommand : ICLICommand
{
    public string Name => "stop";
    public string Description => "Stops the bot.";

    public void Execute(string[] args)
    {
        Kernel.Bot?.Stop();
        Log.Notify("Bot stopped");
    }
}

public class StatusCLICommand : ICLICommand
{
    public string Name => "status";
    public string Description => "Shows the bot status.";

    public void Execute(string[] args)
    {
        Console.WriteLine($"Character: {Game.Player?.Name ?? "Not logged in"}");
        Console.WriteLine($"Bot running: {Kernel.Bot?.Running ?? false}");
    }
}
