using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class LUSERS : Command
    {

        public LUSERS(CommandCode Code) : base(Code)
        {
            base.RegistrationRequired = true;
            base.MinParamCount = 1;
            base.DataType = CommandDataType.Data;
            base.ForceFloodCheck = true;
        }

        public static void SendLusers(Server server, User user)
        {
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LUSERCLIENT_251, IData: new int[] { server.RegisteredUsers, server.InvisibleCount, 1 }));
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LUSEROP_252, IData: new int[] { server.OperatorCount }));
            if (server.UnknownConnections > 0) { user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LUSERUNKNOWN_253, IData: new int[] { server.UnknownConnections })); }
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LUSERCHANNELS_254, IData: new int[] { server.Channels.Length }));
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LUSERME_255, IData: new int[] { server.RegisteredUsers, 0 }));
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LUSERS_265, IData: new int[] { server.RegisteredUsers, server.MaxUsers }));
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_GUSERS_266, IData: new int[] { server.RegisteredUsers, server.MaxUsers }));
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            SendLusers(Frame.Server, Frame.User);
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
