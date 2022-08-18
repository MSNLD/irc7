using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EnumChannelAccessLevel
{
    None = 0,
    ChatGuest = 1,
    ChatUser = 2,
    ChatMember = 3,
    ChatVoice = 4,
    ChatHost = 5,
    ChatOwner = 6
 }