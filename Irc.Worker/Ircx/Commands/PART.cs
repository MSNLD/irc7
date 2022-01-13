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

    public static void ProcessPartChannel(Frame Frame, Channel c)
    {
        if (c.Contains(Frame.User))
        {
            c.Send(RawBuilder.Create(Frame.Server, c, Frame.User, Raws.RPL_PART_IRC), Frame.User);
            c.RemoveMember(Frame.User);

            Frame.User.RemoveChannel(c);
        }
        else
        {
            // you are not on that channel
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                Data: new[] {c.Name}));
        }
    }

    public static void ProcessPartUserChannels(Frame Frame)
    {
        for (var c = 0; c < Frame.User.ChannelList.Count; c++)
        {
            ProcessPartChannel(Frame, Frame.User.ChannelList[c].Channel);
            c--;
        }
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        var message = Frame.Message;

        if (message.Data.Count >= 1)
        {
            // To process 0
            if (message.Data[0] == Resources.Zero && message.Data.Count == 1)
            {
                ProcessPartUserChannels(Frame);
                return COM_RESULT.COM_SUCCESS;
            }

            var channels = Common.GetChannels(Frame.Server, Frame.User, message.Data[0], true);

            if (channels != null)
                if (channels.Count > 0)
                {
                    for (var c = 0; c < channels.Count; c++) ProcessPartChannel(Frame, channels[c]);
                    return COM_RESULT.COM_SUCCESS;
                }

            // null
            // No such channel
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                Data: new[] {Frame.Message.Data[0]}));
        }
        else
        {
            //insufficient parameters
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                Data: new[] {message.Data[0]}));
        }

        return COM_RESULT.COM_SUCCESS;
    }
}