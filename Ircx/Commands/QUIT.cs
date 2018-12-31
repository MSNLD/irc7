using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    public class QUIT : Command
    {
        public QUIT(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 0; // to suppress any warnings
            base.DataType = CommandDataType.None;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            String8 Reason = null;
            if (Frame.Message.Data != null)
            {
                Reason = Frame.Message.Data[0];
            }

            ProcessQuit(Frame.Server, Frame.User, Reason);
            return COM_RESULT.COM_SUCCESS;
        }

        public static void ProcessQuit(Server server, Client client, String8 Reason)
        {
            if (client.Registered) { 
                if (client.ObjectType == ObjType.UserObject) {
                    User user = (User)client;
                    if (Reason == null) { Reason = Resources.CONNRESETBYPEER; }

                    String8 Raw = Raws.Create(Client: user, Raw: Raws.RPL_QUIT_IRC, Data: new String8[] { Reason });

                    for (int c = 0; c < user.ChannelList.Count; c++)
                    {
                        Channel channel = user.ChannelList[c].Channel;
                        channel.RemoveMember(user);
                        channel.Send(Raw, user);
                    }
                    user.Send(Raw);
                }
            }
            // Broadcast quit to server

            client.Terminate();
            server.RemoveObject(client);
            return;
        }
    }
}
