using System;
using Irc.Extensions.Access;

namespace Irc.Worker.Ircx.Objects;
/*
 * If the service receives one of these messages, it temporarily suspends the user's session. After the delay expires, the service resumes normal message processing.
 * Also, if a user tries to join a channel using the wrong password, the service delays processing subsequent attempts by that user to join the channel. 
 */

/*
    [08:52] <.>Sky> The way the flood protection settings works is that it delays the message queue of that user only, so if you are for example pasting 100 lines in a
 *                  channel and subsequently try and do anything else in the queue it wont process that until the rest of the buffer is processed
    [08:52] <.>Sky> if for example you do a very expensive operation and paste 100 lines in a channel with a 2 second delay for message processing
    [08:52] <.>Sky> you have to wait 100 * 2 (200 seconds) to be able to do anything else, be it a version or whatever
    [08:53] <.>Sky> this is what Exchange has demonstrated to me so far and this is how I will implement it
 */
public class FloodProtectionLevel
{
    //Data, Invitation, Join, WrongChannelPassword, Standard, HostMessage, None
    public int Delay, DelayData, DelayInvite, DelayJoin, DelayWrongPass, DelayStandard, DelayHostMessage;

    public long LastProcessedTicks,
        LastProcessedData,
        LastProcessedInvite,
        LastProcessedJoin,
        LastProcessedPassword,
        LastProcessedStandard,
        LastProcessedHostMessage;

    public FloodProtectionLevel(ProtectionLevel level)
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

    public void SetProtectionLevel(ProtectionLevel level)
    {
        switch (level)
        {
            case ProtectionLevel.ctProtectionLow:
            {
                DelayData = 1;
                DelayInvite = 2;
                DelayJoin = 2;
                DelayWrongPass = 2;
                DelayStandard = 1;
                //DelayHostMessage = 1;
                DelayHostMessage = 0;
                break;
            }
            case ProtectionLevel.ctProtectionMedium:
            {
                DelayData = 2;
                DelayInvite = 4;
                DelayJoin = 3;
                DelayWrongPass = 4;
                DelayStandard = 2;
                DelayHostMessage = 1;
                break;
            }
            case ProtectionLevel.ctProtectionHigh:
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

public enum ProtectionLevel
{
    ctProtectionNone,
    ctProtectionLow,
    ctProtectionMedium,
    ctProtectionHigh
}

public class FloodProfile
{
    public FloodProtectionLevel FloodProtectionLevel;
    public uint InputFloodLimit, OutputSaturationLimit, ConnectionLimit, currentInputBytes, currentOutputBytes;

    public FloodProfile()
    {
        FloodProtectionLevel = new FloodProtectionLevel(ProtectionLevel.ctProtectionLow);
        InputFloodLimit = 4096; //bytes
        OutputSaturationLimit = 1 * 1024; //KB
    }

    public SAT_RESULT Saturation
    {
        get
        {
            if (currentInputBytes > InputFloodLimit)
                return SAT_RESULT.S_INPUT_EXCEEDED;
            if (currentOutputBytes > OutputSaturationLimit)
                return SAT_RESULT.S_OUTPUT_EXCEEDED;
            return SAT_RESULT.S_OK;
        }
    }
}

public enum SAT_RESULT
{
    S_OK,
    S_OUTPUT_EXCEEDED,
    S_INPUT_EXCEEDED
}

public enum FLD_RESULT
{
    S_OK,
    S_WAIT
}

public class Flood
{
    public static FLD_RESULT FloodCheck(CommandDataType type, User user)
    {
        return Audit(user.FloodProfile, type, user.Level);
    }

    public static FLD_RESULT FloodCheck(CommandDataType type, UserChannelInfo c)
    {
        return Audit(c.Channel.FloodProfile, type, c.Member.Level);
    }

    public static FLD_RESULT Audit(FloodProfile profile, CommandDataType type, UserAccessLevel level)
    {
        if (level >= UserAccessLevel.ChatHost) return FLD_RESULT.S_OK;

        switch (type)
        {
            case CommandDataType.None:
            {
                if (profile.FloodProtectionLevel.CanProcess)
                    return FLD_RESULT.S_OK;
                return FLD_RESULT.S_WAIT;
            }
            case CommandDataType.Data:
            {
                if (profile.FloodProtectionLevel.CanProcessData)
                    return FLD_RESULT.S_OK;
                return FLD_RESULT.S_WAIT;
            }
            case CommandDataType.Invitation:
            {
                if (profile.FloodProtectionLevel.CanProcessInvite)
                    return FLD_RESULT.S_OK;
                return FLD_RESULT.S_WAIT;
            }
            case CommandDataType.Join:
            {
                if (profile.FloodProtectionLevel.CanProcessJoin)
                    return FLD_RESULT.S_OK;
                return FLD_RESULT.S_WAIT;
            }
            case CommandDataType.WrongChannelPassword:
            {
                if (profile.FloodProtectionLevel.CanProcessPassword)
                    return FLD_RESULT.S_OK;
                return FLD_RESULT.S_WAIT;
            }
            case CommandDataType.Standard:
            {
                if (profile.FloodProtectionLevel.CanProcessStandard)
                    return FLD_RESULT.S_OK;
                return FLD_RESULT.S_WAIT;
            }
            case CommandDataType.HostMessage:
            {
                if (profile.FloodProtectionLevel.CanProcessHostMessage)
                    return FLD_RESULT.S_OK;
                return FLD_RESULT.S_WAIT;
            }
            default:
            {
                return FLD_RESULT.S_OK;
            }
        }
    }
}