using System.Linq;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
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

    public static bool ProcessPrivmsg(Frame Frame, Channel targetChannel, bool Privmsg)
    {
        // Need more logic around if the user CAN send to channel based on modes.
        var channelMemberPair = Frame.User.Channels.FirstOrDefault(c => c.Key == targetChannel);
        var channel = channelMemberPair.Key;
        var channelMember = channelMemberPair.Value;

        if (Flood.FloodCheck(CommandDataType.Standard, channelMember.User) == FLD_RESULT.S_WAIT) return false;

        var bCanSendToChannel = false;
        if (channel != null) // user is on channel
        {
            if (targetChannel.Modes.Moderated.Value == 0)
                bCanSendToChannel = true;
            else if (!channelMember.ChannelMode.IsNormal()) bCanSendToChannel = true;
        }
        else if (targetChannel.Modes.NoExtern.Value == 0)
        {
            bCanSendToChannel = true;
        }

        if (bCanSendToChannel)
        {
            var Raw = Raws.RPL_PRIVMSG_CHAN;
            if (!Privmsg) Raw = Raws.RPL_NOTICE_CHAN;

            targetChannel.Send(RawBuilder.Create(Frame.Server, targetChannel, Frame.User, Raw, new[] {Frame.Message.Parameters[1]}), Frame.User,
                true);
        }
        else
        {
            // you are not on that channel
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_CANNOTSENDTOCHAN_404,
                Data: new[] {targetChannel.Name}));
        }

        return true;
    }

    private static bool ProcessServerPrivmsg(Frame Frame, bool Privmsg)
    {
        if (Flood.FloodCheck(CommandDataType.Standard, Frame.User) == FLD_RESULT.S_WAIT) return false;
        var Nicknames = Tools.CSVToArray(Frame.Message.Parameters[0]);
        if (Nicknames != null)
            for (var i = 0; i < Nicknames.Count; i++)
            {
                User TargetUser = null;
                var TargetNickname = new string(Nicknames[i].ToUpper());

                // TODO: Rewrite below after change ChannelMembersCollection to generic type
                if (Frame.User.Channels.Count > 0)
                {
                    foreach (var channel in Frame.User.Channels.Keys)
                    {
                        var member = channel.Members.FirstOrDefault(member => member.User.Name == Nicknames[i]);
                        if (member != null)
                        {
                            TargetUser = member.User;
                            break;
                        }
                    }
                }

                var objIdentifier = IrcHelper.IdentifyObject(TargetNickname);
                if (TargetUser == null) TargetUser = Frame.Server.Users.FindObj(TargetNickname, objIdentifier);

                if (TargetUser != null)
                    TargetUser.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                        Raw: Privmsg ? Raws.RPL_PRIVMSG_USER : Raws.RPL_NOTICE_USER,
                        Data: new[] {TargetUser.Name, Frame.Message.Parameters[1]}));
                else
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {string.Empty}));
            }
        else
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                Data: new[] {Resources.CommandWhois}));

        return true;
    }


    public static bool ProcessMessage(Frame Frame, bool Privmsg)
    {
        if (Channel.IsChannel(Frame.Message.Parameters[0]))
        {
            var Channels = Common.GetChannels(Frame.Server, Frame.User, Frame.Message.Parameters[0], true);
            if (Channels != null)
                for (var c = 0; c < Channels.Count; c++)
                    ProcessPrivmsg(Frame, Channels[c], Privmsg);
            else
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                    Data: new[] {Resources.CommandWhois}));
        }
        else
        {
            return ProcessServerPrivmsg(Frame, Privmsg);
        }

        return true;
    }

    public new bool Execute(Frame Frame)
    {
        return ProcessMessage(Frame, true);
    }
}