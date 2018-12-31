using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class WHO : Command
    {

        public WHO(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.DataType = CommandDataType.None;
            base.RegistrationRequired = true;
        }

        public static void SendWhoChannelUser(Frame Frame, Channel c, ChannelMember TargetUser)
        {
            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_WHOREPLY_352, Data: new String8[] {
                                //<channel> <user> <host> <server> <nick> \
                             //<H|G>[*][@|+] :<hopcount> <real name>

                                c.Name,
                                TargetUser.User.Address.Userhost, TargetUser.User.Address.Hostname, TargetUser.User.Address.Nickname,
                                (TargetUser.User.Profile.Away ? Resources.Gone : Resources.Home),
                                //(TargetUser.Level == UserAccessLevel.ChatAdministrator ? Resources.Admin : Resources.Null),
                                TargetUser.ChannelMode.ModeString,
                                TargetUser.User.Modes.UserModeString,
                                TargetUser.User.Address.RealName
                            }));
        }
        public static void SendWhoServerUser(Frame Frame, User TargetUser)
        {

        }

        public static void SendWho(Frame Frame, Channel c, ChannelMemberCollection Members)
        {
            if (Members != null)
            {
                if (Members.MemberList.Count > 0)
                {
                    for (int i = 0; i < Members.MemberList.Count; i++)
                    {
                        SendWhoChannelUser(Frame, c, Members.MemberList[i]);
                    }
                }
            }
            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ENDOFWHO_315, Data: new String8[] { Frame.Message.Data[0] }));
        }
        public static void SendWho(Frame Frame, List<User> Members)
        {

            //Loop through members, if channels exist output if not hidden,secret,private
            //Else output without channels



            if (Members != null)
            {
                if (Members.Count > 0)
                {
                    for (int i = 0; i < Members.Count; i++)
                    {
                        //list
                        User User = Members[i];
                        
                        if (User.ChannelList.Count > 0)
                        {
                            for (int x = 0; x < User.ChannelList.Count; x++)
                            {
                                Channel c = User.ChannelList[x].Channel;
                                if ((c.Modes.Hidden.Value != 0x1) && (c.Modes.Secret.Value != 0x1) && (c.Modes.Private.Value != 0x1))
                                {
                                    ChannelMember TargetUser = User.ChannelList[x].Member;
                                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_WHOREPLY_352, Data: new String8[] {
                                    //<channel> <user> <host> <server> <nick> \
                                     //<H|G>[*][@|+] :<hopcount> <real name>

                                        c.Name,
                                        TargetUser.User.Address.Userhost, TargetUser.User.Address.Hostname, TargetUser.User.Address.Nickname,
                                        (TargetUser.User.Profile.Away ? Resources.Gone : Resources.Home),
                                        //(TargetUser.Level == UserAccessLevel.ChatAdministrator ? Resources.Admin : Resources.Null),
                                        TargetUser.ChannelMode.ModeString,
                                        TargetUser.User.Modes.UserModeString,
                                        TargetUser.User.Address.RealName
                                    }));
                                }

                            }
                            
                        }
                        else
                        {
                            // output without channel
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_WHOREPLY_352, Data: new String8[] {
                                    //<channel> <user> <host> <server> <nick>
                                     //<H|G>[*][@|+]<usermodes> :<hopcount> <real name>
                                     //public static String8 IRCX_RPL_WHOREPLY_352 = ":%h 352 %n %s %s %s %h %s %s%s%s :0 %s";
                                        Resources.Wildcard,
                                        User.Address.Userhost, User.Address.Hostname, User.Address.Nickname,
                                        (User.Profile.Away ? Resources.Gone : Resources.Home),
                                        //(User.Level == UserAccessLevel.ChatAdministrator ? Resources.Admin : Resources.Null),
                                        User.Modes.UserModeString,
                                        Resources.Null,

                                        User.Address.RealName
                                    }));
                        }

                    }
                }
            }
            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ENDOFWHO_315, Data: new String8[] { Frame.Message.Data[0] }));
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Server server = Frame.Server;
            Message message = Frame.Message;
            User user = Frame.User;

            

            if (Channel.IsChannel(message.Data[0]))
            {
                ChannelMemberCollection WhoUsers = null;
                
                Channel c = server.Channels.GetChannel(message.Data[0]);
                if (c != null)
                {
                    if (user.IsOnChannel(c))
                    {
                        //permits listing
                        if ((c.Modes.Auditorium.Value == 0x1) && (user.Level < UserAccessLevel.ChatHost))
                        {
                            //filtered output
                            WhoUsers = c.GetMembersByLevel(user, UserAccessLevel.ChatHost);
                        }
                        else
                        {
                            WhoUsers = c.Members;
                        }
                    }
                    else
                    {
                        if (user.Level < UserAccessLevel.ChatGuide)
                        {
                            if ((c.Modes.Private.Value == 0x1) || (c.Modes.Secret.Value == 0x1))
                            {
                                WhoUsers = new ChannelMemberCollection(); //No output
                            }
                            else if (c.Modes.Auditorium.Value == 0x1)
                            {
                                //filtered output
                                WhoUsers = c.GetMembersByLevel(user, UserAccessLevel.ChatHost);
                            }
                            else
                            {
                                WhoUsers = c.Members;
                            }
                        }
                        else
                        { //Guides+ get everything
                            WhoUsers = c.Members;
                        }
                    }
                    SendWho(Frame, c, WhoUsers);
                }
            }
            else
            {
                List<User> WhoUsers = null;
                WhoUsers = new List<User>();
                for (int i = 0; i < server.Users.Length; i++)
                {
                    if (server.Users[i].Registered)
                    {
                        if ((server.Users[i].Modes.Invisible.Value == 0x1) && (user.Level < UserAccessLevel.ChatGuide) && (server.Users[i] != user)) ;
                        else if (String8RegEx.EvaluateString8(message.Data[0], server.Users[i].Address.Nickname, true))
                        {
                            WhoUsers.Add(server.Users[i]);
                        }
                    }
                }
                SendWho(Frame, WhoUsers);
            }

            //display end of list
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
