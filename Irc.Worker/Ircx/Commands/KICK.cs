using System.Linq;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
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

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters.Count >= 2)
        {
            //kick # nick,[...] :reason
            var objType = IrcHelper.IdentifyObject(Frame.Message.Parameters[0]);
            var c = Frame.Server.Channels.FindObj(Frame.Message.Parameters[0], objType);
            if (c != null)
            {
                var channelMemberPair = Frame.User.GetChannelInfo(Frame.Message.Parameters[0]);
                var channel = channelMemberPair.Key;
                var channelMember = channelMemberPair.Value;
                if (channel != null)
                {
                    if (channelMember.Level >= UserAccessLevel.ChatHost)
                    {
                        // TODO: Fix below to report users that do not exist
                        var memberList = Tools.CSVToArray(Frame.Message.Parameters[1].ToUpper());
                        var members = c.Members.Where(member => memberList.Contains(member.User.Name.ToUpper())).ToList();

                        if (members.Count > 0)
                        {
                            var Reason = string.Empty;
                            if (Frame.Message.Parameters.Count >= 3) Reason = Frame.Message.Parameters[2];

                            for (var x = 0; x < members.Count; x++)
                                ProcessKick(Frame.Server, channelMember, c, members[x], Reason);
                        }
                        else
                        {
                            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                                Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new[] { string.Empty }));
                        }
                    }
                    else
                    {
                        //not an operator
                        Frame.User.Send(RawBuilder.Create(Frame.Server, c, Frame.User, Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
                    }
                }
                else
                {
                    //you're not on that channel
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                        Data: new[] {Frame.Message.Parameters[0]}));
                }
            }
            else
            {
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                    Data: new[] {Frame.Message.Parameters[0]}));
                //no such channel
            }
        }
        else
        {
            //insufficient parameters
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                Data: new[] {Frame.Message.GetCommand() }));
        }

        return true;
    }

    public void ProcessKick(Server server, ChannelMember Member, Channel channel, ChannelMember ChannelMember,
        string Reason)
    {
        if (Member.Level < ChannelMember.Level)
        {
            if (ChannelMember.Level >= UserAccessLevel.ChatGuide) /* you're not ircop */
                Member.User.Send(RawBuilder.Create(server, Client: Member.User, Raw: Raws.IRCX_ERR_NOPRIVILEGES_481));
            else /* you're not an operator 482 */
                Member.User.Send(RawBuilder.Create(server, channel, Member.User, Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
        }
        else
        {
            channel.Send(
                RawBuilder.Create(server, channel, Member.User, Raws.RPL_KICK_IRC,
                    new[] {ChannelMember.User.Address.Nickname, Reason}), ChannelMember.User);
            ChannelMember.ChannelMode.SetNormal();
            channel.Members.Remove(ChannelMember);
            ChannelMember.User.RemoveChannel(channel);
        }
    }
}