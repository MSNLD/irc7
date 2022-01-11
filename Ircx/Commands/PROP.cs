using System;
using Core.CSharpTools;
using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

internal class PROP : Command
{
    public PROP(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.Data;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        var server = Frame.Server;
        var user = Frame.User;
        var message = Frame.Message;

        if (user.Authenticated)
        {
            //PROP # *
            //PROP # TOPIC
            //PROP # TOPIC :TEXT
            //PROP # TOPIC TEXT TEXT
            if (message.Data.Count >= 2)
            {
                #region CHAN PROP

                if (Channel.IsChannel(message.Data[0]))
                {
                    if (!user.Registered)
                    {
                        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_NOTREGISTERED_451));
                        return COM_RESULT.COM_SUCCESS;
                    }

                    var c = server.Channels.GetChannel(message.Data[0]);

                    if (c != null)
                    {
                        var UserLevel = user.Level;
                        if (UserLevel < UserAccessLevel.ChatGuide)
                        {
                            var uci = user.GetChannelInfo(c);

                            if (uci != null)
                            {
                                if (Flood.FloodCheck(DataType, uci) == FLD_RESULT.S_WAIT) return COM_RESULT.COM_WAIT;
                                UserLevel = uci.Member.Level;
                            }
                        }

                        if (message.Data[1][0] == (byte) '*')
                        {
                            //list properties for current user level
                            var eligible = true;
                            if (UserLevel >= UserAccessLevel.ChatGuide)
                                eligible = true;
                            else if ((c.Modes.Private.Value == 1 || c.Modes.Secret.Value == 1) && !user.IsOnChannel(c))
                                eligible = false;

                            if (eligible)
                                for (var i = 0; i < c.Properties.List.Count; i++)
                                    if (c.Properties.List[i].Value.Length > 0)
                                        if (c.Properties.List[i].ReadLevel <= UserLevel &&
                                            !c.Properties.List[i].Hidden) /* display prop */
                                            user.Send(Raws.Create(server, c, user, Raws.IRCX_RPL_PROPLIST_818,
                                                new[] {c.Name, c.Properties.List[i].Name, c.Properties.List[i].Value}));
                            //send end of properties
                            user.Send(Raws.Create(server, c, user, Raws.IRCX_RPL_PROPEND_819, new[] {c.Name}));
                        }
                        else
                        {
                            //resolve property
                            var PropertyValue = new string(message.Data[1].ToUpper());

                            var ChannelProperty = c.Properties.GetPropByName(PropertyValue);
                            if (ChannelProperty != null)
                            {
                                if (message.Data.Count >= 3)
                                {
                                    //user wants to change a property

                                    if (!user.IsOnChannel(c))
                                    {
                                        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                                            Data: new[] {message.Data[0]}));
                                        //not on channel
                                    }
                                    else if (ChannelProperty.WriteLevel <= UserLevel)
                                    {
                                        //change and send PROP to channel
                                        if (ChannelProperty.Limit > 0)
                                        {
                                            if (message.Data[2].Length <= ChannelProperty.Limit)
                                            {
                                                var Value = message.Data[2];
                                                if (ChannelProperty == c.Properties.Memberkey)
                                                {
                                                    if (Value.Length > 0)
                                                        c.Modes.Key.Value = 1;
                                                    else
                                                        c.Modes.Key.Value = 0;

                                                    c.Modes.UpdateModes(ChannelProperty);
                                                }
                                                else if (ChannelProperty == c.Properties.Topic)
                                                {
                                                    c.Properties.TopicLastChanged =
                                                        (DateTime.UtcNow.Ticks - Resources.epoch) /
                                                        TimeSpan.TicksPerSecond;
                                                }
                                                else if (ChannelProperty == c.Properties.ClientGuid)
                                                {
                                                    if (Value.Length == 32)
                                                    {
                                                        Guid guid;
                                                        Guid.TryParseExact(Value, "N", out guid);
                                                        if (guid == Guid.Empty)
                                                        {
                                                            //bad value specified
                                                            user.Send(Raws.Create(server, c, user,
                                                                Raws.IRCX_ERR_BADVALUE_906, new[] {message.Data[2]}));
                                                            return COM_RESULT.COM_SUCCESS;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        user.Send(Raws.Create(server, c, user,
                                                            Raws.IRCX_ERR_BADVALUE_906, new[] {message.Data[2]}));
                                                        return COM_RESULT.COM_SUCCESS;
                                                    }
                                                }

                                                ChannelProperty.Value = Value;
                                                c.SendLevel(
                                                    Raws.Create(server, c, user, Raws.RPL_PROP_IRCX,
                                                        new[] {ChannelProperty.Name, ChannelProperty.Value}),
                                                    ChannelProperty.ReadLevel);
                                            }
                                            else
                                            {
                                                //bad value specified
                                                user.Send(Raws.Create(server, c, user, Raws.IRCX_ERR_BADVALUE_906,
                                                    new[] {message.Data[2]}));
                                            }
                                        }
                                        else if (ChannelProperty.Limit == 0)
                                        {
                                            //this is how exchange 5.5 worked
                                            //if <= 9 chars and all numeric then parse to integer
                                            //then rest of logic
                                            //else
                                            //bad command
                                            if (message.Data[2].Length <= 9)
                                            {
                                                var number = Tools.Str2Int(message.Data[2]);
                                                if (number > -1)
                                                {
                                                    //treat like LAG
                                                    if (number >= 0 && number <= 2)
                                                    {
                                                        ChannelProperty.Value = message.Data[2];
                                                        c.SendLevel(
                                                            Raws.Create(server, c, user, Raws.RPL_PROP_IRCX,
                                                                new[] {ChannelProperty.Name, ChannelProperty.Value}),
                                                            ChannelProperty.ReadLevel);
                                                        c.FloodProfile.FloodProtectionLevel.Delay = number;
                                                    }
                                                    else
                                                    {
                                                        //bad value specified
                                                        user.Send(Raws.Create(server, c, user,
                                                            Raws.IRCX_ERR_BADVALUE_906, new[] {message.Data[2]}));
                                                    }
                                                }
                                                else
                                                {
                                                    //bad command
                                                    user.Send(Raws.Create(server, Client: user,
                                                        Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                                        Data: new[] {Resources.CommandProp}));
                                                }
                                            }
                                            else
                                            {
                                                //bad command
                                                user.Send(Raws.Create(server, Client: user,
                                                    Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                                    Data: new[] {Resources.CommandProp}));
                                            }
                                        }
                                        else
                                        {
                                            ; //this is taken care of as if its -1 then you cannot write to it, and therefore no permissions
                                        }
                                    }
                                    else
                                    {
                                        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_SECURITY_908));
                                    }
                                }
                                else
                                {
                                    //user wants to read a property
                                    if (ChannelProperty.Value.Length > 0)
                                        if (ChannelProperty.ReadLevel <= UserLevel &&
                                            !ChannelProperty.Hidden) /* display prop */
                                            user.Send(Raws.Create(server, c, user, Raws.IRCX_RPL_PROPLIST_818,
                                                new[] {c.Name, ChannelProperty.Name, ChannelProperty.Value}));
                                    //send end of properties
                                    user.Send(Raws.Create(server, c, user, Raws.IRCX_RPL_PROPEND_819, new[] {c.Name}));
                                }
                            }
                            else
                            {
                                //<- :Default-Chat-Community 905 Sky2k #test :Bad property specified (muel)
                                user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADPROPERTY_905,
                                    Data: new[] {message.Data[1]}));
                            }
                        }
                    }
                    else
                    {
                        //<- :Default-Chat-Community 461 Sky2k PROP :Not enough parameters
                        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                            Data: new[] {message.Command}));
                    }
                }

                #endregion

                #region USER PROP

                //resolve 2nd param, is it
                else
                {
                    if (Flood.FloodCheck(DataType, Frame.User) == FLD_RESULT.S_WAIT) return COM_RESULT.COM_WAIT;

                    User TargetUser = null;
                    var ObjectNickname = new string(message.Data[0].ToUpper());

                    if (ObjectNickname.Length == 1 && ObjectNickname[0] == (byte) '$')
                        TargetUser = user;
                    else if (user.Address.UNickname == ObjectNickname)
                        TargetUser = user;
                    else
                        TargetUser = server.Users.GetUser(ObjectNickname);

                    if (TargetUser != null)
                    {
                        //for PROP $ NICK, PROP $ MSNPROFILE etc
                        if (message.Data[1][0] == (byte) '*')
                        {
                            //send end of properties
                            user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_SECURITY_908));
                        }
                        else
                        {
                            //resolve property
                            var PropertyValue = new string(message.Data[1].ToUpper());

                            var UserProperty = TargetUser.Properties.GetPropByName(PropertyValue);
                            if (UserProperty != null)
                            {
                                if (message.Data.Count >= 3)
                                {
                                    //user wants to change a property
                                    if (UserProperty.WriteLevel <= user.Level)
                                    {
                                        var Value = message.Data[2];
                                        //change and send PROP to channel
                                        if (UserProperty.Limit > 0)
                                        {
                                            if (message.Data[2].Length <= UserProperty.Limit)
                                            {
                                                if (!user.Guest && UserProperty.Name == Resources.UserPropMsnRegCookie)
                                                {
                                                    var RegCookie = message.Data[2];
                                                    var r = Passport3.DecryptRegCookie(RegCookie);
                                                    if (r.version == 3)
                                                    {
                                                        var Nickname = r.nickname;

                                                        if (TargetUser.Registered)
                                                            NICK.UpdateNickname(Frame.Server, Frame.User, Nickname);
                                                        server.UpdateUserNickname(TargetUser, Nickname);
                                                        //TargetUser.Props.Puid.Value = new string(Puid.bytes, 0, Puid.Length);
                                                        //TargetUser.HasUser = true;
                                                        //Core.Ircx.Runtime.Register.QualifyUser(server, Frame.Connection);
                                                        UserProperty.SetPermissions(UserAccessLevel.NoAccess,
                                                            UserAccessLevel.NoAccess, true, false);
                                                    }
                                                }
                                                else
                                                {
                                                    user.Send(Raws.Create(server, Client: user,
                                                        Raw: Raws.IRCX_ERR_BADVALUE_906,
                                                        Data: new[] {message.Data[3]}));
                                                }
                                            }
                                            else
                                            {
                                                //bad value specified
                                                user.Send(Raws.Create(server, Client: user,
                                                    Raw: Raws.IRCX_ERR_BADVALUE_906, Data: new[] {message.Data[2]}));
                                            }
                                        }
                                        else
                                        {
                                            if (!user.Guest &&
                                                UserProperty.Limit == 0) // only possibility is MSNPROFILE
                                            {
                                                //this is how exchange 5.5 worked
                                                //if <= 9 chars and all numeric then parse to integer
                                                //then rest of logic
                                                //else
                                                //bad command
                                                if (message.Data[2].Length <= 9)
                                                {
                                                    var number = Tools.Str2Int(message.Data[2]);
                                                    if (number > -1)
                                                    {
                                                        switch (number)
                                                        {
                                                            case 0:
                                                            {
                                                                user.Profile.UserType =
                                                                    user.Guest
                                                                        ? ProfileUserType.Guest
                                                                        : ProfileUserType.Registered;
                                                                break;
                                                            }
                                                            case 1:
                                                            {
                                                                user.Profile.UserType = ProfileUserType.ProfileOnly;
                                                                break;
                                                            }
                                                            case 3:
                                                            {
                                                                user.Profile.UserType = ProfileUserType.Male;
                                                                break;
                                                            }
                                                            case 5:
                                                            {
                                                                user.Profile.UserType = ProfileUserType.Female;
                                                                break;
                                                            }
                                                            case 9:
                                                            {
                                                                user.Profile.UserType = ProfileUserType.ProfileOnly;
                                                                user.Profile.Picture = true;
                                                                break;
                                                            }
                                                            case 11:
                                                            {
                                                                user.Profile.UserType = ProfileUserType.Male;
                                                                user.Profile.Picture = true;
                                                                break;
                                                            }
                                                            case 13:
                                                            {
                                                                user.Profile.UserType = ProfileUserType.Female;
                                                                user.Profile.Picture = true;
                                                                break;
                                                            }
                                                            default:
                                                            {
                                                                user.Send(Raws.Create(server, Client: user,
                                                                    Raw: Raws.IRCX_ERR_BADVALUE_906,
                                                                    Data: new[] {message.Data[3]}));
                                                                return COM_RESULT.COM_SUCCESS;
                                                            }
                                                        }

                                                        UserProperty.Value = message.Data[2];
                                                        user.Send(Raws.Create(server, Client: user,
                                                            Raw: Raws.IRCX_RPL_PROPLIST_818,
                                                            Data: new[]
                                                            {
                                                                user.Address.Nickname, UserProperty.Name,
                                                                UserProperty.Value
                                                            }));
                                                    }
                                                    else
                                                    {
                                                        //bad command
                                                        user.Send(Raws.Create(server, Client: user,
                                                            Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                                            Data: new[] {Resources.CommandProp}));
                                                    }
                                                }
                                                else
                                                {
                                                    //bad command
                                                    user.Send(Raws.Create(server, Client: user,
                                                        Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                                        Data: new[] {Resources.CommandProp}));
                                                }
                                            }
                                            else
                                            {
                                                user.Send(Raws.Create(server, Client: user,
                                                    Raw: Raws.IRCX_ERR_SECURITY_908));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_SECURITY_908));
                                    }
                                }
                                else
                                {
                                    //user wants to read a property
                                    if (UserProperty.Value.Length > 0)
                                        if (UserProperty.ReadLevel <= user.Level)
                                            // TODO: Fix NTLM
                                            //if (user.Auth.Signature == Authentication.Package.NTLM.SIGNATURE)
                                            //{
                                            //    if (!user.Registered)
                                            //    {
                                            //        TargetUser.Name = UserProperty.Value;
                                            //        Runtime.Register.QualifyUser(server, Frame.Connection);
                                            //        return COM_RESULT.COM_SUCCESS;
                                            //    }
                                            //}
                                            /* display prop */
                                            user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_PROPLIST_818,
                                                Data: new[]
                                                {
                                                    TargetUser.Address.Nickname, UserProperty.Name, UserProperty.Value
                                                }));
                                    //send end of properties
                                    user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_PROPEND_819,
                                        Data: new[] {TargetUser.Address.Nickname}));
                                }
                            }
                            else
                            {
                                //<- :Default-Chat-Community 905 Sky2k #test :Bad property specified (muel)
                                user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADPROPERTY_905,
                                    Data: new[] {message.Data[1]}));
                            }
                        }
                    }
                    else
                    {
                        //NO such object!
                        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_NOSUCHOBJECT_924,
                            Data: new[] {message.Data[0]}));
                    }
                }

                #endregion
            }
            else
            {
                //insufficient parameters
                user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                    Data: new[] {message.Command}));
            }
        }

        return COM_RESULT.COM_SUCCESS;
    }
}