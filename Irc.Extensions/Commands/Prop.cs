using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Commands;

public class Prop : Command, ICommand
{
    public Prop() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        //chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
        chatFrame.User.Name = "Sky";
        chatFrame.User.GetAddress().Nickname = "Sky";
        chatFrame.User.GetAddress().User = "A65F0CE7D05F0B4E";
        chatFrame.User.GetAddress().Host = "GateKeeperPassport";
        chatFrame.User.GetAddress().RealName = "Sky";
        chatFrame.User.GetAddress().Server = "SERVER";
        //chatFrame.User.Register();
        Register.Execute(chatFrame);
    }
}