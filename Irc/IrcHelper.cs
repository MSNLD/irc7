namespace Irc;

public static class IrcHelper
{
    public enum ObjIdentifier
    {
        ObjIdGlobalChannel = 0x1,
        ObjIdLocalChannel = 0x2,
        ObjIdExtendedGlobalChannel = 0x3,
        ObjIdExtendedLocalChannel = 0x4,
        ObjIdLastChannel = 0xF,
        ObjIdIrcUser = 0x5,
        ObjIdIrcUserUnicode = 0x6,
        ObjIdIrcUserHex = 0x7,
        ObjIdInternal = 0xE,
        ObjIdNetwork = 0xD,
        ObjIdServer = 0xC,
        InvalidObjId = 0xFF
    }

    public static ObjIdentifier IdentifyObject(string objectName)
    {
        if (objectName.Length == 1)
            switch (objectName[0])
            {
                case '$':
                {
                    return ObjIdentifier.ObjIdServer;
                }
                case '*':
                {
                    return ObjIdentifier.ObjIdNetwork;
                }
                case '%':
                {
                    return ObjIdentifier.ObjIdLastChannel;
                }
            }
        else if (objectName.Length > 1)
            switch (objectName[0])
            {
                case '%':
                {
                    switch (objectName[1])
                    {
                        case '#':
                        {
                            return ObjIdentifier.ObjIdExtendedGlobalChannel;
                        }
                        case '&':
                        {
                            return ObjIdentifier.ObjIdExtendedLocalChannel;
                        }
                        default:
                        {
                            return ObjIdentifier.InvalidObjId;
                        }
                    }
                }
                case '^':
                {
                    return ObjIdentifier.ObjIdIrcUserHex;
                }
                case '0':
                {
                    return ObjIdentifier.ObjIdInternal;
                }
                case '\'':
                {
                    return ObjIdentifier.ObjIdIrcUserUnicode;
                }
                case '#':
                {
                    return ObjIdentifier.ObjIdGlobalChannel;
                }
                case '&':
                {
                    return ObjIdentifier.ObjIdLocalChannel;
                }
            }

        // Default is user nickname
        return ObjIdentifier.ObjIdIrcUser;
    }

    public static bool IsObject(string name)
    {
        // Rule that an object must begin with 0
        if (name.Length > 0)
            if (name[0] == 48)
                return true;

        return false;
    }
}