using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface IFloodProtectionManager
{
    EnumFloodResult FloodCheck(EnumCommandDataType type, IUser user);

    EnumFloodResult Audit(IFloodProtectionProfile protectionProfile, EnumCommandDataType type,
        EnumUserAccessLevel level);
}