using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class AWAY : Command
{
    public AWAY(CommandCode Code) : base(Code)
    {
        RegistrationRequired = true;
        MinParamCount = 0;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters != null)
        {
            string AwayReason;
            AwayReason = Frame.Message.Parameters[0];
            if (AwayReason.Length >= 64) AwayReason = new string(AwayReason.Substring(64));

            Frame.User.Profile.AwayReason = AwayReason;

            Frame.User.Profile.Away = true;
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_NOWAWAY_306));
            Frame.User.BroadcastToChannels(
                RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_USERNOWAWAY_822,
                    Data: new[] {AwayReason}), true);
        }
        else
        {
            //return
            Frame.User.Profile.Away = false;
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_UNAWAY_305));
            Frame.User.BroadcastToChannels(
                RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_USERUNAWAY_821), true);
        }


        return true;
    }
}