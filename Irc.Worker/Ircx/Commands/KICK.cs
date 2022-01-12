using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class KICK : Command
{
    public KICK(CommandCode Code) : base(Code)
    {
        MinParamCount = 2;
        RegistrationRequired = true;
        DataType = CommandDataType.Standard;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        if (Frame.Message.Data.Count >= 2)
        {
            //kick # nick,[...] :reason
            var c = Frame.Server.Channels.GetChannel(Frame.Message.Data[0]);
            if (c != null)
            {
                var uci = Frame.User.GetChannelInfo(c);
                if (uci != null)
                {
                    if (uci.Member.Level >= UserAccessLevel.ChatHost)
                    {
                        var Members = c.Members.GetMembers(Frame.Server, c, Frame.User, Frame.Message.Data[1], true);

                        if (Members != null)
                        {
                            if (Members.Count > 0)
                            {
                                var Reason = Resources.Null;
                                if (Frame.Message.Data.Count >= 3) Reason = Frame.Message.Data[2];

                                for (var x = 0; x < Members.Count; x++)
                                    ProcessKick(Frame.Server, uci.Member, c, Members[x], Reason);
                            }
                            else
                            {
                                Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                                    Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new[] {Resources.Null}));
                            }
                        }
                        else
                        {
                            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                                Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new[] {Resources.Null}));
                        }
                    }
                    else
                    {
                        //not an operator
                        Frame.User.Send(Raws.Create(Frame.Server, c, Frame.User, Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
                    }
                }
                else
                {
                    //you're not on that channel
                    Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                        Data: new[] {Frame.Message.Data[0]}));
                }
            }
            else
            {
                Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                    Data: new[] {Frame.Message.Data[0]}));
                //no such channel
            }
        }
        else
        {
            //insufficient parameters
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                Data: new[] {Frame.Message.Command}));
        }

        return COM_RESULT.COM_SUCCESS;
    }

    public void ProcessKick(Server server, ChannelMember Member, Channel channel, ChannelMember ChannelMember,
        string Reason)
    {
        if (Member.Level < ChannelMember.Level)
        {
            if (ChannelMember.Level >= UserAccessLevel.ChatGuide) /* you're not ircop */
                Member.User.Send(Raws.Create(server, Client: Member.User, Raw: Raws.IRCX_ERR_NOPRIVILEGES_481));
            else /* you're not an operator 482 */
                Member.User.Send(Raws.Create(server, channel, Member.User, Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
        }
        else
        {
            channel.Send(
                Raws.Create(server, channel, Member.User, Raws.RPL_KICK_IRC,
                    new[] {ChannelMember.User.Address.Nickname, Reason}), ChannelMember.User);
            ChannelMember.ChannelMode.SetNormal();
            channel.RemoveMember(ChannelMember);
            ChannelMember.User.RemoveChannel(channel);
        }
    }
}