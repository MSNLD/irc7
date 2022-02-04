using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Worker.Ircx.Objects;

public interface IChannelMember
{
    EnumUserAccessLevel GetLevel();
    void SetLevel(EnumUserAccessLevel level);
    IChatMemberModes GetChanUserMode();
    User GetUser();
}