using Core.Ircx.Objects;
using CSharpTools;

namespace Core.Ircx.Commands;

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

    public new COM_RESULT Execute(Frame Frame)
    {
        // Find Nicknames and output as follows
        // IRCX_RPL_REVEAL_852 = ":SERVER 851 Sky Nickname Address IP OID :%s"

        if (Frame.User.Level < UserAccessLevel.ChatGuide)
        {
            //no such command
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421,
                Data: new[] {Frame.Message.Command}));
            return COM_RESULT.COM_SUCCESS;
        }

        if (Frame.Message.Data.Count == 0)
        {
            //nothing
        }
        else
        {
            for (var i = 0; i < Frame.Server.Users.Length; i++)
            {
                var User = Frame.Server.Users[i];
                if (User.Registered)
                    if (StringBuilderRegEx.EvaluateString(Frame.Message.Data[0], User.Address.Nickname, true))
                    {
                        if (User.ChannelList.Count == 0)
                            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_REVEAL_851,
                                Data: new[]
                                {
                                    User.Address.Nickname, User.OIDX8, User.Address.RemoteIP, User.Address._address[1],
                                    Resources.Null
                                }));
                        else
                            for (var x = 0; x < User.ChannelList.Count; x++)
                                Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                                    Raw: Raws.IRCX_RPL_REVEAL_851,
                                    Data: new[]
                                    {
                                        User.Address.Nickname, User.OIDX8, User.Address.RemoteIP,
                                        User.Address._address[1], User.ChannelList[x].Channel.Name
                                    }));
                    }
            }
        }


        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_REVEALEND_852));
        return COM_RESULT.COM_SUCCESS;
    }
}