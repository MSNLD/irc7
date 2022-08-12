﻿namespace Irc.Enumerations;

public enum EnumAccessResult
{
    SUCCESS_OWNER = -6,
    SUCCESS_HOST = -5,
    SUCCESS_VOICE = -4,
    SUCCESS_GRANTED = -3,
    SUCCESS_MEMBERKEY = -2,
    SUCCESS = -1,
    NONE = 0,
    ERR_ALREADYINCHANNEL = 1,
    ERR_NOSUCHNICK = 401,
    ERR_NICKINUSE = 433,
    ERR_CHANNELISFULL = 471,
    ERR_INVITEONLYCHAN = 473,
    ERR_BANNEDFROMCHAN = 474,
    ERR_BADCHANNELKEY = 475,
    ERR_SECUREONLYCHAN = 557,
    ERR_AUTHONLYCHAN = 904
}