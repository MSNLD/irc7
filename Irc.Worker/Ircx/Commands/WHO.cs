using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class WHO : Command
{
    public WHO(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.None;
        RegistrationRequired = true;
    }

    public static void SendWhoChannelUser(Frame Frame, Channel c, ChannelMember TargetUser)
    {
        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_WHOREPLY_352, Data: new[]
        {
            //<channel> <user> <host> <server> <nick> \
            //<H|G>[*][@|+] :<hopcount> <real name>

            c.Name,
            TargetUser.User.Address.User, TargetUser.User.Address.Host, TargetUser.User.Address.Nickname,
            TargetUser.User.Profile.Away ? Resources.Gone : Resources.Home,
            //(TargetUser.Level == UserAccessLevel.ChatAdministrator ? Resources.Admin : string.Empty),
            TargetUser.ChannelMode.ModeString,
            TargetUser.User.Modes.UserModeString,
            TargetUser.User.RealName
        }));
    }

    public static void SendWhoServerUser(Frame Frame, User TargetUser)
    {
    }

    public static void SendWho(Frame Frame, Channel c, IList<ChannelMember> Members)
    {
        if (Members != null)
            if (Members.Count > 0)
            {
                foreach (var channelMember in Members)
                {
                    SendWhoChannelUser(Frame, c, channelMember);
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ENDOFWHO_315,
                        Data: new[] { Frame.Message.Parameters[0] }));
                }
            }
    }

    public static void SendWho(Frame Frame, List<User> Members)
    {
        //Loop through members, if channels exist output if not hidden,secret,private
        //Else output without channels


        if (Members != null)
            if (Members.Count > 0)
                for (var i = 0; i < Members.Count; i++)
                {
                    //list
                    var User = Members[i];

                    if (User.Channels.Count > 0)
                        foreach (var channelMemberPair in User.Channels)
                        {
                            var channel = channelMemberPair.Key;
                            var member = channelMemberPair.Value;
                            if (channel.Modes.Hidden.Value != 0x1 && channel.Modes.Secret.Value != 0x1 &&
                                channel.Modes.Private.Value != 0x1)
                            {
                                var TargetUser = member;
                                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                                    Raw: Raws.IRCX_RPL_WHOREPLY_352, Data: new[]
                                    {
                                        //<channel> <user> <host> <server> <nick> \
                                        //<H|G>[*][@|+] :<hopcount> <real name>

                                        channel.Name,
                                        TargetUser.User.Address.User, TargetUser.User.Address.Host,
                                        TargetUser.User.Address.Nickname,
                                        TargetUser.User.Profile.Away ? Resources.Gone : Resources.Home,
                                        //(TargetUser.Level == UserAccessLevel.ChatAdministrator ? Resources.Admin : string.Empty),
                                        TargetUser.ChannelMode.ModeString,
                                        TargetUser.User.Modes.UserModeString,
                                        TargetUser.User.RealName
                                    }));
                            }

                        }
                    else
                        // output without channel
                        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_WHOREPLY_352,
                            Data: new[]
                            {
                                //<channel> <user> <host> <server> <nick>
                                //<H|G>[*][@|+]<usermodes> :<hopcount> <real name>
                                //public static string IRCX_RPL_WHOREPLY_352 = ":%h 352 %n %s %s %s %h %s %s%s%s :0 %s";
                                Resources.Wildcard,
                                User.Address.User, User.Address.Host, User.Address.Nickname,
                                User.Profile.Away ? Resources.Gone : Resources.Home,
                                //(User.Level == UserAccessLevel.ChatAdministrator ? Resources.Admin : string.Empty),
                                User.Modes.UserModeString,
                                string.Empty,

                                User.RealName
                            }));
                }

        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ENDOFWHO_315,
            Data: new[] {Frame.Message.Parameters[0]}));
    }

    public new bool Execute(Frame Frame)
    {
        var server = Frame.Server;
        var message = Frame.Message;
        var user = Frame.User;


        if (Channel.IsChannel(message.Parameters[0]))
        {
            var objType = IrcHelper.IdentifyObject(message.Parameters[0]);
            var c = server.Channels.FindObj(message.Parameters[0], objType);
            if (c != null)
            {
                IList<ChannelMember> whoUsers;
                if (user.Channels.ContainsKey(c))
                {
                    //permits listing
                    if (c.Modes.Auditorium.Value == 0x1 && user.Level < UserAccessLevel.ChatHost)
                        //filtered output
                        whoUsers = c.GetMembersByLevel(user, UserAccessLevel.ChatHost);
                    else
                        whoUsers = c.Members;
                }
                else
                {
                    if (user.Level < UserAccessLevel.ChatGuide)
                    {
                        if (c.Modes.Private.Value == 0x1 || c.Modes.Secret.Value == 0x1)
                            whoUsers = new List<ChannelMember>(); //No output
                        else if (c.Modes.Auditorium.Value == 0x1)
                            //filtered output
                            whoUsers = c.GetMembersByLevel(user, UserAccessLevel.ChatHost);
                        else
                            whoUsers = c.Members;
                    }
                    else
                    {
                        //Guides+ get everything
                        whoUsers = c.Members;
                    }
                }

                SendWho(Frame, c, whoUsers);
            }
        }
        else
        {
            List<User> WhoUsers = null;
            WhoUsers = new List<User>();
            for (var i = 0; i < server.Users.Count; i++)
                if (server.Users[i].Registered)
                {
                    if (server.Users[i].Modes.Invisible.Value == 0x1 && user.Level < UserAccessLevel.ChatGuide &&
                        server.Users[i] != user) ;
                    else if (StringBuilderRegEx.EvaluateString(message.Parameters[0], server.Users[i].Address.Nickname, true))
                        WhoUsers.Add(server.Users[i]);
                }

            SendWho(Frame, WhoUsers);
        }

        //display end of list
        return true;
    }
}