    using System;
using System.Collections.Generic;
using System.Text;
using Core.Ircx.Objects;
using CSharpTools;

namespace Core.Ircx.Runtime
{
    static class Register
    {
        public static void QualifyUser(Server server, Connection Connection)
        {
            Client Client = (Client)Connection.Client;
            User User = (User)Client;

            if (Client.Auth != null)
            {
                if (Client.Auth.UserCredentials != null)
                {
                    if (Client.Auth.UserCredentials.Password != null)
                    {
                        Client.Auth.UserCredentials.Username = Client.Address.Userhost;

                        for (int c = 0; c < Program.Credentials.Count; c++)
                        {
                            if (Client.Auth.UserCredentials.Username == Program.Credentials[c].Username)
                            {
                                if (Client.Auth.UserCredentials.Password == Program.Credentials[c].Password)
                                {
                                    // TO FIX

                                    User.Level = Program.Credentials[c].Level;
                                    server.UpdateUserNickname(User, Program.Credentials[c].Nickname);
                                    Client.Address.Userhost = Program.Credentials[c].Username;
                                    Client.Address.Hostname = "cg";

                                    switch (User.Level)
                                    {
                                        case UserAccessLevel.ChatAdministrator: { User.Profile.UserMode = ProfileUserMode.Admin; break; }
                                        case UserAccessLevel.ChatSysopManager: case UserAccessLevel.ChatSysop: { User.Profile.UserMode = ProfileUserMode.Sysop; break; }
                                        case UserAccessLevel.ChatGuide: { User.Profile.UserMode = ProfileUserMode.Guide; break; }
                                    }

                                    User.Profile.UserMode = ProfileUserMode.Admin;
                                }
                            }
                        }
                        
                    }
                }
            }

            // Check user's user and nickname
            //if ((user.Address.Nickname != Resources.Wildcard) && (user.Authenticated))
            if ((Client.Address.Nickname != Resources.Wildcard) && (Client.Address.Userhost != Resources.Wildcard) && (Client.Address.Hostname != Resources.Wildcard) && (Client.IsConnected))
            {
                if (Client.Auth == null)
                {
                    Client.Auth = new Authentication.ANON();
                    if (Client.Address.Nickname.bytes[0] != '>')
                    {
                        int nLen = (Client.Address.Nickname.Length > 62 ? 63 : Client.Address.Nickname.Length);
                        String8 ChangeNick = new String8(nLen + 1);
                        ChangeNick.append('>');
                        ChangeNick.append(Client.Address.Nickname.bytes, 0, nLen);
                        server.UpdateUserNickname(User, ChangeNick);
                        
                    }
                }

                Commands.NICK.ValidateNicknameResult vNicknameRes = Commands.NICK.ValidateNickname(User, Client.Name);

                if ((vNicknameRes != Commands.NICK.ValidateNicknameResult.INVALID) || (User.Level >= UserAccessLevel.ChatGuide))
                {
                    Client.Address.UpdateAddressMask(Address.AddressMaskType.NUHS);


                    // Check against access
                    Core.Ircx.Objects.Access.ObjectAccessResult result = server.Access.GetAccess(Client.Address);
                    if (result.Entry != null)
                    {
                        if (result.Entry.Level.Level == EnumAccessLevel.DENY)
                        {
                            Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_CLOSINGLINK, Data: new String8[] { Client.Address.RemoteIP, result.Entry.Reason }));
                            Client.Terminate();
                            return;
                            // Deny user from network
                        }
                    }

                    if (User.ObjectType == ObjType.UserObject) { 
                        server.RegisterUser(User);
                        // welcome raws

                        String8 ServerName = Program.Config.ServerFullName;

                        Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_RPL_WELCOME_001, Data: new String8[] { ServerName }));
                        Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_RPL_WELCOME_002, IData: new int[] { Program.Config.major, Program.Config.minor, Program.Config.build }));
                        Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_RPL_WELCOME_003, Data: new String8[] { server.CreationDate }));
                        Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_RPL_WELCOME_004, IData: new int[] { Program.Config.major, Program.Config.minor, Program.Config.build }));

                        // Below is not required anymore due to mIRC obeying IRCX with version >= 5.5
                        //switch (user.Profile.Ircvers)
                        //{
                        //    case -1: case 0: case 9: { user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WELCOME_005)); break; }
                        //}

                        Commands.LUSERS.SendLusers(server, User);
                        Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_ERR_NOMOTD_422));

                        if (User.Level >= UserAccessLevel.ChatGuide)
                        {
                            String8 RPLStaffRaw = Resources.Null;
                            byte StaffChar = 0x0;

                            switch (User.Level)
                            {
                                case UserAccessLevel.ChatGuide: { RPLStaffRaw = Raws.IRCX_RPL_YOUREGUIDE_629; StaffChar = Resources.UserModeCharOper; break; }
                                case UserAccessLevel.ChatSysop: { RPLStaffRaw = Raws.IRCX_RPL_YOUREOPER_381; StaffChar = Resources.UserModeCharOper; break; }
                                case UserAccessLevel.ChatSysopManager: { RPLStaffRaw = Raws.IRCX_RPL_YOUREOPER_381; StaffChar = Resources.UserModeCharOper; break; }
                                case UserAccessLevel.ChatAdministrator: { RPLStaffRaw = Raws.IRCX_RPL_YOUREADMIN_386; StaffChar = Resources.UserModeCharAdmin; break; }
                                case UserAccessLevel.ChatService: { RPLStaffRaw = Raws.IRCX_RPL_YOUREADMIN_386; StaffChar = Resources.UserModeCharAdmin; break; }
                            }

                            Core.Ircx.Commands.UserModeInvisibleFunction.ToggleInvisible(server, User, 1);
                            User.Modes.Admin.Value = 0x1;
                            User.Modes.UpdateModes();

                            AuditModeReport AdminMode = new AuditModeReport();
                            AdminMode.UserModes.Add(new AuditUserMode(User, Client.Address.Nickname, StaffChar, true));
                            AdminMode.UserModes.Add(new AuditUserMode(User, Client.Address.Nickname, Resources.UserModeCharInvisible, true));
                            Commands.MODE.ProcessUserReport(server, User, User, AdminMode);
                            Client.Send(Raws.Create(Server: server, Client: Client, Raw: RPLStaffRaw));
                        }
                    }
                    else if (User.ObjectType == ObjType.ServerObject)
                    {
                        // Attempt at inter-server communication
                        Server serv = (Server)server.AddObject(User.Name, ObjType.ServerObject, User.Name);
                        server.RemoveObject(User);
                        User = null;
                        Client = null;
                        Connection.Client = serv;
                        serv.Register();
                        serv.Send(Raws.Create(Server: serv, Raw: Raws.RPL_SERVICE_DATA, Data: new String8[] { "LOGON", serv.OIDX8 }));
                    }
                }
                else if (vNicknameRes == Commands.NICK.ValidateNicknameResult.IDENTICAL) ;
                else
                {
                    //user.Address.Nickname = Resources.Wildcard;
                    Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_ERR_ERRONEOUSNICK_432, Data: new String8[] { Client.Name }));
                    Client.Address.Nickname = Resources.Wildcard;
                }
            }
            return;
        }
    }
}
