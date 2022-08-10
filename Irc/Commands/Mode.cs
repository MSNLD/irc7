using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Objects.Server;

namespace Irc.Commands;

internal class Mode : Command, ICommand
{
    public Mode() : base(1) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        if (!chatFrame.User.IsRegistered())
        {
            if (chatFrame.Message.Parameters.First().ToUpper() == Resources.ISIRCX)
            {
                var protocol = chatFrame.User.GetProtocol().GetProtocolType();
                var isircx = protocol > EnumProtocolType.IRC;
                chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0,
                    chatFrame.Server.MaxInputBytes, Resources.IRCXOptions));
            }
        }
        else
        {
            // TODO: implement MODE
            var objectName = chatFrame.Message.Parameters.First();

            ChatObject chatObject = null;

            if (Channel.ValidName(objectName))
            {
                chatObject = (ChatObject)chatFrame.Server.GetChannelByName(objectName);
            }
            else
            {
                chatObject = (ChatObject)chatFrame.Server.GetUsers().FirstOrDefault(user => user.Name.ToUpperInvariant() == objectName.ToUpperInvariant());
            }

            if (chatObject == null)
            {
                // :sky-8a15b323126 403 Sky aaa :No such channel
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, objectName));
                return;
            }

            if (chatFrame.Message.Parameters.Count > 1)
            {
                // Perform mode operation
                Queue<string> modeParameters = null;
                if (chatFrame.Message.Parameters.Count > 2)
                {
                    modeParameters = new Queue<string>(chatFrame.Message.Parameters.Skip(2).ToArray());
                }
                ModeEngine.Breakdown(chatFrame.User, chatObject, chatFrame.Message.Parameters[1], modeParameters);
            }
            else
            {
                if (chatObject != null)
                {
                    /*-> sky-8a15b323126 MODE Sky
                        <- :sky-8a15b323126 221 Sky +ix
                        -> sky-8a15b323126 MODE #test
                        <- :sky-8a15b323126 324 Sky #test +tnl 50*/
                    if (chatObject is IChannel)
                    {
                        chatFrame.User.Send(Raw.IRCX_RPL_MODE_324(chatFrame.Server, chatFrame.User, ((IChannel)chatObject),
                            ((IChannel)chatObject).GetModes().ToString()));

                    }
                    else if (chatObject is IUser)
                    {
                        chatFrame.User.Send(Raw.IRCX_RPL_UMODEIS_221(chatFrame.Server, chatFrame.User, chatObject.GetModes().ToString()));
                    }

                }
            }
        }
    }
}