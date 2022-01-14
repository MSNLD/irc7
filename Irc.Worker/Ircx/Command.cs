using System;
using System.Collections.Generic;
using System.Reflection;
using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx;

public enum CommandCode
{
    WEBIRC,
    PING,
    PONG,
    AUTH,
    IRCVERS,
    IRCX,
    ISIRCX,
    VERSION,
    NICK,
    USER,
    QUIT,
    CREATE,
    FINDS,
    FINDU,
    KILL,
    GOTO,
    MODE,
    PROP,
    KICK,
    TIME,
    TOPIC,
    PRIVMSG,
    NOTICE,
    WHISPER,
    ACCESS,
    JOIN,
    PART,
    WHO,
    AWAY,
    NAMES,
    WHOIS,
    INVITE,
    LISTX,
    EVENT,
    USERHOST,
    ADMIN,
    INFO,
    PASS,
    SERVER,

    RE
    //, NONE obsoleted?
}

public enum CommandDataType
{
    Data,
    Invitation,
    Join,
    WrongChannelPassword,
    Standard,
    HostMessage,
    None
}

public class CommandCollection
{
    private readonly List<Command> Commands = new();

    public CommandCollection()
    {
        var commandStrings = Enum.GetNames(typeof(CommandCode));
        var commands = Enum.GetValues(typeof(CommandCode));
        for (var c = 0; c < commands.Length; c++)
            try
            {
                var type = Type.GetType("Irc.Worker.Ircx.Commands." + commandStrings[c]);
                var commandPrototype = (Command) Activator.CreateInstance(type, (CommandCode) commands.GetValue(c));
                commandPrototype.Function = type.GetMethod("Execute");
                Commands.Add(commandPrototype);
            }
            catch (Exception e)
            {
                Commands.Add(new Command((CommandCode) commands.GetValue(c)));
            }
    }

    public object GetCommand(string Name)
    {
        for (var c = 0; c < Commands.Count; c++)
            if (Commands[c].Name == Name)
                return Commands[c];
        return null;
    }
}

public class Command
{
    private CommandCode Code;
    public CommandDataType DataType;
    public bool ForceFloodCheck;
    public MethodInfo Function;
    public int MinParamCount;
    public string Name;
    public bool PreRegistration;
    public bool RegistrationRequired;

    public Command(CommandCode Code)
    {
        this.Code = Code;
        Name = Code.ToString();
        DataType = CommandDataType.None;
    }

    public bool fldchk(User u)
    {
        if (Flood.FloodCheck(DataType, u) == FLD_RESULT.S_WAIT)
            return false;
        return true;
    }

    public bool Execute(Frame Frame)
    {
        if (ForceFloodCheck)
            if (!fldchk(Frame.User))
                return false;

        if (Function != null)
        {
            if (PreRegistration && !Frame.User.Registered || Frame.User.Registered && RegistrationRequired ||
                !PreRegistration && !RegistrationRequired)
            {
                if (MinParamCount > 0)
                {
                    if (Frame.Message.Parameters != null)
                        if (Frame.Message.Parameters.Count >= MinParamCount)
                            return (bool) Function.Invoke(Frame.Command, new object[] {Frame});
                }
                else
                {
                    return (bool) Function.Invoke(Frame.Command, new object[] {Frame});
                }

                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                    Data: new[] {Frame.Message.GetCommand()}));
            }
            else if (Frame.User.Registered && PreRegistration)
            {
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                    Raw: Raws.IRCX_ERR_ALREADYREGISTERED_462));
            }
            else
            {
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTREGISTERED_451));
            }
        }
        else
        {
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421,
                Data: new[] {Frame.Message.GetCommand() }));
            //Frame.User.Send(DefaultFrames.NotImplemented);
        }

        //not implemented to irc user
        return true;
    }
}