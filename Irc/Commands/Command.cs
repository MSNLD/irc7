using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Commands;

public class Command : ICommand
{
    protected int _requiredMaximumParameters;
    protected int _requiredMinimumParameters;
    private readonly bool _registrationRequired;

    public Command(int requiredMinimumParameters = 0, bool registrationRequired = true, int requiredMaximumParameters = -1)
    {
        _requiredMinimumParameters = requiredMinimumParameters;
        _registrationRequired = registrationRequired;
        _requiredMaximumParameters = requiredMaximumParameters;
    }

    public string GetName()
    {
        return GetType().Name;
    }

    public EnumCommandDataType GetDataType()
    {
        throw new NotImplementedException();
    }

    public void Execute(ChatFrame chatFrame)
    {
        throw new NotImplementedException();
    }

    public bool ParametersAreValid(ChatFrame chatFrame)
    {
        var parameterCount = chatFrame.Message.Parameters.Count;

        if (
            parameterCount >= _requiredMinimumParameters &&
            (_requiredMaximumParameters < 0 || parameterCount <= _requiredMaximumParameters)
            ) return true;

        chatFrame.User.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(chatFrame.Server, chatFrame.User, GetName()));
        return false;
    }

    public bool RegistrationNeeded(ChatFrame chatFrame)
    {
        if (!_registrationRequired || (_registrationRequired && chatFrame.User.IsRegistered())) return false;

        chatFrame.User.Send(Raw.IRCX_ERR_NOTREGISTERED_451(chatFrame.Server, chatFrame.User));
        return true;
    }
}