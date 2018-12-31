using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class IRCVERS : Command
    {
        /// <summary>
        /// IRCVERS IRC8 MSN-OCX!9.02.0310.2401
        /// </summary>
        /// <param name="Code"></param>
        public IRCVERS(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 2;
            base.DataType = CommandDataType.None;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (!Frame.User.Registered)
            {
                if (Frame.Message.Data[0].length == 4)
                {
                    if (!String8.compare(Frame.Message.Data[0], Resources.IRC, 3))
                    {
                        if ((Frame.Message.Data[0].bytes[3] >= 48) && (Frame.Message.Data[0].bytes[3] <= 57))
                        {
                            Frame.User.Properties.Ircvers.Value = Frame.Message.Data[0];
                            Frame.User.Modes.Ircx.Value = 1;
                            Frame.User.Profile.Ircvers = (byte)(Frame.Message.Data[0].bytes[3] - 48);
                            Frame.User.Properties.Client.Value = Frame.Message.Data[1];

                            IRCX.ProcessIRCXReply(Frame);

                            return COM_RESULT.COM_SUCCESS;
                        }
                    }
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_BADVALUE_906, Data: new String8[] { Frame.Message.Data[0] }));
                }
            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ALREADYREGISTERED_462));
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
