using RSBot.Core;
using RSBot.Core.Components.Command;
using RSBot.Core.Objects;
using System;

namespace RSBot.Chat;

public class ChatCLICommand : ICLICommand
{
    public string Name => "chat";
    public string Description => "Sends a chat message. Format: chat,type,message,[receiver]";

    public void Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Log.Warn("Usage: chat,type,message,[receiver]");
            return;
        }

        if (!Enum.TryParse<ChatType>(args[0], true, out var type))
        {
            Log.Warn($"Invalid chat type: {args[0]}");
            return;
        }

        var message = args[1];
        var receiver = args.Length > 2 ? args[2] : null;

        ChatManager.Send(type, message, receiver);
        Log.Notify($"Message ({type}) sent: {message}");
    }
}
