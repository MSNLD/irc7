using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class WHISPER : Command
    {

        public WHISPER(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Standard;
        }

        public static void ProcessWhisper(Frame Frame, Channel Channel, String8 TargetNicknames, String8 Message)
        {
            if (Frame.User.Modes.Gag.Value == 1) { return; }

            List<String8> Nicknames = CSharpTools.Tools.CSVToArray(TargetNicknames);
            if (Nicknames != null)
            {
                for (int c = 0; c < Nicknames.Count; c++)
                {
                    ChannelMember member = Channel.Members.GetMemberByName(Nicknames[c]);
                    if (member != null)
                    {
                        if ((member.User.Guest) && (Channel.Modes.NoGuestWhisper.Value == 1))
                        {
                            // Guest Whispers not permitted
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Channel: Channel, Client: Frame.User, Raw: Raws.IRCX_ERR_NOWHISPER_923));
                        }
                        else if ((member.User.Level < UserAccessLevel.ChatHost) && (Channel.Modes.NoWhisper.Value == 1))
                        {
                            // Whispers not permitted
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Channel: Channel, Client: Frame.User, Raw: Raws.IRCX_ERR_NOWHISPER_923));
                        }
                        else
                        {
                            // Send Whisper
                            member.User.Send(Raws.Create(Server: Frame.Server, Channel: Channel, Client: Frame.User, Raw: Raws.RPL_CHAN_WHISPER, Data: new String8[] { member.User.Name, Message }));
                        }
                    }
                    else
                    {
                        // Member not on channel
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401_N, Data: new String8[] { Nicknames[c] }));
                    }
                }
            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401_N, Data: new String8[] { Resources.Null }));
            }
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data.Count == 1)
            {
                // No recipient given
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRC_ERR_NORECIPIENT_411, Data: new String8[] { Resources.CommandWhisper }));
            }
            else if (Frame.Message.Data.Count == 2)
            {
                // No text to send
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRC_ERR_NOTEXT_412, Data: new String8[] { Resources.CommandWhisper }));
            }
            else if (Frame.Message.Data.Count == 3)
            {
                if (Channel.IsChannel(Frame.Message.Data[0]))
                {
                    UserChannelInfo uci = Frame.User.GetChannelInfo(Frame.Message.Data[0]);
                    if (uci != null)
                    {
                        if (Flood.FloodCheck(base.DataType, uci) == FLD_RESULT.S_WAIT) { return COM_RESULT.COM_WAIT; }

                        if ((Frame.User.Guest) && (uci.Channel.Modes.NoGuestWhisper.Value == 1))
                        {
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Channel: uci.Channel, Client: Frame.User, Raw: Raws.IRCX_ERR_NOWHISPER_923));
                        }
                        else { 
                            // Frame.User has channel in channel collection
                            ProcessWhisper(Frame, uci.Channel, Frame.Message.Data[1], Frame.Message.Data[2]);
                        }
                    }
                    else
                    {
                        // if channel exists, Frame.User is not on that channel
                        Channel c = Frame.Server.Channels.GetChannel(Frame.Message.Data[0]);
                        if (c != null)
                        {
                            // You are not on that channel
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new String8[] { Frame.Message.Data[0] }));
                        }
                        else
                        {
                            // No such channel
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new String8[] { Frame.Message.Data[0] }));
                        }
                    }


                }
                else
                {
                    // No such channel
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new String8[] { Frame.Message.Data[0] }));
                }

            }
            else
            {
                // Too many arguments
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_TOOMANYARGUMENTS_901, Data: new String8[] { Frame.Message.Data[2] }));
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
