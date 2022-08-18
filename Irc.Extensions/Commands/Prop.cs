using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;
using Irc.Objects.Channel;

namespace Irc.Extensions.Commands;

public class Prop : Command, ICommand
{
    public Prop() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        //chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
        // Passport hack
        //chatFrame.User.Name = "Sky";
        //chatFrame.User.GetAddress().Nickname = "Sky";
        //chatFrame.User.GetAddress().User = "A65F0CE7D05F0B4E";
        //chatFrame.User.GetAddress().Host = "GateKeeperPassport";
        //chatFrame.User.GetAddress().RealName = "Sky";
        //chatFrame.User.GetAddress().Server = "SERVER";
        //chatFrame.User.Register();
        // Register.Execute(chatFrame);

        if (!chatFrame.User.IsRegistered())
        {
            // Prop $ NICK
        }
        else
        {
            var objectName = chatFrame.Message.Parameters.First();

            ChatObject chatObject = null;

            // Lookup object
            if (Channel.ValidName(objectName))
            {
                chatObject = (ChatObject)chatFrame.Server.GetChannelByName(objectName);
            }
            else if (objectName.Equals(chatFrame.User.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                chatObject = (ChatObject)chatFrame.User;
            }
            // <$> The $ value is used to indicate the user that originated the request.
            else if (objectName == "$")
            {
                chatObject = (ChatObject)chatFrame.User;
            }
            {
                chatObject = (ChatObject)chatFrame.Server.GetUsers().FirstOrDefault(user => user.Name.ToUpperInvariant() == objectName.ToUpperInvariant());
            }
        }
    }
}