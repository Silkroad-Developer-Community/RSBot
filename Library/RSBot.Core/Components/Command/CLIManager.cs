using System.Collections.Generic;

namespace RSBot.Core.Components.Command;

public static class CLIManager
{
    private static readonly Dictionary<string, ICLICommand> _commands = new();

    /// <summary>
    /// Registers the specified command.
    /// </summary>
    /// <param name="command">The command.</param>
    public static void Register(ICLICommand command)
    {
        _commands[command.Name.ToLowerInvariant()] = command;
    }

    /// <summary>
    /// Executes the specified command name.
    /// </summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="args">The arguments.</param>
    public static void Execute(string commandName, string[] args)
    {
        if (_commands.TryGetValue(commandName.ToLowerInvariant(), out var command))
        {
            command.Execute(args);
        }
        else
        {
            Log.Warn($"Unknown CLI command: {commandName}");
        }
    }
}
