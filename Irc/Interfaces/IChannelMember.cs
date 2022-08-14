using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IChannelMember : IMemberModes
{
    EnumChannelAccessLevel GetLevel();
    IUser GetUser();
    EnumIrcError CanModify(IChannelMember target, EnumChannelAccessLevel requiredLevel);
}