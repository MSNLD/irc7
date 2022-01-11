    using System;
using System.Collections.Generic;
using System.Text;
    using Core.CSharpTools;
    using Core.Ircx.Objects;
using CSharpTools;

namespace Core.Ircx.Runtime
{
    static class Register
    {
        public static void QualifyUser(Server server, Connection Connection)
        {
            Client Client = (Client)Connection.Client;
            User user = (User)Client;

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

                                    user.Level = Program.Credentials[c].Level;
                                    server.UpdateUserNickname(user, Program.Credentials[c].Nickname);
                                    Client.Address.Userhost = Program.Credentials[c].Username;
                                    Client.Address.Hostname = "cg";

                                    switch (user.Level)
                                    {
                                        case UserAccessLevel.ChatAdministrator: { user.Profile.UserMode = ProfileUserMode.Admin; break; }
                                        case UserAccessLevel.ChatSysopManager: case UserAccessLevel.ChatSysop: { user.Profile.UserMode = ProfileUserMode.Sysop; break; }
                                        case UserAccessLevel.ChatGuide: { user.Profile.UserMode = ProfileUserMode.Guide; break; }
                                    }

                                    user.Profile.UserMode = ProfileUserMode.Admin;
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
                    if (Client.Address.Nickname[0] != '>')
                    {
                        int nLen = (Client.Address.Nickname.Length > 62 ? 63 : Client.Address.Nickname.Length);
                        StringBuilder ChangeNick = new StringBuilder(nLen + 1);
                        ChangeNick.Append('>');
                        ChangeNick.Append(Client.Address.Nickname.ToString().Substring(nLen));
                        server.UpdateUserNickname(user, ChangeNick.ToString());
                        
                    }
                }

                Commands.NICK.ValidateNicknameResult vNicknameRes = Commands.NICK.ValidateNickname(user, Client.Name);

                if ((vNicknameRes != Commands.NICK.ValidateNicknameResult.INVALID) || (user.Level >= UserAccessLevel.ChatGuide))
                {
                    Client.Address.UpdateAddressMask(Address.AddressMaskType.NUHS);


                    // Check against access
                    Core.Ircx.Objects.Access.ObjectAccessResult result = server.Access.GetAccess(Client.Address);
                    if (result.Entry != null)
                    {
                        if (result.Entry.Level.Level == EnumAccessLevel.DENY)
                        {
                            Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_CLOSINGLINK, Data: new string[] { Client.Address.RemoteIP, result.Entry.Reason }));
                            Client.Terminate();
                            return;
                            // Deny user from network
                        }
                    }

                    if (user.ObjectType == ObjType.UserObject) { 
                        server.RegisterUser(user);
                        // welcome raws

                        string ServerName = Program.Config.ServerFullName;
                        
                        Client.Send(Raw.IRCX_RPL_WELCOME_001(server, user));
                        Client.Send(Raw.IRCX_RPL_WELCOME_002(server, user, new Version(Program.Config.major, Program.Config.minor, Program.Config.build)));
                        Client.Send(Raw.IRCX_RPL_WELCOME_003(server, user));
                        Client.Send(Raw.IRCX_RPL_WELCOME_004(server, user, new Version(Program.Config.major, Program.Config.minor, Program.Config.build)));

                        // Below is not required anymore due to mIRC obeying IRCX with version >= 5.5
                        //switch (user.Profile.Ircvers)
                        //{
                        //    case -1: case 0: case 9: { user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WELCOME_005)); break; }
                        //}

                        Commands.LUSERS.SendLusers(server, user);
                        Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_ERR_NOMOTD_422));

                        if (user.Level >= UserAccessLevel.ChatGuide)
                        {
                            string RPLStaffRaw = Resources.Null;
                            byte StaffChar = 0x0;

                            switch (user.Level)
                            {
                                case UserAccessLevel.ChatGuide: { RPLStaffRaw = Raws.IRCX_RPL_YOUREGUIDE_629; StaffChar = Resources.UserModeCharOper; break; }
                                case UserAccessLevel.ChatSysop: { RPLStaffRaw = Raws.IRCX_RPL_YOUREOPER_381; StaffChar = Resources.UserModeCharOper; break; }
                                case UserAccessLevel.ChatSysopManager: { RPLStaffRaw = Raws.IRCX_RPL_YOUREOPER_381; StaffChar = Resources.UserModeCharOper; break; }
                                case UserAccessLevel.ChatAdministrator: { RPLStaffRaw = Raws.IRCX_RPL_YOUREADMIN_386; StaffChar = Resources.UserModeCharAdmin; break; }
                                case UserAccessLevel.ChatService: { RPLStaffRaw = Raws.IRCX_RPL_YOUREADMIN_386; StaffChar = Resources.UserModeCharAdmin; break; }
                            }

                            Core.Ircx.Commands.UserModeInvisibleFunction.ToggleInvisible(server, user, 1);
                            user.Modes.Admin.Value = 0x1;
                            user.Modes.UpdateModes();

                            AuditModeReport AdminMode = new AuditModeReport();
                            AdminMode.UserModes.Add(new AuditUserMode(user, Client.Address.Nickname, StaffChar, true));
                            AdminMode.UserModes.Add(new AuditUserMode(user, Client.Address.Nickname, Resources.UserModeCharInvisible, true));
                            Commands.MODE.ProcessUserReport(server, user, user, AdminMode);
                            Client.Send(Raws.Create(Server: server, Client: Client, Raw: RPLStaffRaw));
                        }
                    }
                    else if (user.ObjectType == ObjType.ServerObject)
                    {
                        // Attempt at inter-server communication
                        Server serv = (Server)server.AddObject(user.Name, ObjType.ServerObject, user.Name);
                        server.RemoveObject(user);
                        user = null;
                        Client = null;
                        Connection.Client = serv;
                        serv.Register();
                        serv.Send(Raws.Create(Server: serv, Raw: Raws.RPL_SERVICE_DATA, Data: new string[] { "LOGON", serv.OIDX8 }));
                    }
                }
                else if (vNicknameRes == Commands.NICK.ValidateNicknameResult.IDENTICAL) ;
                else
                {
                    //user.Address.Nickname = Resources.Wildcard;
                    Client.Send(Raws.Create(Server: server, Client: Client, Raw: Raws.IRCX_ERR_ERRONEOUSNICK_432, Data: new string[] { Client.Name }));
                    Client.Address.Nickname = Resources.Wildcard;
                }
            }
            return;
        }
    }
}
