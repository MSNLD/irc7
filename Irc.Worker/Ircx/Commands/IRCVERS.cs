using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class IRCVERS : Command
{
    /// <summary>
    ///     IRCVERS IRC8 MSN-OCX!9.02.0310.2401
    /// </summary>
    /// <param name="Code"></param>
    public IRCVERS(CommandCode Code) : base(Code)
    {
        MinParamCount = 2;
        DataType = CommandDataType.None;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        if (!Frame.User.Registered)
        {
            if (Frame.Message.Parameters[0].Length == 4)
            {
                if (Frame.Message.Parameters[0].StartsWith(Resources.IRC))
                    if (Frame.Message.Parameters[0][3] >= 48 && Frame.Message.Parameters[0][3] <= 57)
                    {
                        Frame.User.Properties.Set("Ircvers", Frame.Message.Parameters[0]);
                        Frame.User.Modes.Ircx.Value = 1;
                        Frame.User.Profile.Ircvers = (byte) (Frame.Message.Parameters[0][3] - 48);
                        Frame.User.Properties.Set("Client", Frame.Message.Parameters[1]);

                        IRCX.ProcessIRCXReply(Frame);

                        return true;
                    }

                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADVALUE_906,
                    Data: new[] {Frame.Message.Parameters[0]}));
            }
        }
        else
        {
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ALREADYREGISTERED_462));
        }

        return true;
    }
}