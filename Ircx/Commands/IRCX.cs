using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class IRCX : Command
    {

        //         800 - IRCRPL_IRCX

        // <state> <version> <package-list> <maxmsg> <option-list>

        // The response to the IRCX and ISIRCX commands.The<state>
        // indicates if the client has IRCX mode enabled (0 for disabled,
        // 1 for enabled).  The<version> is the version of the IRCX
        //protocol starting at 0.   The<package-list> contains a list
        // of authentication packages supported by the server.The
        // package name of "ANON" is reserved to indicate that anonymous
        // connections are permitted.The<maxmsg> defines the maximum
        // message size permitted, with the standard being 512. The
        // <option-list> contains a list of options supported by the
        // server; these are implementation-dependent.If no options are
        // available, the  '*'  character is used.
        public IRCX(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 0;
            base.DataType = CommandDataType.Standard;
            base.ForceFloodCheck = true;
        }

        public static void ProcessIRCXReply(Frame Frame)
        {
            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_IRCX_800, Data: new string[] { Core.Authentication.SSP.SupportedPackages, Resources.IRCXOptions }, IData: new int[] { Frame.User.Modes.Ircx.Value, Frame.Server.IrcxVersion, Program.Config.BufferSize }));
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Frame.User.Modes.Ircx.Value = 1;
            ProcessIRCXReply(Frame);
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
