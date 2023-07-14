using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Commands;

public interface ICommand
{
    EnumCommandDataType GetDataType();
    string GetName();
    void Execute(IChatFrame chatFrame);
    bool ParametersAreValid(IChatFrame chatFrame);
    bool RegistrationNeeded(IChatFrame chatFrame);
}