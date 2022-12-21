using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface IFloodProtectionLevel
{
    bool CanProcess { get; }
    bool CanProcessData { get; }
    bool CanProcessInvite { get; }
    bool CanProcessJoin { get; }
    bool CanProcessPassword { get; }
    bool CanProcessStandard { get; }
    bool CanProcessHostMessage { get; }
    void SetProtectionLevel(EnumFloodProtectionLevel level);
}