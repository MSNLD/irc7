using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface ICommand
{
    EnumCommandDataType GetDataType();
    string GetName();
    void Execute(IChatFrame chatFrame);
    bool CheckParameters(IChatFrame chatFrame);
    bool CheckRegister(IChatFrame chatFrame);
}