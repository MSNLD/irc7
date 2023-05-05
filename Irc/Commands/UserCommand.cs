using Irc.Enumerations;

namespace Irc.Commands;

public class UserCommand : Command, ICommand
{
    public UserCommand() : base(4, false) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new string GetName()
    {
        return "User";
    }

    public new void Execute(ChatFrame chatFrame)
    {
        if (chatFrame.User.IsRegistered())
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
        }
        else
        {
            // Gotta check each param
            //chatFrame.User.GetAddress().User = chatFrame.Message.Parameters[0];
            //chatFrame.User.Address.Host = chatFrame.Message.Parameters[1];
            var address = chatFrame.User.GetAddress();
            address.User = address.MaskedIP;
            address.Host = "anon";
            address.Server = chatFrame.Message.Parameters[2];
            address.RealName = chatFrame.Message.Parameters[3];
        }
    }
}