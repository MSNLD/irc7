using Irc.Enumerations;

namespace Irc.Commands;

public interface ICommand
{
    EnumCommandDataType GetDataType();
    string GetName();
    void Execute(ChatFrame chatFrame);
    bool CheckParameters(ChatFrame chatFrame);
}