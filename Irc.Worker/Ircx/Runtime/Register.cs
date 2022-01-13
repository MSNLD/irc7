using System;
using System.Text;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Extensions.Security.Packages;
using Irc.Worker.Ircx.Commands;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Runtime; 

internal static class Register
{
    public static void QualifyUser(Server server, Connection Connection)
    {
        var Client = Connection.Client;
        var user = (User) Client;

        if (Client.Auth != null)
            if (Client.Auth.UserCredentials != null)
                if (Client.Auth.UserCredentials.Password != null)
                {
                    Client.Auth.UserCredentials.Username = Client.Address.Userhost;

                    for (var c = 0; c < Program.Credentials.Count; c++)
                        if (Client.Auth.UserCredentials.Username == Program.Credentials[c].Username)
                            if (Client.Auth.UserCredentials.Password == Program.Credentials[c].Password)
                            {
                                // TO FIX

                                user.Level = Program.Credentials[c].Level;
                                user.UpdateUserNickname(Program.Credentials[c].Nickname);
                                Client.Address.Userhost = Program.Credentials[c].Username;
                                Client.Address.Hostname = "cg";

                                switch (user.Level)
                                {
                                    case UserAccessLevel.ChatAdministrator:
                                    {
                                        user.Profile.UserMode = ProfileUserMode.Admin;
                                        break;
                                    }
                                    case UserAccessLevel.ChatSysopManager:
                                    case UserAccessLevel.ChatSysop:
                                    {
                                        user.Profile.UserMode = ProfileUserMode.Sysop;
                                        break;
                                    }
                                    case UserAccessLevel.ChatGuide:
                                    {
                                        user.Profile.UserMode = ProfileUserMode.Guide;
                                        break;
                                    }
                                }

                                user.Profile.UserMode = ProfileUserMode.Admin;
                            }
                }

        // Check user's user and nickname
        //if ((user.Address.Nickname != Resources.Wildcard) && (user.Authenticated))
        if (Client.Address.Nickname != Resources.Wildcard && Client.Address.Userhost != Resources.Wildcard &&
            Client.Address.Hostname != Resources.Wildcard && Client.IsConnected)
        {
            if (Client.Auth == null)
            {
                Client.Auth = new ANON();
                if (Client.Address.Nickname[0] != '>')
                {
                    var nLen = Client.Address.Nickname.Length > 62 ? 63 : Client.Address.Nickname.Length;
                    var ChangeNick = new StringBuilder(nLen + 1);
                    ChangeNick.Append('>');
                    ChangeNick.Append(Client.Address.Nickname.Substring(nLen));
                    user.UpdateUserNickname(ChangeNick.ToString());
                }
            }

            var vNicknameRes = NICK.ValidateNickname(user, Client.Name);

            if (vNicknameRes != NICK.ValidateNicknameResult.INVALID || user.Level >= UserAccessLevel.ChatGuide)
            {
                Client.Address.UpdateAddressMask(Address.AddressMaskType.NUHS);


                // Check against access
                var result = server.Access.GetAccess(Client.Address);
                if (result.Entry != null)
                    if (result.Entry.Level.Level == EnumAccessLevel.DENY)
                    {
                        Client.Send(RawBuilder.Create(server, Client: Client, Raw: Raws.IRCX_CLOSINGLINK,
                            Data: new[] {Client.Address.RemoteIP, result.Entry.Reason}));
                        Client.Terminate();
                        return;
                        // Deny user from network
                    }

                if (user is User)
                {
                    server.RegisterUser(user);
                    // welcome raws

                    var ServerName = Program.Config.ServerFullName;

                    Client.Send(Raw.IRCX_RPL_WELCOME_001(server, user));
                    Client.Send(Raw.IRCX_RPL_WELCOME_002(server, user,
                        new Version(Program.Config.major, Program.Config.minor, Program.Config.build)));
                    Client.Send(Raw.IRCX_RPL_WELCOME_003(server, user));
                    Client.Send(Raw.IRCX_RPL_WELCOME_004(server, user,
                        new Version(Program.Config.major, Program.Config.minor, Program.Config.build)));

                    // Below is not required anymore due to mIRC obeying IRCX with version >= 5.5
                    //switch (user.Profile.Ircvers)
                    //{
                    //    case -1: case 0: case 9: { user.Send(RawBuilder.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_WELCOME_005)); break; }
                    //}

                    LUSERS.SendLusers(server, user);
                    Client.Send(RawBuilder.Create(server, Client: Client, Raw: Raws.IRCX_ERR_NOMOTD_422));

                    if (user.Level >= UserAccessLevel.ChatGuide)
                    {
                        var RPLStaffRaw = Resources.Null;
                        byte StaffChar = 0x0;

                        switch (user.Level)
                        {
                            case UserAccessLevel.ChatGuide:
                            {
                                RPLStaffRaw = Raws.IRCX_RPL_YOUREGUIDE_629;
                                StaffChar = Resources.UserModeCharOper;
                                break;
                            }
                            case UserAccessLevel.ChatSysop:
                            {
                                RPLStaffRaw = Raws.IRCX_RPL_YOUREOPER_381;
                                StaffChar = Resources.UserModeCharOper;
                                break;
                            }
                            case UserAccessLevel.ChatSysopManager:
                            {
                                RPLStaffRaw = Raws.IRCX_RPL_YOUREOPER_381;
                                StaffChar = Resources.UserModeCharOper;
                                break;
                            }
                            case UserAccessLevel.ChatAdministrator:
                            {
                                RPLStaffRaw = Raws.IRCX_RPL_YOUREADMIN_386;
                                StaffChar = Resources.UserModeCharAdmin;
                                break;
                            }
                            case UserAccessLevel.ChatService:
                            {
                                RPLStaffRaw = Raws.IRCX_RPL_YOUREADMIN_386;
                                StaffChar = Resources.UserModeCharAdmin;
                                break;
                            }
                        }

                        UserModeInvisibleFunction.ToggleInvisible(server, user, 1);
                        user.Modes.Admin.Value = 0x1;
                        user.Modes.UpdateModes();

                        var AdminMode = new AuditModeReport();
                        AdminMode.UserModes.Add(new AuditUserMode(user, Client.Address.Nickname, StaffChar, true));
                        AdminMode.UserModes.Add(new AuditUserMode(user, Client.Address.Nickname,
                            Resources.UserModeCharInvisible, true));
                        MODE.ProcessUserReport(server, user, user, AdminMode);
                        Client.Send(RawBuilder.Create(server, Client: Client, Raw: RPLStaffRaw));
                    }
                }
            }
            else if (vNicknameRes == NICK.ValidateNicknameResult.IDENTICAL)
            {
                ;
            }
            else
            {
                //user.Address.Nickname = Resources.Wildcard;
                Client.Send(RawBuilder.Create(server, Client: Client, Raw: Raws.IRCX_ERR_ERRONEOUSNICK_432,
                    Data: new[] {Client.Name}));
                Client.Address.Nickname = Resources.Wildcard;
            }
        }
    }
}