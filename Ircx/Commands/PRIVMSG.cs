using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    public class PRIVMSG : Command
    {
        public PRIVMSG(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 2;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Standard;
        }

        public static COM_RESULT ProcessPrivmsg(Frame Frame, Channel Channel, bool Privmsg)
        {
            // Need more logic around if the user CAN send to channel based on modes.
            UserChannelInfo uci = Frame.User.GetChannelInfo(Channel);

            if (Flood.FloodCheck(CommandDataType.Standard, uci) == FLD_RESULT.S_WAIT) { return COM_RESULT.COM_WAIT; }

            bool bCanSendToChannel = false;
            if (uci != null) // user is on channel
            {
                if (Channel.Modes.Moderated.Value == 0) { bCanSendToChannel = true; }
                else if (!uci.Member.ChannelMode.IsNormal()) { bCanSendToChannel = true; }
            }
            else if (Channel.Modes.NoExtern.Value == 0) { bCanSendToChannel = true; }

            if (bCanSendToChannel)
            {
                string Raw = Raws.RPL_PRIVMSG_CHAN;
                if (!Privmsg) { Raw = Raws.RPL_NOTICE_CHAN; }

                Channel.Send(Raws.Create(Server: Frame.Server, Channel: Channel, Client: Frame.User, Raw: Raw, Data: new string[] { Frame.Message.Data[1] }), Frame.User, true);
            }
            else
            {
                // you are not on that channel
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_CANNOTSENDTOCHAN_404, Data: new string[] { Channel.Name }));
            }
            return COM_RESULT.COM_SUCCESS;
        }

        private static COM_RESULT ProcessServerPrivmsg(Frame Frame, bool Privmsg)
        {
            if (Flood.FloodCheck(CommandDataType.Standard, Frame.User) == FLD_RESULT.S_WAIT) { return COM_RESULT.COM_WAIT; }
            List<string> Nicknames = CSharpTools.Tools.CSVToArray(Frame.Message.Data[0]);
            if (Nicknames != null)
            {

                for (int i = 0; i < Nicknames.Count; i++)
                {
                    User TargetUser = null;
                    string TargetNickname = new string(Nicknames[i].ToString().ToUpper());

                    if (Frame.User.ActiveChannel != null)
                    {
                        ChannelMember c = Frame.User.ActiveChannel.Channel.Members.GetMemberByName(Nicknames[i]);
                        if (c != null) { TargetUser = c.User; }
                    }
                    if (TargetUser == null)
                    {
                        TargetUser = Frame.Server.Users.GetUser(TargetNickname);
                    }

                    if (TargetUser != null) {
                        TargetUser.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: (Privmsg == true ? Raws.RPL_PRIVMSG_USER : Raws.RPL_NOTICE_USER), Data: new string[] { TargetUser.Name, Frame.Message.Data[1] }));
                    }
                    else
                    {
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new string[] { Resources.Null }));
                    }
                }

            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { Resources.CommandWhois }));
            }
            return COM_RESULT.COM_SUCCESS;
        }


        public static COM_RESULT ProcessMessage(Frame Frame, bool Privmsg)
        {
            if (Channel.IsChannel(Frame.Message.Data[0])) { 
                List<Channel> Channels = Frame.Server.Channels.GetChannels(Frame.Server, Frame.User, Frame.Message.Data[0], true);
                if (Channels != null)
                {
                    for (int c = 0; c < Channels.Count; c++)
                    {
                        return ProcessPrivmsg(Frame, Channels[c], Privmsg);
                    }
                }
                else
                {
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { Resources.CommandWhois }));
                }
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
}
