namespace Irc.Interfaces;

public interface IFloodProtectionProfile
{
    void SetFloodProtectionLevel(IFloodProtectionLevel floodProtectionLevel);
    IFloodProtectionLevel GetFloodProtectionLevel();
}