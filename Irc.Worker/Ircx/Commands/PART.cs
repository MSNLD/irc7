using System.Linq;
using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class PART : Command
{
    public PART(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        RegistrationRequired = true;
        DataType = CommandDataType.Standard;
    }

    public static void ProcessPartChannel(Frame Frame, Channel channel)
    {
        var channelMember = channel.Members.FirstOrDefault(member => member.User == Frame.User);
        if (channelMember != null)
        {
            channel.Send(RawBuilder.Create(Frame.Server, channel, Frame.User, Raws.RPL_PART_IRC), Frame.User);
            channel.Members.Remove(channelMember);

            Frame.User.RemoveChannel(channel);
        }
        else
        {
            // you are not on that channel
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                Data: new[] {channel.Name}));
        }
    }

    public static void ProcessPartUserChannels(Frame Frame)
    {
        foreach (var channelMemberPair in Frame.User.Channels)
        {
            ProcessPartChannel(Frame, channelMemberPair.Key);
        }
    }

    public new bool Execute(Frame Frame)
    {
        var message = Frame.Message;

        if (message.Parameters.Count >= 1)
        {
            // To process 0
            if (message.Parameters[0] == Resources.Zero && message.Parameters.Count == 1)
            {
                ProcessPartUserChannels(Frame);
                return true;
            }

            var channels = Common.GetChannels(Frame.Server, Frame.User, message.Parameters[0], true);

            if (channels != null)
                if (channels.Count > 0)
                {
                    for (var c = 0; c < channels.Count; c++) ProcessPartChannel(Frame, channels[c]);
                    return true;
                }

            // null
            // No such channel
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                Data: new[] {Frame.Message.Parameters[0]}));
        }
        else
        {
            //insufficient parameters
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                Data: new[] {message.Parameters[0]}));
        }

        return true;
    }
}