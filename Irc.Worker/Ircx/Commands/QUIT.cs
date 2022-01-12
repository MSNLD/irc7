using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class QUIT : Command
{
    public QUIT(CommandCode Code) : base(Code)
    {
        MinParamCount = 0; // to suppress any warnings
        DataType = CommandDataType.None;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        string Reason = null;
        if (Frame.Message.Data != null) Reason = Frame.Message.Data[0];

        ProcessQuit(Frame.Server, Frame.User, Reason);
        return COM_RESULT.COM_SUCCESS;
    }

    public static void ProcessQuit(Server server, Client client, string Reason)
    {
        if (client.Registered)
            if (client.ObjectType == ObjType.UserObject)
            {
                var user = (User) client;
                if (Reason == null) Reason = Resources.CONNRESETBYPEER;

                var Raw = Raws.Create(Client: user, Raw: Raws.RPL_QUIT_IRC, Data: new[] {Reason});

                for (var c = 0; c < user.ChannelList.Count; c++)
                {
                    var channel = user.ChannelList[c].Channel;
                    channel.RemoveMember(user);
                    channel.Send(Raw, user);
                }

                user.Send(Raw);
            }
        // Broadcast quit to server

        client.Terminate();
        server.RemoveObject(client);
    }
}