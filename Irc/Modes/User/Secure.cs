using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Modes.User;

public class Secure : ModeRule, IModeRule
{
    public Secure() : base(Resources.UserModeSecure)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.ERR_NOPERMS;
    }
}