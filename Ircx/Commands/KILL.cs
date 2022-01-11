using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

internal class KILL : Command
{
    public KILL(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.None;
        RegistrationRequired = true;
        PreRegistration = false;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        if (Frame.Message.Data.Count >= 1)
        {
            var objs = Frame.Server.GetObjects(Frame.Message.Data[0]);

            if (objs.Count > 0)
            {
                // Supports KILL OID1,OID2,OID3 Reason
                // Can mix user / chan

                var Reason = Resources.Null;
                if (Frame.Message.Data.Count > 1) Reason = Frame.Message.Data[1];

                for (var i = 0; i < objs.Count; i++)
                    //Determine type
                    //Kill with / without reason

                    if (objs[i].ObjectType == ObjType.ChannelObject)
                    {
                        Frame.Channel = (Channel) objs[i];
                        ProcessChannelKill(Frame, Reason);
                    }
                    else if (objs[i].ObjectType == ObjType.UserObject)
                    {
                        var TargetUser = (User) objs[i];
                        ProcessKill(Frame, TargetUser, Reason);
                    }
            }
        }

        return COM_RESULT.COM_SUCCESS;
    }

    public bool ProcessKill(Frame Frame, User TargetUser, string Reason)
    {
        if (Frame.User.Level >= UserAccessLevel.ChatGuide && Frame.User.Level >= TargetUser.Level)
        {
            var channels = TargetUser.ChannelList;

            var KillRaw = Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.RPL_KILL_IRC,
                Data: new[] {TargetUser.Address.Nickname, Reason});

            while (channels.Count > 0)
            {
                var channel = channels[0].Channel;
                //TargetUser.ChannelMode.SetNormal();
                // Need to reset usermodes
                channel.RemoveMember(TargetUser);
                TargetUser.RemoveChannel(channel);

                //Broadcast to channel after user has been removed
                channel.Send(KillRaw, Frame.User, true);
            }

            if (Frame.User != TargetUser) Frame.User.Send(KillRaw);
            TargetUser.Send(KillRaw);
            TargetUser.Terminate();
            return true;
        }

        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_SECURITY_908));
        return false;
    }

    public void ProcessChannelKill(Frame Frame, string Reason)
    {
        if (Frame.User.Level >= UserAccessLevel.ChatGuide)
        {
            var Members = Frame.Channel.MemberList;

            if (Members != null)
            {
                if (Members.Count > 0)
                {
                    // Dispose of all users first
                    if (Frame.Message.Data.Count >= 2) Reason = Frame.Message.Data[1];

                    for (var x = 0; x < Members.Count; x++)
                        // Channel kill doesnt kill people of same level if they are in the chan
                        if (Frame.User.Level > Members[x].User.Level)
                            // Security is checked in the kill
                            if (ProcessKill(Frame, Members[x].User, Reason))
                                x--;
                    //else
                    //{
                    // Some permissions error
                    //Frame.User.Send(Raws.Create(Server: Frame.Server, Frame.Channel, Frame.User, Raw: Raws.IRCX_ERR_SECURITY_908));
                    //}
                }

                // Only if empty then remove the channel
                if (Members.Count == 0 && Frame.Channel.Modes.Registered.Value != 0x1)
                {
                    // Remove Channel
                    var KillRaw = Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.RPL_KILL_IRC,
                        Data: new[] {Frame.Channel.Name, Reason});
                    Frame.Server.RemoveObject(Frame.Channel);
                    Frame.User.Send(KillRaw);
                }
            }
            else
            {
                Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                    Data: new[] {Resources.Null}));
            }
        }
        else
        {
            //not an operator
            Frame.User.Send(Raws.Create(Frame.Server, Frame.Channel, Frame.User, Raws.IRCX_ERR_NOPRIVILEGES_481));
        }
    }
}