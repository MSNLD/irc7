using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class LUSERS : Command
{
    public LUSERS(CommandCode Code) : base(Code)
    {
        RegistrationRequired = true;
        MinParamCount = 1;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public static void SendLusers(Server server, User user)
    {
        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERCLIENT_251,
            IData: new[] {server.ServerFields.RegisteredUsers, server.ServerFields.InvisibleCount, 1}));
        user.Send(
            RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSEROP_252, IData: new[] {server.ServerFields.OperatorCount}));
        if (server.ServerFields.UnknownConnections > 0)
            user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERUNKNOWN_253,
                IData: new[] {server.ServerFields.UnknownConnections}));
        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERCHANNELS_254,
            IData: new[] {server.Channels.Length}));
        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERME_255,
            IData: new[] {server.ServerFields.RegisteredUsers, 0}));
        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERS_265,
            IData: new[] {server.ServerFields.RegisteredUsers, server.ServerFields.MaxUsers}));
        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_GUSERS_266,
            IData: new[] {server.ServerFields.RegisteredUsers, server.ServerFields.MaxUsers}));
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        SendLusers(Frame.Server, Frame.User);
        return COM_RESULT.COM_SUCCESS;
    }
}