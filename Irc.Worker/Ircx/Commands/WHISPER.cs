using Irc.ClassExtensions.CSharpTools;
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

    public static void ProcessWhisper(Frame Frame, Channel Channel, string TargetNicknames, string Message)
    {
        if (Frame.User.Modes.Gag.Value == 1) return;

        var Nicknames = Tools.CSVToArray(TargetNicknames);
        if (Nicknames != null)
            for (var c = 0; c < Nicknames.Count; c++)
            {
                var member = Channel.Members.GetMemberByName(Nicknames[c]);
                if (member != null)
                {
                    if (member.User.Guest && Channel.Modes.NoGuestWhisper.Value == 1)
                        // Guest Whispers not permitted
                        Frame.User.Send(Raws.Create(Frame.Server, Channel, Frame.User, Raws.IRCX_ERR_NOWHISPER_923));
                    else if (member.User.Level < UserAccessLevel.ChatHost && Channel.Modes.NoWhisper.Value == 1)
                        // Whispers not permitted
                        Frame.User.Send(Raws.Create(Frame.Server, Channel, Frame.User, Raws.IRCX_ERR_NOWHISPER_923));
                    else
                        // Send Whisper
                        member.User.Send(Raws.Create(Frame.Server, Channel, Frame.User, Raws.RPL_CHAN_WHISPER,
                            new[] {member.User.Name, Message}));
                }
                else
                {
                    // Member not on channel
                    Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401_N,
                        Data: new[] {Nicknames[c]}));
                }
            }
        else
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401_N,
                Data: new[] {Resources.Null}));
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        if (Frame.Message.Data.Count == 1)
        {
            // No recipient given
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRC_ERR_NORECIPIENT_411,
                Data: new[] {Resources.CommandWhisper}));
        }
        else if (Frame.Message.Data.Count == 2)
        {
            // No text to send
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRC_ERR_NOTEXT_412,
                Data: new[] {Resources.CommandWhisper}));
        }
        else if (Frame.Message.Data.Count == 3)
        {
            if (Channel.IsChannel(Frame.Message.Data[0]))
            {
                var uci = Frame.User.GetChannelInfo(Frame.Message.Data[0]);
                if (uci != null)
                {
                    if (Flood.FloodCheck(DataType, uci) == FLD_RESULT.S_WAIT) return COM_RESULT.COM_WAIT;

                    if (Frame.User.Guest && uci.Channel.Modes.NoGuestWhisper.Value == 1)
                        Frame.User.Send(Raws.Create(Frame.Server, uci.Channel, Frame.User,
                            Raws.IRCX_ERR_NOWHISPER_923));
                    else // Frame.User has channel in channel collection
                        ProcessWhisper(Frame, uci.Channel, Frame.Message.Data[1], Frame.Message.Data[2]);
                }
                else
                {
                    // if channel exists, Frame.User is not on that channel
                    var c = Frame.Server.Channels.GetChannel(Frame.Message.Data[0]);
                    if (c != null)
                        // You are not on that channel
                        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new[] {Frame.Message.Data[0]}));
                    else
                        // No such channel
                        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new[] {Frame.Message.Data[0]}));
                }
            }
            else
            {
                // No such channel
                Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                    Data: new[] {Frame.Message.Data[0]}));
            }
        }
        else
        {
            // Too many arguments
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_TOOMANYARGUMENTS_901,
                Data: new[] {Frame.Message.Data[2]}));
        }

        return COM_RESULT.COM_SUCCESS;
    }
}