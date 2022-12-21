namespace Irc.Interfaces;

public interface IMessage
{
    List<string> Parameters { get; }
    string OriginalText { get; }
    string GetPrefix { get; }
    ICommand GetCommand();
    string GetCommandName();
    List<string> GetParameters();
}