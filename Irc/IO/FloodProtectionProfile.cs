using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.IO;

public class FloodProtectionProfile : IFloodProtectionProfile
{
    private FloodProtectionLevel _floodProtectionLevel;

    public FloodProtectionProfile()
    {
        _floodProtectionLevel = new FloodProtectionLevel(EnumFloodProtectionLevel.Low);
    }

    public void SetFloodProtectionLevel(FloodProtectionLevel floodProtectionLevel)
    {
        _floodProtectionLevel = floodProtectionLevel;
    }

    public FloodProtectionLevel GetFloodProtectionLevel()
    {
        return _floodProtectionLevel;
    }
}