using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class AWAY : Command
    {

        public AWAY(CommandCode Code) : base(Code)
        {
            base.RegistrationRequired = true;
            base.MinParamCount = 0;
            base.DataType = CommandDataType.Data;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data != null)
            {
                string AwayReason;
                AwayReason = Frame.Message.Data[0];
                if (AwayReason.Length >= 64) { AwayReason = new string(AwayReason.ToString().Substring(64)); }

                Frame.User.Profile.AwayReason = AwayReason.ToString();

                Frame.User.Profile.Away = true;
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_NOWAWAY_306));
                Frame.User.BroadcastToChannels(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_USERNOWAWAY_822, Data: new string[] { AwayReason } ), true);
            }
            else
            {
                //return
                Frame.User.Profile.Away = false;
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_UNAWAY_305));
                Frame.User.BroadcastToChannels(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_USERUNAWAY_821), true);
            }


            return COM_RESULT.COM_SUCCESS;
        }
    }
}
