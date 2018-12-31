using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;

namespace Core.Ircx.Objects
{
    public class Address
    {
        public static String8 Unassigned = new String8("*");

        public String8[] _address = new String8[5];
        public static int MaxNickLen = 64;
        public static int MaxFieldLen = 64;
        public enum AddressMaskType { NU, UH, NUH, NUHS };
        public bool UsesIP;
        //             Nickname ! Userhost @ Hostname $ Server
        //Address1     Nickname ! Userhost
        //Address2                Userhost @ Hostname
        //Address3     Nickname ! Userhost @ Hostname
        //Address4     Nickname ! Userhost @ Hostname $ Server
        //Address5     Nickname ! Userhost @ IP       $ Server

        private String8 InternalNickname = Unassigned, InternalNicknameU = Unassigned, InternalUserhost = Unassigned, InternalHostname = null, InternalServer = Unassigned, InternalRealname = null;

        public String8 RemoteIP = Resources.Wildcard;
        public String8 RemoteHost = Resources.Wildcard;

        public String8 RealName
        {
            get { return (InternalRealname == null ? Resources.Null : InternalRealname); }
            set {
                InternalRealname = value;
                if (InternalRealname.length > MaxFieldLen) { InternalRealname = new String8(InternalRealname.bytes, 0, (Address.MaxFieldLen - 1));  }
            }
        }

        public String8 Nickname
        {
            get { return (InternalNickname == null ? Unassigned : InternalNickname); }
            set { InternalNickname = value; InternalNicknameU = new String8(InternalNickname.bytes); InternalNicknameU.toupper(); UpdateAddressMask(AddressMaskType.NU); }
        }
        public String8 UNickname
        {
            get { return (InternalNicknameU == null ? Unassigned : InternalNicknameU); }
        }
        public String8 Userhost
        {
            get { return (InternalUserhost == null ? Unassigned : InternalUserhost); }
            set { InternalUserhost = value; UpdateAddressMask(AddressMaskType.UH); }
        }
        public String8 Hostname
        {
            get { return (InternalHostname == null ? RemoteIP : InternalHostname); }
            set { InternalHostname = value; UpdateAddressMask(AddressMaskType.UH); }
        }
        public String8 Server
        {
            get { return (InternalServer == null ? Unassigned : InternalServer); }
            set { InternalServer = value; UpdateAddressMask(AddressMaskType.NUHS); }
        }

        public Address()
        {
        }

        public static bool operator ==(Address a1, Address a2)
        {
            if ((object.ReferenceEquals(a1, null) && (object.ReferenceEquals(a2, null)))) { return true; }
            else if ((object.ReferenceEquals(a1, null) || (object.ReferenceEquals(a2, null)))) { return false; }
            else if (object.ReferenceEquals(a1, a2)) { return true; }

            return ((a1._address[0] == a2._address[0]) &&
                    (a1._address[1] == a2._address[1]) &&
                    (a1._address[2] == a2._address[2]) &&
                    (a1._address[3] == a2._address[3])
                );
        }

        public static bool operator !=(Address a1, Address a2)
        {
            if ((object.ReferenceEquals(a1, null) && (object.ReferenceEquals(a2, null)))) { return false; }
            else if ((object.ReferenceEquals(a1, null) || (object.ReferenceEquals(a2, null)))) { return true; }
            else if (object.ReferenceEquals(a1, a2)) { return false; }

            return (!(a1._address[0] == a2._address[0]) &&
                    (a1._address[1] == a2._address[1]) &&
                    (a1._address[2] == a2._address[2]) &&
                    (a1._address[3] == a2._address[3])
                );
        }

        public void InvalidateNickname() { Nickname = Unassigned; }

        public bool FromMask(String8 Mask)
        {
            if (Mask.length > 80) { Mask.length = 80; } //Verified in Exchange 5.5, accounts for !$@ + 1 more

            String8 FieldData = new String8(MaxFieldLen);
            int CurrentField = 0; //0 = Nick, 1 = User, 2 = Host, 3 = Server

            for (int i = 0; i < Mask.length; i++)
            {
                switch (Mask.bytes[i])
                {
                    case 33:
                        {
                            if (FieldData.length > 0) { _address[CurrentField] = new String8(FieldData.bytes, 0, FieldData.length); }
                            CurrentField = 1;
                            FieldData.length = 0;
                            break;
                        } //!
                    case 64:
                        {
                            if (CurrentField == 0) { CurrentField = 1; }
                            if (FieldData.length > 0) { _address[CurrentField] = new String8(FieldData.bytes, 0, FieldData.length); }
                            CurrentField = 2;
                            FieldData.length = 0;
                            break;
                        } //@
                    case 36:
                        {
                            if (CurrentField == 0) { CurrentField = 2; }
                            if (FieldData.length > 0) { _address[CurrentField] = new String8(FieldData.bytes, 0, FieldData.length); }
                            CurrentField = 3;
                            FieldData.length = 0;
                            break;
                        }  //$
                    default:
                        {
                            if ((Mask.bytes[i] == 46) && (CurrentField == 2))
                            { // .
                                UsesIP = true;
                            }
                            FieldData.append(Mask.bytes[i]);
                            break;
                        }
                }
            }
            if (FieldData.length > 0) { _address[CurrentField] = new String8(FieldData.bytes, 0, FieldData.length); }

            InternalNickname = _address[0];
            InternalUserhost = _address[1];
            InternalHostname = _address[2];
            InternalServer = _address[3];
            UpdateAddressMask(AddressMaskType.UH);

            return true;
        }

        public void UpdateAddressMask(AddressMaskType type)
        {
            switch (type)
            {
                case AddressMaskType.NU:
                    {
                        _address[0] = new String8(Nickname.length + Userhost.length + 1);
                        _address[0].append(Nickname);
                        _address[0].append('!');
                        _address[0].append(Userhost);
                        UpdateAddressMask(AddressMaskType.NUH);
                        break;
                    }
                case AddressMaskType.UH:
                    {
                        _address[1] = new String8(Userhost.length + Hostname.length + 1);
                        _address[1].append(Userhost);
                        _address[1].append('@');
                        _address[1].append(Hostname);
                        UpdateAddressMask(AddressMaskType.NU);
                        UpdateAddressMask(AddressMaskType.NUH);
                        break;
                    }
                case AddressMaskType.NUH:
                    {
                        _address[2] = new String8(Nickname.length + Userhost.length + Hostname.length + 2);
                        _address[2].append(_address[0]);
                        _address[2].append('@');
                        _address[2].append(Hostname);
                        UpdateAddressMask(AddressMaskType.NUHS);
                        break;
                    }
                case AddressMaskType.NUHS:
                    {
                        _address[3] = new String8(_address[2].Length + Server.Length + 1);
                        _address[3].append(_address[2]);
                        _address[3].append('$');
                        _address[3].append(Server);

                        _address[4] = new String8(_address[0].Length + Hostname.Length + Server.length + 2);
                        _address[4].append(_address[0]);
                        _address[4].append('@');
                        _address[4].append(Hostname);
                        _address[4].append('$');
                        _address[4].append(Server);
                        break;
                    }
            }
        }

    }
}
