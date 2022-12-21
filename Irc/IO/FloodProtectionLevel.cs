using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.IO;

public class FloodProtectionLevel : IFloodProtectionLevel
{
    //Parameters, Invitation, Join, WrongChannelPassword, Standard, HostMessage, None
    public int Delay, DelayData, DelayInvite, DelayJoin, DelayWrongPass, DelayStandard, DelayHostMessage;

    public long LastProcessedTicks,
        LastProcessedData,
        LastProcessedInvite,
        LastProcessedJoin,
        LastProcessedPassword,
        LastProcessedStandard,
        LastProcessedHostMessage;

    public FloodProtectionLevel(EnumFloodProtectionLevel level)
    {
        var offset = 0x089f7ff5f7b58000; //01/01/1970
        LastProcessedTicks = offset;
        LastProcessedData = offset;
        LastProcessedInvite = offset;
        LastProcessedJoin = offset;
        LastProcessedPassword = offset;
        LastProcessedStandard = offset;
        LastProcessedHostMessage = offset;

        SetProtectionLevel(level);
    }

    public bool CanProcess
    {
        get
        {
            if (Delay > 0)
            {
                if ((DateTime.UtcNow.Ticks - LastProcessedTicks) / TimeSpan.TicksPerSecond >= Delay)
                {
                    LastProcessedTicks = DateTime.UtcNow.Ticks;
                    return true;
                }

                return false;
            }

            return true;
        }
    }

    public bool CanProcessData
    {
        get
        {
            if (DelayData > 0)
            {
                if ((DateTime.UtcNow.Ticks - LastProcessedData) / TimeSpan.TicksPerSecond >= DelayData)
                {
                    LastProcessedData = DateTime.UtcNow.Ticks;
                    return true;
                }

                return false;
            }

            return true;
        }
    }

    public bool CanProcessInvite
    {
        get
        {
            if ((DateTime.UtcNow.Ticks - LastProcessedInvite) / TimeSpan.TicksPerSecond >= DelayInvite)
            {
                LastProcessedInvite = DateTime.UtcNow.Ticks;
                return true;
            }

            return false;
        }
    }

    public bool CanProcessJoin
    {
        get
        {
            if ((DateTime.UtcNow.Ticks - LastProcessedJoin) / TimeSpan.TicksPerSecond >= DelayJoin)
            {
                LastProcessedJoin = DateTime.UtcNow.Ticks;
                return true;
            }

            return false;
        }
    }

    public bool CanProcessPassword
    {
        get
        {
            if ((DateTime.UtcNow.Ticks - LastProcessedPassword) / TimeSpan.TicksPerSecond >= DelayWrongPass)
            {
                LastProcessedPassword = DateTime.UtcNow.Ticks;
                return true;
            }

            return false;
        }
    }

    public bool CanProcessStandard
    {
        get
        {
            if ((DateTime.UtcNow.Ticks - LastProcessedStandard) / TimeSpan.TicksPerSecond >= DelayStandard)
            {
                LastProcessedStandard = DateTime.UtcNow.Ticks;
                return true;
            }

            return false;
        }
    }

    public bool CanProcessHostMessage
    {
        get
        {
            if (DelayHostMessage > 0)
            {
                if ((DateTime.UtcNow.Ticks - LastProcessedHostMessage) / TimeSpan.TicksPerSecond >= DelayHostMessage)
                {
                    LastProcessedHostMessage = DateTime.UtcNow.Ticks;
                    return true;
                }

                return false;
            }

            return true;
        }
    }

    public void SetProtectionLevel(EnumFloodProtectionLevel level)
    {
        switch (level)
        {
            case EnumFloodProtectionLevel.Low:
            {
                DelayData = 1;
                DelayInvite = 2;
                DelayJoin = 2;
                DelayWrongPass = 2;
                DelayStandard = 1;
                DelayHostMessage = 0;
                break;
            }
            case EnumFloodProtectionLevel.Medium:
            {
                DelayData = 2;
                DelayInvite = 4;
                DelayJoin = 3;
                DelayWrongPass = 4;
                DelayStandard = 2;
                DelayHostMessage = 1;
                break;
            }
            case EnumFloodProtectionLevel.High:
            {
                DelayData = 3;
                DelayInvite = 5;
                DelayJoin = 4;
                DelayWrongPass = 5;
                DelayStandard = 3;
                DelayHostMessage = 2;
                break;
            }
            default:
            {
                DelayData = 0;
                DelayInvite = 0;
                DelayJoin = 0;
                DelayWrongPass = 0;
                DelayStandard = 0;
                DelayHostMessage = 0;
                break;
            }
        }
    }
}