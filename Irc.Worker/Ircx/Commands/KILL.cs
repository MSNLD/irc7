using System.Linq;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class KILL : Command
{
    public KILL(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.None;
        RegistrationRequired = true;
        PreRegistration = false;
    }

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters.Count >= 1)
        {
            var objs = Frame.Server.GetObjects(Frame.Message.Parameters[0]);

            if (objs.Count > 0)
            {
                // Supports KILL OID1,OID2,OID3 Reason
                // Can mix user / chan

                var Reason = string.Empty;
                if (Frame.Message.Parameters.Count > 1) Reason = Frame.Message.Parameters[1];

                for (var i = 0; i < objs.Count; i++)
                    //Determine type
                    //Kill with / without reason

                    if (objs[i] is Channel)
                    {
                        Frame.Channel = (Channel) objs[i];
                        ProcessChannelKill(Frame, Reason);
                    }
                    else if (objs[i] is User)
                    {
                        var TargetUser = (User) objs[i];
                        ProcessKill(Frame, TargetUser, Reason);
                    }
            }
        }

        return true;
    }

    public bool ProcessKill(Frame Frame, User TargetUser, string Reason)
    {
        if (Frame.User.Level >= UserAccessLevel.ChatGuide && Frame.User.Level >= TargetUser.Level)
        {
            var channels = TargetUser.Channels;

            var KillRaw = RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.RPL_KILL_IRC,
                Data: new[] {TargetUser.Address.Nickname, Reason});

            foreach (var channelMemberPair in channels)
            {
                var channel = channelMemberPair.Key;
                //TargetUser.ChannelMode.SetNormal();
                // Need to reset usermodes
                var member = channelMemberPair.Value;
                channel.Members.Remove(member);
                TargetUser.RemoveChannel(channel);

                //Broadcast to channel after user has been removed
                channel.Send(KillRaw, Frame.User, true);
            }

            if (Frame.User != TargetUser) Frame.User.Send(KillRaw);
            TargetUser.Send(KillRaw);
            TargetUser.Terminate();
            return true;
        }

        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_SECURITY_908));
        return false;
    }

    public void ProcessChannelKill(Frame Frame, string Reason)
    {
        if (Frame.User.Level >= UserAccessLevel.ChatGuide)
        {
            var Members = Frame.Channel.Members;

            if (Members != null)
            {
                if (Members.Count > 0)
                {
                    // Dispose of all users first
                    if (Frame.Message.Parameters.Count >= 2) Reason = Frame.Message.Parameters[1];

                    for (var x = 0; x < Members.Count; x++)
                        // Channel kill doesnt kill people of same level if they are in the chan
                        if (Frame.User.Level > Members[x].User.Level)
                            // Security is checked in the kill
                            if (ProcessKill(Frame, Members[x].User, Reason))
                                x--;
                    //else
                    //{
                    // Some permissions error
                    //Frame.User.Send(RawBuilder.Create(Server: Frame.Server, Frame.Channel, Frame.User, Raw: Raws.IRCX_ERR_SECURITY_908));
                    //}
                }

                // Only if empty then remove the channel
                if (Members.Count == 0 && Frame.Channel.Modes.Registered.Value != 0x1)
                {
                    // Remove Channel
                    var KillRaw = RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.RPL_KILL_IRC,
                        Data: new[] {Frame.Channel.Name, Reason});
                    Frame.Server.RemoveChannel(Frame.Channel);
                    Frame.User.Send(KillRaw);
                }
            }
            else
            {
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                    Data: new[] {string.Empty}));
            }
        }
        else
        {
            //not an operator
            Frame.User.Send(RawBuilder.Create(Frame.Server, Frame.Channel, Frame.User, Raws.IRCX_ERR_NOPRIVILEGES_481));
        }
    }
}