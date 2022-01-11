using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;
using Core.Ircx.Objects;
using Core.Ircx.Commands;
using System.Reflection;

namespace Core.Ircx
{

    public enum CommandCode
    {
        WEBIRC, PING, PONG, AUTH, IRCVERS, IRCX, ISIRCX, VERSION, NICK, USER, QUIT, CREATE, FINDS, FINDU, KILL, GOTO,
        MODE, PROP, KICK, TIME, TOPIC, PRIVMSG, NOTICE, WHISPER, ACCESS, JOIN, PART, WHO, AWAY, NAMES, WHOIS, INVITE,
        LISTX, EVENT, USERHOST, ADMIN, INFO, PASS, SERVER, RE
            //, NONE obsoleted?
    };
    public enum CommandDataType
    {
        Data, Invitation, Join, WrongChannelPassword, Standard, HostMessage, None
    };
    public enum COM_RESULT
    {
        COM_SUCCESS = 0, COM_ERR = -1, COM_WAIT = -2
    }

    public class CommandCollection
    {
        List<Command> Commands = new List<Command>();

        public CommandCollection()
        {
            string[] commandStrings = Enum.GetNames(typeof(CommandCode));
            Array commands = Enum.GetValues(typeof(CommandCode));
            for (int c = 0; c < commands.Length; c++)
            {
                try
                {
                    Type type = Type.GetType("Core.Ircx.Commands." + commandStrings[c]);
                    Command commandPrototype = (Command)Activator.CreateInstance(type, (CommandCode)commands.GetValue(c));
                    commandPrototype.Function = type.GetMethod("Execute");
                    Commands.Add(commandPrototype);
                }
                catch (Exception e) { Commands.Add(new Command((CommandCode)commands.GetValue(c))); }               
            }
        }

        public object GetCommand(string Name)
        {
            for (int c = 0; c < Commands.Count; c++)
            {
                if (((Command)Commands[c]).Name == Name) { return Commands[c]; }
            }
            return null;
        }
    }

    public class Command
    {

        CommandCode Code;
        public string Name;
        public MethodInfo Function;
        public int MinParamCount;
        public bool RegistrationRequired;
        public bool PreRegistration;
        public CommandDataType DataType;
        public bool ForceFloodCheck;

        public Command(CommandCode Code)
        {
            this.Code = Code;
            Name = Code.ToString();
            DataType = CommandDataType.None;
        }

        public bool fldchk(User u)
        {
            if (Flood.FloodCheck(DataType, u) == FLD_RESULT.S_WAIT) { return false; }
            else { return true; }
        }

        public COM_RESULT Execute(Frame Frame)
        {
            if (ForceFloodCheck) { if (!fldchk(Frame.User)) { return COM_RESULT.COM_WAIT; } }

            if (Function != null)
            {
                if (((PreRegistration) && (!Frame.User.Registered)) || ((Frame.User.Registered) && (RegistrationRequired)) || ((!PreRegistration) && (!RegistrationRequired)))
                {
                    if (MinParamCount > 0)
                    {
                        if (Frame.Message.Data != null)
                        {
                            if (Frame.Message.Data.Count >= MinParamCount)
                            {
                                return (COM_RESULT)Function.Invoke(Frame.Command, new object[] { Frame });
                            }
                        }
                    }
                    else { return (COM_RESULT)Function.Invoke(Frame.Command, new object[] { Frame }); }

                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461, Data: new string[] { Frame.Message.Command }));
                }
                else if ((Frame.User.Registered) && (PreRegistration))
                {
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ALREADYREGISTERED_462));
                }
                else
                {
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTREGISTERED_451));
                }
            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421, Data: new string[] { Frame.Message.Command }));
                //Frame.User.Send(DefaultFrames.NotImplemented);
            }
            //not implemented to irc user
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
