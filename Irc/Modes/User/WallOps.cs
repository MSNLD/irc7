using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Modes.User;

public class WallOps : ModeRule, IModeRule
{
    public WallOps() : base(Resources.UserModeWallops)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}