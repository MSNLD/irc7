using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

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
        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERCLIENT_251,
            IData: new[] {server.RegisteredUsers, server.InvisibleCount, 1}));
        user.Send(
            Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSEROP_252, IData: new[] {server.OperatorCount}));
        if (server.UnknownConnections > 0)
            user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERUNKNOWN_253,
                IData: new[] {server.UnknownConnections}));
        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERCHANNELS_254,
            IData: new[] {server.Channels.Length}));
        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERME_255,
            IData: new[] {server.RegisteredUsers, 0}));
        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_LUSERS_265,
            IData: new[] {server.RegisteredUsers, server.MaxUsers}));
        user.Send(Raws.Create(server, Client: user, Raw: Raws.IRCX_RPL_GUSERS_266,
            IData: new[] {server.RegisteredUsers, server.MaxUsers}));
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        SendLusers(Frame.Server, Frame.User);
        return COM_RESULT.COM_SUCCESS;
    }
}