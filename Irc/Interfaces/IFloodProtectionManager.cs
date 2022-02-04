using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.IO;

public interface IFloodProtectionManager
{
    EnumFloodResult FloodCheck(EnumCommandDataType type, User user);
    EnumFloodResult Audit(IFloodProtectionProfile protectionProfile, EnumCommandDataType type, EnumUserAccessLevel level);
}