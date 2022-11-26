using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Interfaces;
using Irc.Extensions.Objects.Channel;
using Irc.Extensions.Objects.User;
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
                            SendProp(chatFrame.Server, chatFrame.User, (IExtendedChatObject)chatFrame.User, "NICK", chatFrame.User.Name);
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
                        else chatFrame.User.Send(Raw.IRCX_ERR_BADPROPERTY_905(chatFrame.Server, chatFrame.User, chatFrame.Message.Parameters[1]));
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

            // <$> The $ value is used to indicate the user that originated the request.
            if (objectName == "$")
            {
                chatObject = (IExtendedChatObject)chatFrame.User;
            }
            else
            {
                chatObject = (IExtendedChatObject)chatFrame.Server.GetChatObject(objectName);
            }

            if (chatObject == null)
            {
                // No such object
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHOBJECT_924(chatFrame.Server, chatFrame.User, objectName));
            }
            else
            {
                var props = new List<IPropRule>();
                if (chatFrame.Message.Parameters[1] == "*")
                {
                    props.AddRange(chatObject.PropCollection.GetProps());
                }
                else
                {
                    var prop = chatObject.PropCollection.GetProp(chatFrame.Message.Parameters[1]);
                    if (prop != null)
                    {
                        props.Add(prop);
                    }
                    else
                    {
                        // Bad prop
                        chatFrame.User.Send(Raw.IRCX_ERR_BADPROPERTY_905(chatFrame.Server, chatFrame.User, objectName));
                    }
                }

                if (props.Count > 0)
                {
                    SendProps(chatFrame.Server, chatFrame.User, chatObject, props);
                }
            }
        }
    }

    // TODO: Rewrite this code
    public void SendProps(IServer server, IUser user, IExtendedChatObject targetObject, List<IPropRule> props)
    {
        var propsSent = 0;
        foreach (var prop in props)
        {
            if (prop.ReadAccessLevel == EnumChannelAccessLevel.None)
            {
                if (props.Count == 1) user.Send(Raw.IRCX_ERR_SECURITY_908(server, user));
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
        if (propsSent > 0)
        {
            user.Send(IrcxRaws.IRCX_RPL_PROPEND_819(server, user, targetObject));
        }
    }

    public void SendProp(IServer server, IUser user, IExtendedChatObject targetObject, string propName, string propValue)
    {
        user.Send(IrcxRaws.IRCX_RPL_PROPLIST_818(server, user, targetObject, propName, propValue));
    }
}