using System.Linq;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class WHISPER : Command
{
    public WHISPER(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        RegistrationRequired = true;
        DataType = CommandDataType.Standard;
    }

    public static void ProcessWhisper(Frame Frame, Channel channel, string TargetNicknames, string Message)
    {
        if (Frame.User.Modes.Gag.Value == 1) return;

        var Nicknames = Tools.CSVToArray(TargetNicknames);
        if (Nicknames != null)
            for (var c = 0; c < Nicknames.Count; c++)
            {
                var member = channel.Members.FirstOrDefault(member => member.User.Name == Nicknames[c]);
                if (member != null)
                {
                    if (member.User.Guest && channel.Modes.NoGuestWhisper.Value == 1)
                        // Guest Whispers not permitted
                        Frame.User.Send(RawBuilder.Create(Frame.Server, channel, Frame.User, Raws.IRCX_ERR_NOWHISPER_923));
                    else if (member.User.Level < UserAccessLevel.ChatHost && channel.Modes.NoWhisper.Value == 1)
                        // Whispers not permitted
                        Frame.User.Send(RawBuilder.Create(Frame.Server, channel, Frame.User, Raws.IRCX_ERR_NOWHISPER_923));
                    else
                        // Send Whisper
                        member.User.Send(RawBuilder.Create(Frame.Server, channel, Frame.User, Raws.RPL_CHAN_WHISPER,
                            new[] {member.User.Name, Message}));
                }
                else
                {
                    // Member not on channel
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401_N,
                        Data: new[] {Nicknames[c]}));
                }
            }
        else
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401_N,
                Data: new[] {string.Empty}));
    }

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters.Count == 1)
        {
            // No recipient given
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRC_ERR_NORECIPIENT_411,
                Data: new[] {Resources.CommandWhisper}));
        }
        else if (Frame.Message.Parameters.Count == 2)
        {
            // No text to send
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRC_ERR_NOTEXT_412,
                Data: new[] {Resources.CommandWhisper}));
        }
        else if (Frame.Message.Parameters.Count == 3)
        {
            if (Channel.IsChannel(Frame.Message.Parameters[0]))
            {
                var channelMemberPair = Frame.User.GetChannelInfo(Frame.Message.Parameters[0]);
                var channel = channelMemberPair.Key;
                var channelMember = channelMemberPair.Value;
                if (channel != null)
                {
                    if (Flood.FloodCheck(DataType, channelMember.User) == FLD_RESULT.S_WAIT) return false;

                    if (Frame.User.Guest && channel.Modes.NoGuestWhisper.Value == 1)
                        Frame.User.Send(RawBuilder.Create(Frame.Server, channel, Frame.User,
                            Raws.IRCX_ERR_NOWHISPER_923));
                    else // Frame.User has channel in channel collection
                        ProcessWhisper(Frame, channel, Frame.Message.Parameters[1], Frame.Message.Parameters[2]);
                }
                else
                {
                    // if channel exists, Frame.User is not on that channel
                    var objType = IrcHelper.IdentifyObject(Frame.Message.Parameters[0]);
                    var c = Frame.Server.Channels.FindObj(Frame.Message.Parameters[0], objType);
                    if (c != null)
                        // You are not on that channel
                        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new[] {Frame.Message.Parameters[0]}));
                    else
                        // No such channel
                        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new[] {Frame.Message.Parameters[0]}));
                }
            }
            else
            {
                // No such channel
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                    Data: new[] {Frame.Message.Parameters[0]}));
            }
        }
        else
        {
            // Too many arguments
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_TOOMANYARGUMENTS_901,
                Data: new[] {Frame.Message.Parameters[2]}));
        }

        return true;
    }
}