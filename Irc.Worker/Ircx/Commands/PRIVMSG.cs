using Irc.ClassExtensions.CSharpTools;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class PRIVMSG : Command
{
    public PRIVMSG(CommandCode Code) : base(Code)
    {
        MinParamCount = 2;
        RegistrationRequired = true;
        DataType = CommandDataType.Standard;
    }

    public static COM_RESULT ProcessPrivmsg(Frame Frame, Channel Channel, bool Privmsg)
    {
        // Need more logic around if the user CAN send to channel based on modes.
        var uci = Frame.User.GetChannelInfo(Channel);

        if (Flood.FloodCheck(CommandDataType.Standard, uci) == FLD_RESULT.S_WAIT) return COM_RESULT.COM_WAIT;

        var bCanSendToChannel = false;
        if (uci != null) // user is on channel
        {
            if (Channel.Modes.Moderated.Value == 0)
                bCanSendToChannel = true;
            else if (!uci.Member.ChannelMode.IsNormal()) bCanSendToChannel = true;
        }
        else if (Channel.Modes.NoExtern.Value == 0)
        {
            bCanSendToChannel = true;
        }

        if (bCanSendToChannel)
        {
            var Raw = Raws.RPL_PRIVMSG_CHAN;
            if (!Privmsg) Raw = Raws.RPL_NOTICE_CHAN;

            Channel.Send(Raws.Create(Frame.Server, Channel, Frame.User, Raw, new[] {Frame.Message.Data[1]}), Frame.User,
                true);
        }
        else
        {
            // you are not on that channel
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_CANNOTSENDTOCHAN_404,
                Data: new[] {Channel.Name}));
        }

        return COM_RESULT.COM_SUCCESS;
    }

    private static COM_RESULT ProcessServerPrivmsg(Frame Frame, bool Privmsg)
    {
        if (Flood.FloodCheck(CommandDataType.Standard, Frame.User) == FLD_RESULT.S_WAIT) return COM_RESULT.COM_WAIT;
        var Nicknames = Tools.CSVToArray(Frame.Message.Data[0]);
        if (Nicknames != null)
            for (var i = 0; i < Nicknames.Count; i++)
            {
                User TargetUser = null;
                var TargetNickname = new string(Nicknames[i].ToUpper());

                if (Frame.User.ActiveChannel != null)
                {
                    var c = Frame.User.ActiveChannel.Channel.Members.GetMemberByName(Nicknames[i]);
                    if (c != null) TargetUser = c.User;
                }

                if (TargetUser == null) TargetUser = Frame.Server.Users.GetUser(TargetNickname);

                if (TargetUser != null)
                    TargetUser.Send(Raws.Create(Frame.Server, Client: Frame.User,
                        Raw: Privmsg ? Raws.RPL_PRIVMSG_USER : Raws.RPL_NOTICE_USER,
                        Data: new[] {TargetUser.Name, Frame.Message.Data[1]}));
                else
                    Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {Resources.Null}));
            }
        else
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                Data: new[] {Resources.CommandWhois}));

        return COM_RESULT.COM_SUCCESS;
    }


    public static COM_RESULT ProcessMessage(Frame Frame, bool Privmsg)
    {
        if (Channel.IsChannel(Frame.Message.Data[0]))
        {
            var Channels = Frame.Server.Channels.GetChannels(Frame.Server, Frame.User, Frame.Message.Data[0], true);
            if (Channels != null)
                for (var c = 0; c < Channels.Count; c++)
                    ProcessPrivmsg(Frame, Channels[c], Privmsg);
            else
                Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                    Data: new[] {Resources.CommandWhois}));
        }
        else
        {
            return ProcessServerPrivmsg(Frame, Privmsg);
        }

        return COM_RESULT.COM_SUCCESS;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        return ProcessMessage(Frame, true);
    }
}