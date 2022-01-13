using System.Text;
using Irc.Constants;

namespace Irc.Worker.Ircx.Objects;

public class Address
{
    public enum AddressMaskType
    {
        NU,
        UH,
        NUH,
        NUHS
    }

    public static string Unassigned = new("*");
    public static int MaxNickLen = 64;
    public static int MaxFieldLen = 64;

    public string[] _address = new string[5];
    //             Nickname ! Userhost @ Hostname $ Server
    //Address1     Nickname ! Userhost
    //Address2                Userhost @ Hostname
    //Address3     Nickname ! Userhost @ Hostname
    //Address4     Nickname ! Userhost @ Hostname $ Server
    //Address5     Nickname ! Userhost @ IP       $ Server

    private string InternalNickname = Unassigned,
        InternalNicknameU = Unassigned,
        InternalUserhost = Unassigned,
        InternalHostname,
        InternalServer = Unassigned,
        InternalRealname;

    public string RemoteHost = Resources.Wildcard;

    public string RemoteIP = Resources.Wildcard;
    public bool UsesIP;

    public string RealName
    {
        get => InternalRealname == null ? Resources.Null : InternalRealname;
        set
        {
            InternalRealname = value;
            if (InternalRealname.Length > MaxFieldLen)
                InternalRealname = new string(InternalRealname.Substring(MaxFieldLen));
        }
    }

    public string Nickname
    {
        get => InternalNickname == null ? Unassigned : InternalNickname;
        set
        {
            InternalNickname = value;
            InternalNicknameU = new string(InternalNickname.ToUpper());
            UpdateAddressMask(AddressMaskType.NU);
        }
    }

    public string UNickname => InternalNicknameU == null ? Unassigned : InternalNicknameU;

    public string Userhost
    {
        get => InternalUserhost == null ? Unassigned : InternalUserhost;
        set
        {
            InternalUserhost = value;
            UpdateAddressMask(AddressMaskType.UH);
        }
    }

    public string Hostname
    {
        get => InternalHostname == null ? RemoteIP : InternalHostname;
        set
        {
            InternalHostname = value;
            UpdateAddressMask(AddressMaskType.UH);
        }
    }

    public string Server
    {
        get => InternalServer == null ? Unassigned : InternalServer;
        set
        {
            InternalServer = value;
            UpdateAddressMask(AddressMaskType.NUHS);
        }
    }

    public static bool operator ==(Address a1, Address a2)
    {
        if (ReferenceEquals(a1, null) && ReferenceEquals(a2, null))
            return true;
        if (ReferenceEquals(a1, null) || ReferenceEquals(a2, null))
            return false;
        if (ReferenceEquals(a1, a2)) return true;

        return a1._address[0] == a2._address[0] &&
               a1._address[1] == a2._address[1] &&
               a1._address[2] == a2._address[2] &&
               a1._address[3] == a2._address[3];
    }

    public static bool operator !=(Address a1, Address a2)
    {
        if (ReferenceEquals(a1, null) && ReferenceEquals(a2, null))
            return false;
        if (ReferenceEquals(a1, null) || ReferenceEquals(a2, null))
            return true;
        if (ReferenceEquals(a1, a2)) return false;

        return !(a1._address[0] == a2._address[0]) &&
               a1._address[1] == a2._address[1] &&
               a1._address[2] == a2._address[2] &&
               a1._address[3] == a2._address[3];
    }

    public void InvalidateNickname()
    {
        Nickname = Unassigned;
    }

    public bool FromMask(string Mask)
    {
        if (Mask.Length > 80) Mask.Substring(80);

        var FieldData = new StringBuilder(MaxFieldLen);
        var CurrentField = 0; //0 = Nick, 1 = User, 2 = Host, 3 = Server

        for (var i = 0; i < Mask.Length; i++)
            switch (Mask[i])
            {
                case (char) 33:
                {
                    if (FieldData.Length > 0) _address[CurrentField] = new string(FieldData.ToString());
                    CurrentField = 1;
                    FieldData.Length = 0;
                    break;
                } //!
                case (char) 64:
                {
                    if (CurrentField == 0) CurrentField = 1;
                    if (FieldData.Length > 0) _address[CurrentField] = new string(FieldData.ToString());
                    CurrentField = 2;
                    FieldData.Length = 0;
                    break;
                } //@
                case (char) 36:
                {
                    if (CurrentField == 0) CurrentField = 2;
                    if (FieldData.Length > 0) _address[CurrentField] = new string(FieldData.ToString());
                    CurrentField = 3;
                    FieldData.Length = 0;
                    break;
                } //$
                default:
                {
                    if (Mask[i] == 46 && CurrentField == 2)
                        // .
                        UsesIP = true;
                    FieldData.Append(Mask[i]);
                    break;
                }
            }

        if (FieldData.Length > 0) _address[CurrentField] = new string(FieldData.ToString());

        InternalNickname = _address[0];
        InternalUserhost = _address[1];
        InternalHostname = _address[2];
        InternalServer = _address[3];
        UpdateAddressMask(AddressMaskType.UH);

        return true;
    }

    public void UpdateAddressMask(AddressMaskType type)
    {
        StringBuilder _addressBuilder;
        switch (type)
        {
            case AddressMaskType.NU:
            {
                _addressBuilder = new StringBuilder(Nickname.Length + Userhost.Length + 1);
                _addressBuilder.Append(Nickname);
                _addressBuilder.Append('!');
                _addressBuilder.Append(Userhost);
                _address[0] = _addressBuilder.ToString();
                UpdateAddressMask(AddressMaskType.NUH);
                break;
            }
            case AddressMaskType.UH:
            {
                _addressBuilder = new StringBuilder(Userhost.Length + Hostname.Length + 1);
                _addressBuilder.Append(Userhost);
                _addressBuilder.Append('@');
                _addressBuilder.Append(Hostname);
                _address[1] = _addressBuilder.ToString();
                UpdateAddressMask(AddressMaskType.NU);
                UpdateAddressMask(AddressMaskType.NUH);
                break;
            }
            case AddressMaskType.NUH:
            {
                _addressBuilder = new StringBuilder(Nickname.Length + Userhost.Length + Hostname.Length + 2);
                _addressBuilder.Append(_address[0]);
                _addressBuilder.Append('@');
                _addressBuilder.Append(Hostname);
                _address[2] = _addressBuilder.ToString();
                UpdateAddressMask(AddressMaskType.NUHS);
                break;
            }
            case AddressMaskType.NUHS:
            {
                _addressBuilder = new StringBuilder(_address[2].Length + Server.Length + 1);
                _addressBuilder.Append(_address[2]);
                _addressBuilder.Append('$');
                _addressBuilder.Append(Server);
                _address[3] = _addressBuilder.ToString();

                _addressBuilder = new StringBuilder(_address[0].Length + Hostname.Length + Server.Length + 2);
                _addressBuilder.Append(_address[0]);
                _addressBuilder.Append('@');
                _addressBuilder.Append(Hostname);
                _addressBuilder.Append('$');
                _addressBuilder.Append(Server);
                _address[4] = _addressBuilder.ToString();
                break;
            }
        }
    }
}