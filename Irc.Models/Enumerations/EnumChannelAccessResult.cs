﻿namespace Irc.Models.Enumerations;

public enum EnumChannelAccessResult
{
    ERR_NICKINUSE = -8,
    ERR_ALREADYINCHANNEL = -7,
    ERR_CHANNELISFULL = -6,
    ERR_INVITEONLYCHAN = -5,
    ERR_BANNEDFROMCHAN = -4,
    ERR_BADCHANNELKEY = -3,
    ERR_AUTHONLYCHAN = -2,
    ERR_SECUREONLYCHAN = -1,
    NONE = 0,
    SUCCESS_MEMBER = 1,
    SUCCESS_VOICE = 2,
    SUCCESS_HOST = 3,
    SUCCESS_OWNER = 4
}