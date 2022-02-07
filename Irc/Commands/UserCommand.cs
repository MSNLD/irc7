using Irc.Enumerations;

namespace Irc.Commands;

internal class UserCommand : Command, ICommand
{
    public UserCommand()
    {
        _requiredMinimumParameters = 4;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new string GetName()
    {
        return "User";
    }

    public void Execute(ChatFrame chatFrame)
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
            chatFrame.User.GetAddress().Host = chatFrame.User.GetAddress().RemoteIP;
            chatFrame.User.GetAddress().Server = chatFrame.Message.Parameters[2];
            chatFrame.User.GetAddress().RealName = chatFrame.Message.Parameters[3];
        }
    }
}