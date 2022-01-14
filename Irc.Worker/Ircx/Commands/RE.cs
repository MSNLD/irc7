using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class RE : Command
{
    // Something special we cooked up called Reveal for admins
    public RE(CommandCode Code) : base(Code)
    {
        MinParamCount = 0;
        DataType = CommandDataType.Data;
        RegistrationRequired = true;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        // Find Nicknames and output as follows
        // IRCX_RPL_REVEAL_852 = ":SERVER 851 Sky Nickname Address IP OID :%s"

        if (Frame.User.Level < UserAccessLevel.ChatGuide)
        {
            //no such command
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421,
                Data: new[] {Frame.Message.GetCommand() }));
            return true;
        }

        if (Frame.Message.Parameters.Count == 0)
        {
            //nothing
        }
        else
        {
            for (var i = 0; i < Frame.Server.Users.Count; i++)
            {
                var User = Frame.Server.Users[i];
                if (User.Registered)
                    if (StringBuilderRegEx.EvaluateString(Frame.Message.Parameters[0], User.Address.Nickname, true))
                    {
                        if (User.Channels.Count == 0)
                            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_REVEAL_851,
                                Data: new[]
                                {
                                    User.Address.Nickname, User.Id.ToString(), User.RemoteIP, User.Address.GetUserHost(),
                                    string.Empty
                                }));
                        else
                            foreach (var channelMemberPair in User.Channels)
                            {
                                var channel = channelMemberPair.Key;
                                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                                    Raw: Raws.IRCX_RPL_REVEAL_851,
                                    Data: new[]
                                    {
                                        User.Address.Nickname, User.Id.ToString(), User.RemoteIP,
                                        User.Address.GetUserHost(), channel.Name
                                    }));
                            }
                    }
            }
        }


        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_REVEALEND_852));
        return true;
    }
}