using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.IO;

public class FloodProtectionProfile : IFloodProtectionProfile
{
    private IFloodProtectionLevel _floodProtectionLevel;

    public FloodProtectionProfile()
    {
        _floodProtectionLevel = new FloodProtectionLevel(EnumFloodProtectionLevel.Low);
    }

    public void SetFloodProtectionLevel(IFloodProtectionLevel floodProtectionLevel)
    {
        _floodProtectionLevel = floodProtectionLevel;
    }

    public IFloodProtectionLevel GetFloodProtectionLevel()
    {
        return _floodProtectionLevel;
    }
}