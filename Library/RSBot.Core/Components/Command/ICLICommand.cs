namespace RSBot.Core.Components.Command;

public interface ICLICommand
{
    string Name { get; }
    string Description { get; }
    void Execute(string[] args);
}
