using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Commands;

public class Command : ICommand
{
    private readonly bool _needsReg;
    protected readonly int MaxParams;
    protected int MinParams;

    public Command(int minParams = 0, bool needsReg = true, int maxParams = -1)
    {
        MinParams = minParams;
        _needsReg = needsReg;
        MaxParams = maxParams;
    }

    public string GetName()
    {
        return GetType().Name;
    }

    public EnumCommandDataType GetDataType()
    {
        throw new NotImplementedException();
    }

    public virtual void Execute(IChatFrame chatFrame)
    {
        throw new NotImplementedException();
    }

    public bool CheckParameters(IChatFrame chatFrame)
    {
        if (chatFrame.Message.Parameters.Count >= MinParams &&
             (MaxParams < 0 || chatFrame.Message.Parameters.Count <= MaxParams)
           ) return true;

        chatFrame.User.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(chatFrame.Server, chatFrame.User, GetName()));
        return false;
    }

    public bool CheckRegister(IChatFrame chatFrame)
    {
        if (!_needsReg || (_needsReg && chatFrame.User.IsRegistered())) return true;

        chatFrame.User.Send(Raw.IRCX_ERR_NOTREGISTERED_451(chatFrame.Server, chatFrame.User));
        return false;
    }
}