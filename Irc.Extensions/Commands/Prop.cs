using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Interfaces;
using Irc.Extensions.Objects.Channel;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Objects.Server;

namespace Irc.Extensions.Commands;

public class Prop : Command, ICommand
{
    public Prop() : base(2, false) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        //chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
        // Passport hack
        //chatFrame.User.Name = "Sky";
        //chatFrame.User.GetAddress().Nickname = "Sky";
        //chatFrame.User.GetAddress().User = "A65F0CE7D05F0B4E";
        //chatFrame.User.GetAddress().Host = "GateKeeperPassport";
        //chatFrame.User.GetAddress().RealName = "Sky";
        //chatFrame.User.GetAddress().Server = "SERVER";
        //chatFrame.User.Register();
        // Register.Execute(chatFrame);

        // TODO: Resolve object first, e.g. IChatServer.GetObject(string)

        var objectName = chatFrame.Message.Parameters.First();
        if (!chatFrame.User.IsRegistered())
        {
            if (chatFrame.User.IsAuthenticated())
            {
                // Prop $ NICK
                if (objectName == "$")
                {
                    if (chatFrame.Message.Parameters.Count >= 2)
                    {
                        // TODO: This needs rewriting
                        if (string.Compare("NICK", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            chatFrame.User.GetAddress().Nickname = chatFrame.User.Name;
                            SendProps(chatFrame.Server, chatFrame.User, (IExtendedChatObject)chatFrame.User, new string[] { "NICK" });
                        }
                        else if (string.Compare("MSNREGCOOKIE", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            if (chatFrame.Message.Parameters.Count >= 3)
                            {
                                var regcookie = chatFrame.Message.Parameters[2];
                                ((IExtendedServerObject)chatFrame.Server).ProcessCookie(chatFrame.User, "MSNREGCOOKIE", regcookie);
                            }
                        }
                        else if (string.Compare("SUBSCRIBERINFO", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            var subscriberinfo = chatFrame.Message.Parameters[2];
                            ((IExtendedServerObject)chatFrame.Server).ProcessCookie(chatFrame.User, "SUBSCRIBERINFO", subscriberinfo);
                        }
                        else if (string.Compare("MSNPROFILE", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            // TODO: Hook up to actual prop
                            var msnprofile = chatFrame.Message.Parameters[2];
                            ((IExtendedServerObject)chatFrame.Server).ProcessCookie(chatFrame.User, "MSNPROFILE", msnprofile);
                        }
                        else chatFrame.User.Send(Raw.IRCX_ERR_BADPROPERTY_905(chatFrame.Server, chatFrame.User));
                    }
                }
                // PROP $ MSNREGCOOKIE
                // If regcookie is prop'd then no user is required, this fills in the USER info
                // Performs a NICK command
            }
            else
            {
                // You have not authenticated or registered or whatever
            }
        }
        else
        {
            IExtendedChatObject chatObject = null;

            // Lookup object
            if (Channel.ValidName(objectName))
            {
                chatObject = (IExtendedChatObject)chatFrame.Server.GetChannelByName(objectName);
            }
            else if (objectName.Equals(chatFrame.User.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                chatObject = (IExtendedChatObject)chatFrame.User;
            }
            // <$> The $ value is used to indicate the user that originated the request.
            else if (objectName == "$")
            {
                chatObject = (IExtendedChatObject)chatFrame.User;
            }

            if (chatObject == null)
            {
                // No such object
            }
            else
            {
                var props = chatFrame.Message.Parameters[1] == "*" ? chatObject.PropCollection.GetProps() : new List<IPropRule>() { chatObject.PropCollection.GetProp(chatFrame.Message.Parameters[1]) };
                // TODO: Workout null path props[0] == null
                SendProps(chatFrame.Server, chatFrame.User, chatObject, props.Where(x => x != null).Select(x => x.Name).ToArray<string>());
            }
        }
    }

    // TODO: Rewrite this code
    public void SendProps(IServer server, IUser user, IExtendedChatObject targetObject, string[] propNames)
    {
        var propsSent = 0;
        foreach (var propName in propNames)
        {
            var prop = targetObject.PropCollection.GetProp(propName);
            if (prop != null)
            {
                if (prop.ReadAccessLevel == EnumChannelAccessLevel.None)
                {
                    if (propNames.Length == 1) user.Send(Raw.IRCX_ERR_SECURITY_908(server, user));
                    continue;
                }

                if (targetObject is Channel)
                {
                    var kvp = user.GetChannels().FirstOrDefault(x => x.Key == targetObject);
                    if (kvp.Value != null)
                    {
                        var member = kvp.Value;
                        var propValue = prop.GetValue();
                        if (member.GetLevel() >= prop.ReadAccessLevel && !string.IsNullOrEmpty(propValue))
                        {
                            SendProp(server, user, targetObject, prop.Name, propValue);
                            propsSent++;
                        }
                    }
                }
                else SendProp(server, user, targetObject, prop.Name, prop.GetValue()); propsSent++;
            }
            else
            {
                // No such prop
                user.Send(Raw.IRCX_ERR_BADPROPERTY_905(server, user));
            }
        }
        if (propsSent > 0) user.Send(IrcxRaws.IRCX_RPL_PROPEND_819(server, user, targetObject));
    }

    public void SendProp(IServer server, IUser user, IExtendedChatObject targetObject, string propName, string propValue)
    {
        user.Send(IrcxRaws.IRCX_RPL_PROPLIST_818(server, user, targetObject, propName, propValue));
    }
}