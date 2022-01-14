using System;
using System.Linq;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Extensions.Apollo.Security;
using Irc.Extensions.Apollo.Security.Credentials;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class PROP : Command
{
    public PROP(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.Data;
    }

    public new bool Execute(Frame Frame)
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
            if (message.Parameters.Count >= 2)
            {
                #region CHAN PROP

                if (Channel.IsChannel(message.Parameters[0]))
                {
                    if (!user.Registered)
                    {
                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_NOTREGISTERED_451));
                        return true;
                    }
                    var objType = IrcHelper.IdentifyObject(message.Parameters[0]);
                    var targetChannel = server.Channels.FindObj(message.Parameters[0], objType);

                    if (targetChannel != null)
                    {
                        var userFoundChannelMemberPair = Frame.User.Channels.FirstOrDefault(c => c.Key == targetChannel);
                        var userFoundChannel = userFoundChannelMemberPair.Key;
                        var userFoundChannelMember = userFoundChannelMemberPair.Value;

                        var userLevel = user.Level;
                        if (userLevel < UserAccessLevel.ChatGuide)
                        {
                            if (userFoundChannel != null)
                            {
                                if (Flood.FloodCheck(DataType, userFoundChannelMember.User) == FLD_RESULT.S_WAIT) return false;
                                userLevel = userFoundChannelMember.Level;
                            }
                        }

                        if (message.Parameters[1][0] == (byte) '*')
                        {
                            //list properties for current user level
                            var eligible = true;
                            if (userLevel >= UserAccessLevel.ChatGuide)
                                eligible = true;
                            else if ((targetChannel.Modes.Private.Value == 1 || targetChannel.Modes.Secret.Value == 1) && userFoundChannel != null)
                                eligible = false;

                            if (eligible)
                                for (var i = 0; i < targetChannel.Properties.GetList().Count; i++)
                                    if (!string.IsNullOrWhiteSpace(targetChannel.Properties.GetList()[i].Value))
                                    {
                                        Prop prop = ChannelProperties.PropertyRules.FirstOrDefault(x =>
                                            x.Key == targetChannel.Properties.GetList()[i].Key).Value;
                                        if (prop != null)
                                        {
                                            if (prop.ReadLevel <= userLevel &&
                                                !prop.Hidden) /* display prop */
                                                user.Send(RawBuilder.Create(server, targetChannel, user, Raws.IRCX_RPL_PROPLIST_818,
                                                    new[] { targetChannel.Name, prop.Name, targetChannel.Properties.GetList()[i].Value }));
                                        }
                                    }

                            //send end of properties
                            user.Send(RawBuilder.Create(server, targetChannel, user, Raws.IRCX_RPL_PROPEND_819, new[] {targetChannel.Name}));
                        }
                        else
                        {
                            //resolve property
                            var propertyName = new string(message.Parameters[1].ToUpper());

                            var propertyValue = targetChannel.Properties.Get(propertyName);
                            ChannelProperties.PropertyRules.TryGetValue(propertyName, out var prop);

                            if (propertyValue != null)
                            {
                                if (message.Parameters.Count >= 3)
                                {
                                    //user wants to change a property

                                    if (userFoundChannel != null)
                                    {
                                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                                            Data: new[] {message.Parameters[0]}));
                                        //not on channel
                                    }
                                    else if (prop.WriteLevel <= userLevel)
                                    {
                                        //change and send PROP to channel
                                        if (prop.Limit > 0)
                                        {
                                            if (message.Parameters[2].Length <= prop.Limit)
                                            {
                                                var Value = message.Parameters[2];
                                                if (propertyValue == Resources.ChannelPropMemberkey)
                                                {
                                                    if (Value.Length > 0)
                                                        targetChannel.Modes.Key.Value = 1;
                                                    else
                                                        targetChannel.Modes.Key.Value = 0;

                                                    targetChannel.Modes.UpdateModes(propertyValue);
                                                }
                                                else if (propertyValue == Resources.ChannelPropTopic)
                                                {
                                                    targetChannel.TopicLastChanged =
                                                        (DateTime.UtcNow.Ticks - Resources.epoch) /
                                                        TimeSpan.TicksPerSecond;
                                                }
                                                else if (propertyValue == Resources.ChannelPropClientGuid)
                                                {
                                                    if (Value.Length == 32)
                                                    {
                                                        Guid guid;
                                                        Guid.TryParseExact(Value, "N", out guid);
                                                        if (guid == Guid.Empty)
                                                        {
                                                            //bad value specified
                                                            user.Send(RawBuilder.Create(server, targetChannel, user,
                                                                Raws.IRCX_ERR_BADVALUE_906, new[] {message.Parameters[2]}));
                                                            return true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        user.Send(RawBuilder.Create(server, targetChannel, user,
                                                            Raws.IRCX_ERR_BADVALUE_906, new[] {message.Parameters[2]}));
                                                        return true;
                                                    }
                                                }

                                                targetChannel.Properties.Set(propertyName, propertyValue);
                                                targetChannel.SendLevel(
                                                    RawBuilder.Create(server, targetChannel, user, Raws.RPL_PROP_IRCX,
                                                        new[] {prop.Name, propertyValue}),
                                                    prop.ReadLevel);
                                            }
                                            else
                                            {
                                                //bad value specified
                                                user.Send(RawBuilder.Create(server, targetChannel, user, Raws.IRCX_ERR_BADVALUE_906,
                                                    new[] {message.Parameters[2]}));
                                            }
                                        }
                                        else if (!string.IsNullOrWhiteSpace(propertyValue))
                                        {
                                            //this is how exchange 5.5 worked
                                            //if <= 9 chars and all numeric then parse to integer
                                            //then rest of logic
                                            //else
                                            //bad command
                                            if (message.Parameters[2].Length <= 9)
                                            {
                                                var number = Tools.Str2Int(message.Parameters[2]);
                                                if (number > -1)
                                                {
                                                    //treat like LAG
                                                    if (number >= 0 && number <= 2)
                                                    {
                                                        targetChannel.Properties.Set(propertyName, message.Parameters[2]);
                                                        targetChannel.SendLevel(
                                                            RawBuilder.Create(server, targetChannel, user, Raws.RPL_PROP_IRCX,
                                                                new[] {prop.Name, propertyValue}),
                                                            prop.ReadLevel);
                                                        targetChannel.FloodProfile.FloodProtectionLevel.Delay = number;
                                                    }
                                                    else
                                                    {
                                                        //bad value specified
                                                        user.Send(RawBuilder.Create(server, targetChannel, user,
                                                            Raws.IRCX_ERR_BADVALUE_906, new[] {message.Parameters[2]}));
                                                    }
                                                }
                                                else
                                                {
                                                    //bad command
                                                    user.Send(RawBuilder.Create(server, Client: user,
                                                        Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                                        Data: new[] {Resources.CommandProp}));
                                                }
                                            }
                                            else
                                            {
                                                //bad command
                                                user.Send(RawBuilder.Create(server, Client: user,
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
                                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_SECURITY_908));
                                    }
                                }
                                else
                                {
                                    //user wants to read a property
                                    if (!string.IsNullOrWhiteSpace(propertyValue))
                                    {
                                        if (prop.ReadLevel <= userLevel &&
                                            !prop.Hidden) /* display prop */
                                            user.Send(RawBuilder.Create(server, targetChannel, user, Raws.IRCX_RPL_PROPLIST_818,
                                                new[] { targetChannel.Name, prop.Name, propertyValue }));
                                    }

                                    //send end of properties
                                    user.Send(RawBuilder.Create(server, targetChannel, user, Raws.IRCX_RPL_PROPEND_819, new[] {targetChannel.Name}));
                                }
                            }
                            else
                            {
                                //<- :Default-Chat-Community 905 Sky2k #test :Bad property specified (muel)
                                user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADPROPERTY_905,
                                    Data: new[] {message.Parameters[1]}));
                            }
                        }
                    }
                    else
                    {
                        //<- :Default-Chat-Community 461 Sky2k PROP :Not enough parameters
                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                            Data: new[] {message.GetCommand() }));
                    }
                }

                #endregion

                #region USER PROP

                //resolve 2nd param, is it
                else
                {
                    if (Flood.FloodCheck(DataType, Frame.User) == FLD_RESULT.S_WAIT) return false;

                    User TargetUser = null;
                    var ObjectNickname = new string(message.Parameters[0].ToUpper());
                    var objIdentifier = IrcHelper.IdentifyObject(ObjectNickname);

                    if (ObjectNickname.Length == 1 && ObjectNickname[0] == (byte) '$')
                        TargetUser = user;
                    else if (user.Address.Nickname.ToUpper() == ObjectNickname)
                        TargetUser = user;
                    else
                        TargetUser = server.Users.FindObj(ObjectNickname, objIdentifier);

                    if (TargetUser != null)
                    {
                        //for PROP $ NICK, PROP $ MSNPROFILE etc
                        if (message.Parameters[1][0] == (byte) '*')
                        {
                            //send end of properties
                            user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_SECURITY_908));
                        }
                        else
                        {
                            //resolve property
                            var propertyName = new string(message.Parameters[1].ToUpper());
                                UserProperties.PropertyRules.TryGetValue(propertyName, out var propertyRule);


                            var propertyValue = TargetUser.Properties.Get(propertyName);
                            if (propertyRule != null && !string.IsNullOrWhiteSpace(propertyValue))
                            {
                                if (message.Parameters.Count >= 3)
                                {
                                    //user wants to change a property
                                    if (propertyRule.WriteLevel <= user.Level)
                                    {
                                        var Value = message.Parameters[2];
                                        //change and send PROP to channel
                                        if (propertyRule.Limit > 0)
                                        {
                                            if (message.Parameters[2].Length <= propertyRule.Limit)
                                            {
                                                if (!user.Guest && propertyRule.Name == Resources.UserPropMsnRegCookie)
                                                {
                                                    var RegCookie = message.Parameters[2];
                                                    var r = (new Passport(Program.Config.PassportKey)).DecryptRegCookie(RegCookie);
                                                    if (r.version == 3)
                                                    {
                                                        var Nickname = r.nickname;

                                                        if (TargetUser.Registered)
                                                            NICK.UpdateNickname(Frame.Server, Frame.User, Nickname);
                                                        TargetUser.UpdateUserNickname(Nickname);
                                                        //TargetUser.Props.Puid.Value = new string(Puid.bytes, 0, Puid.Length);
                                                        //TargetUser.HasUser = true;
                                                        //Core.Ircx.Runtime.Register.QualifyUser(server, Frame.Connection);
                                                        propertyRule.SetPermissions(UserAccessLevel.NoAccess,
                                                            UserAccessLevel.NoAccess, true, false);
                                                    }
                                                }
                                                else
                                                {
                                                    user.Send(RawBuilder.Create(server, Client: user,
                                                        Raw: Raws.IRCX_ERR_BADVALUE_906,
                                                        Data: new[] {message.Parameters[3]}));
                                                }
                                            }
                                            else
                                            {
                                                //bad value specified
                                                user.Send(RawBuilder.Create(server, Client: user,
                                                    Raw: Raws.IRCX_ERR_BADVALUE_906, Data: new[] {message.Parameters[2]}));
                                            }
                                        }
                                        else
                                        {
                                            if (!user.Guest &&
                                                propertyRule.Limit == 0) // only possibility is MSNPROFILE
                                            {
                                                //this is how exchange 5.5 worked
                                                //if <= 9 chars and all numeric then parse to integer
                                                //then rest of logic
                                                //else
                                                //bad command
                                                if (message.Parameters[2].Length <= 9)
                                                {
                                                    var number = Tools.Str2Int(message.Parameters[2]);
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
                                                                user.Send(RawBuilder.Create(server, Client: user,
                                                                    Raw: Raws.IRCX_ERR_BADVALUE_906,
                                                                    Data: new[] {message.Parameters[3]}));
                                                                return true;
                                                            }
                                                        }

                                                        user.Properties.Set(propertyName, message.Parameters[2]);
                                                        user.Send(RawBuilder.Create(server, Client: user,
                                                            Raw: Raws.IRCX_RPL_PROPLIST_818,
                                                            Data: new[]
                                                            {
                                                                user.Address.Nickname, propertyRule.Name,
                                                                propertyValue
                                                            }));
                                                    }
                                                    else
                                                    {
                                                        //bad command
                                                        user.Send(RawBuilder.Create(server, Client: user,
                                                            Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                                            Data: new[] {Resources.CommandProp}));
                                                    }
                                                }
                                                else
                                                {
                                                    //bad command
                                                    user.Send(RawBuilder.Create(server, Client: user,
                                                        Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                                        Data: new[] {Resources.CommandProp}));
                                                }
                                            }
                                            else
                                            {
                                                user.Send(RawBuilder.Create(server, Client: user,
                                                    Raw: Raws.IRCX_ERR_SECURITY_908));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_SECURITY_908));
                                    }
                                }
                                else
                                {
                                    //user wants to read a property
                                    if (!string.IsNullOrWhiteSpace(propertyValue))
                                        if (propertyRule.ReadLevel <= user.Level)
                                            // TODO: Fix NTLM
                                            //if (user.Auth.Signature == Authentication.Package.NTLM.SIGNATURE)
                                            //{
                                            //    if (!user.Registered)
                                            //    {
                                            //        TargetUser.Name = UserProperty.Value;
                                            //        Runtime.Register.QualifyUser(server, Frame.Connection);
                                            //        return true;
                                            //    }
                                            //}
                                            /* display prop */
                                            user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_PROPLIST_818,
                                                Data: new[]
                                                {
                                                    TargetUser.Address.Nickname, propertyRule.Name, propertyValue
                                                }));
                                    //send end of properties
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_PROPEND_819,
                                        Data: new[] {TargetUser.Address.Nickname}));
                                }
                            }
                            else
                            {
                                //<- :Default-Chat-Community 905 Sky2k #test :Bad property specified (muel)
                                user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADPROPERTY_905,
                                    Data: new[] {message.Parameters[1]}));
                            }
                        }
                    }
                    else
                    {
                        //NO such object!
                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_NOSUCHOBJECT_924,
                            Data: new[] {message.Parameters[0]}));
                    }
                }

                #endregion
            }
            else
            {
                //insufficient parameters
                user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                    Data: new[] {message.GetCommand() }));
            }
        }

        return true;
    }
}