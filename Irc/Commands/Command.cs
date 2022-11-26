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

    public bool CheckParameters(ChatFrame chatFrame)
    {
        if (chatFrame.Message.Parameters.Count >= _requiredMinimumParameters) return true;
        if (_requiredMaximumParameters < 0 || chatFrame.Message.Parameters.Count <= _requiredMaximumParameters) return true;

        chatFrame.User.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(chatFrame.Server, chatFrame.User, GetName()));
        return false;
    }

    public bool CheckRegister(ChatFrame chatFrame)
    {
        if (!_registrationRequired || (_registrationRequired && chatFrame.User.IsRegistered())) return true;

        chatFrame.User.Send(Raw.IRCX_ERR_NOTREGISTERED_451(chatFrame.Server, chatFrame.User));
        return false;
    }
}