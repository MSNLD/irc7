using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class WHOIS : Command
    {
        // Improvement notes
        // When invisible you can WHOIS yourself
        // When invisible, another person who WHOIS you gets a end of whois reply only
        // When user does not exist, a no suck nick/channel is displayed, after which an end of WHOIS is sent
        public WHOIS(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.None;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Message message = Frame.Message;
            User user = Frame.User;
            Server server = Frame.Server;

            if (message.Data.Count >= 1)
            {

                List<String8> Nicknames = CSharpTools.Tools.CSVToArray(message.Data[0]);
                if (Nicknames != null)
                {

                    for (int i = 0; i < Nicknames.Count; i++)
                    {
                        User TargetUser = null;
                        String8 TargetNickname = new String8(Nicknames[i].bytes);
                        TargetNickname.toupper();

                        if (user.ChannelList.Count > 0) {
                            for (int x = 0; x < user.ChannelList.Count; x++) { 
                                ChannelMember c = user.ChannelList[x].Channel.Members.GetMemberByName(Nicknames[i]);
                                if (c != null) { TargetUser = c.User; }
                            }
                        }
                        if (TargetUser == null)
                        {
                            
                            TargetUser = server.Users.GetUser(TargetNickname);
                            if (TargetUser != null)
                            {
                                if ((TargetUser.Modes.Invisible.Value == 0x1) && (user.Level <= UserAccessLevel.ChatGuide)) { TargetUser = null; }
                            }
                        }

                        if (TargetUser != null)
                        {
                            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WHOISUSER_311, Data: new String8[] { TargetUser.Address.Nickname, TargetUser.Address.Userhost, TargetUser.Address.Hostname, TargetUser.Address.RealName }));

                            if (TargetUser.Channels.ChannelList.Count > 0)
                            {
                                String8 OutputRaw = new String8(512);
                                String8 WHOIS_319_RAW = Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WHOISCHANNELS_319X, Data: new String8[] { TargetUser.Address.Nickname }, Newline: false);
                                OutputRaw.append(WHOIS_319_RAW);

                                for (int c = 0; c < TargetUser.ChannelList.Count; c++)
                                {
                                    bool HasMode = (TargetUser.ChannelList[c].Member.ChannelMode.UserMode > ChanUserMode.Normal ? true : false);
                                    if (OutputRaw.Length + TargetUser.ChannelList[c].Channel.Name.Length + (HasMode ? 1 : 0) < 510)
                                    {
                                        if (HasMode) { OutputRaw.append(TargetUser.ChannelList[c].Member.ChannelMode.modeChar); }
                                        OutputRaw.append(TargetUser.ChannelList[c].Channel.Name);
                                        OutputRaw.append(' ');
                                    }
                                    else
                                    {
                                        OutputRaw.length--;
                                        OutputRaw.append(Resources.CRLF);
                                        user.Send(new String8(OutputRaw.bytes, 0, OutputRaw.Length));
                                        OutputRaw.length = 0;
                                        OutputRaw.append(WHOIS_319_RAW);
                                    }
                                }
                                OutputRaw.length--;
                                OutputRaw.append(Resources.CRLF);
                                user.Send(new String8(OutputRaw.bytes, 0, OutputRaw.Length));

                            }

                            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WHOISSERVER_312, Data: new String8[] { TargetUser.Address.Nickname, server.Name, Resources.Null }));

                            if (TargetUser.Profile.Away)
                            {
                                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_AWAY_301, Data: new String8[] { TargetUser.Address.Nickname, TargetUser.Profile.AwayReason }));
                            }

                            if (TargetUser.Level == UserAccessLevel.ChatAdministrator) { user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WHOISOPERATOR_313A, Data: new String8[] { TargetUser.Address.Nickname })); }
                            else if (TargetUser.Level >= UserAccessLevel.ChatGuide) { user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WHOISOPERATOR_313O, Data: new String8[] { TargetUser.Address.Nickname })); }

                            if (user.Level >= UserAccessLevel.ChatGuide)
                            {
                                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WHOISIP_320, Data: new String8[] { TargetUser.Address.Nickname, TargetUser.Address.RemoteIP }));
                            }

                            if (TargetUser.Modes.Secure.Value == 1)
                            {
                                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRC2_RPL_WHOISSECURE_671, Data: new String8[] { TargetUser.Address.Nickname }));
                            }

                            int SecondsSinceLogon = ((int)((TargetUser.LoggedOn - Resources.epoch) / TimeSpan.TicksPerSecond)), SecondsIdle = ((int)((DateTime.UtcNow.Ticks - TargetUser.LastIdle) / TimeSpan.TicksPerSecond));
                            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WHOISIDLE_317, Data: new String8[] { TargetUser.Address.Nickname }, IData: new int[] { SecondsIdle, SecondsSinceLogon }));
                            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_ENDOFWHOIS_318, Data: new String8[] { TargetUser.Address.Nickname }));

                        }
                        else
                        {
                            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_NOSUCHNICK_401_N, Data: new String8[] { Nicknames[i] }));
                        }
                    }

                }
                else
                {
                    user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new String8[] { Resources.CommandWhois }));
                }
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
