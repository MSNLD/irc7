using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Commands;

public class UserCommand : Command, ICommand
{
    public UserCommand() : base(4, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new string GetName()
    {
        return "User";
    }

    public new void Execute(IChatFrame chatFrame)
    {
        if (chatFrame.User.IsRegistered())
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
        }
        else
        {
            // Gotta check each param
            chatFrame.User.GetAddress().User = chatFrame.Message.Parameters[0];
            //chatFrame.User.Address.Host = chatFrame.Message.Parameters[1];
            chatFrame.User.GetAddress().Host = chatFrame.User.GetAddress().RemoteIp;
            chatFrame.User.GetAddress().Server = chatFrame.Message.Parameters[2];
            chatFrame.User.GetAddress().RealName = chatFrame.Message.Parameters[3];
        }
    }
}