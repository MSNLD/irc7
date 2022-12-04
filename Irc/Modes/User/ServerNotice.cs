using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Modes.User;

public class ServerNotice : ModeRule, IModeRule
{
    public ServerNotice() : base(Resources.UserModeServerNotice)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}