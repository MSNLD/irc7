using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class RE : Command
    {
        // Something special we cooked up called Reveal for admins
        public RE(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 0;
            base.DataType = CommandDataType.Data;
            base.RegistrationRequired = true;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            // Find Nicknames and output as follows
            // IRCX_RPL_REVEAL_852 = ":SERVER 851 Sky Nickname Address IP OID :%s"

            if (Frame.User.Level < UserAccessLevel.ChatGuide)
            {
                //no such command
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421, Data: new string[] { Frame.Message.Command }));
                return COM_RESULT.COM_SUCCESS;
            }
            else if (Frame.Message.Data.Count == 0)
            {
                //nothing
            }
            else
            {
                for (int i = 0; i < Frame.Server.Users.Length; i++)
                {
                    User User = Frame.Server.Users[i];
                    if (User.Registered)
                    {
                        if (StringBuilderRegEx.EvaluateString(Frame.Message.Data[0].ToString(), User.Address.Nickname.ToString(), true))
                        {
                            if (User.ChannelList.Count == 0)
                            {
                                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_REVEAL_851, Data: new string[] { User.Address.Nickname, User.OIDX8, User.Address.RemoteIP, User.Address._address[1], Resources.Null }));
                            }
                            else
                            {
                                for (int x = 0; x < User.ChannelList.Count; x++)
                                {
                                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_REVEAL_851, Data: new string[] { User.Address.Nickname, User.OIDX8, User.Address.RemoteIP, User.Address._address[1], User.ChannelList[x].Channel.Name }));
                                }
                            }
                        }
                    }
                }
            }



            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_REVEALEND_852));
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
