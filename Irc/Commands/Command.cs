using Irc.Enumerations;

namespace Irc.Commands;

public class Command : ICommand
{
    private readonly bool _needsReg;
    protected int _maxParams;
    protected int _minParams;

    public Command(int minParams = 0, bool needsReg = true, int maxParams = -1)
    {
        _minParams = minParams;
        _needsReg = needsReg;
        _maxParams = maxParams;
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
        if (chatFrame.Message.Parameters.Count >= _minParams) return true;
        if (_maxParams < 0 || chatFrame.Message.Parameters.Count <= _maxParams) return true;

        chatFrame.User.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(chatFrame.Server, chatFrame.User, GetName()));
        return false;
    }

    public bool CheckRegister(ChatFrame chatFrame)
    {
        if (!_needsReg || (_needsReg && chatFrame.User.IsRegistered())) return true;

        chatFrame.User.Send(Raw.IRCX_ERR_NOTREGISTERED_451(chatFrame.Server, chatFrame.User));
        return false;
    }
}