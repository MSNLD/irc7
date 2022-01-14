using System.ComponentModel.DataAnnotations;
using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class QUIT : Command
{
    public QUIT(CommandCode Code) : base(Code)
    {
        MinParamCount = 0; // to suppress any warnings
        DataType = CommandDataType.None;
    }

    public new bool Execute(Frame Frame)
    {
        string Reason = null;
        if (Frame.Message.Parameters != null) Reason = Frame.Message.Parameters[0];

        ProcessQuit(Frame.Server, Frame.User, Reason);
        return true;
    }

    public static void ProcessQuit(Server server, Client client, string Reason)
    {
        if (client.Registered)
            if (client is User)
            {
                var user = (User) client;
                if (Reason == null) Reason = Resources.CONNRESETBYPEER;

                var Raw = RawBuilder.Create(Client: user, Raw: Raws.RPL_QUIT_IRC, Data: new[] { Reason });

                foreach (var channelMemberPair in user.Channels)
                {
                    var channel = channelMemberPair.Key;
                    var member = channelMemberPair.Value;
                    channel.Members.Remove(member);
                    channel.Send(Raw, user);
                }

                user.Send(Raw);
            }
        // Broadcast quit to server

        client.Terminate();
        server.RemoveUser(client as User);
    }
}