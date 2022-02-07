using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IChannelMember : IMemberModes
{
    EnumUserAccessLevel GetLevel();
    IUser GetUser();
}