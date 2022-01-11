using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;
using Core.Authentication;
using Core.CSharpTools;

namespace Core.Ircx.Commands
{
    class AUTH : Command
    {
        //AUTH <Package> <Seg> <Token>
        public AUTH(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 3;
            base.DataType = CommandDataType.None;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data[1] == "I")
            {
                if (Frame.User.Auth == null)
                {
                    if (Frame.User.Registered)
                    {
                        // You are already authenticated
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ALREADYAUTHENTICATED_909, Data: new string[] { Frame.Message.Data[0] }));
                    }
                    else
                    {
                        Frame.User.Auth = Authentication.SSP.GetPackage(Frame.Message.Data[0]);
                        if (Frame.User.Auth == null)
                        {
                            // No such authentication package
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNPACKAGE_912, Data: new string[] { Frame.Message.Data[0] }));
                        }
                        else
                        {
                            if (Frame.User.Auth.InitializeSecurityContext(Frame.Message.Data[2].ToString(), Program.Config.ExternalIP) == Authentication.SSP.state.SSP_OK)
                            {
                                string data = StringExtensions.ToEscape(Frame.User.Auth.CreateSecurityChallenge(SSP.state.SSP_SEC));
                                string reply = Raws.Create(Server: Frame.Server, Raw: Raws.RPL_AUTH_SEC_REPLY, Data: new string[] { Frame.Message.Data[0], data });
                                Frame.User.Send(reply);
                            }
                            else
                            {
                                // Authentication failed
                                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_AUTHENTICATIONFAILED_910, Data: new string[] { Frame.Message.Data[0] }));
                            }
                        }
                    }
                }
            }
            else if (Frame.Message.Data[1] == "S")
            {
                if (Frame.User.Auth != null)
                {
                    if (Frame.User.Registered)
                    {
                        // You are already authenticated
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ALREADYAUTHENTICATED_909, Data: new string[] { Frame.Message.Data[0] }));
                    }
                    else
                    {
                        SSP.state State = Frame.User.Auth.AcceptSecurityContext(Frame.Message.Data[2].ToString(), Program.Config.ExternalIP);
                        if (State == Authentication.SSP.state.SSP_OK)
                        {
                            if (Frame.User.Auth.Signature == Core.Authentication.Package.GateKeeperPassport.SIGNATURE)
                            {
                                Frame.User.Properties.Puid.Value = ((Core.Authentication.Package.GateKeeperPassport)Frame.User.Auth).puid;
                                if (Frame.User.Modes.Secure.Value == 0x1) { 
                                    Frame.User.Profile.Registered = true; // For subscriber
                                }
                                Frame.User.Properties.MsnRegCookie.SetPermissions(UserAccessLevel.None, UserAccessLevel.None, false, true);
                                Frame.User.Profile.UserType = ProfileUserType.Registered;
                                Frame.User.Level = UserAccessLevel.ChatUser;
                            }

                            Frame.User.Address.Userhost = new string(StringExtensions.FromBytes(Frame.User.Auth.uuid).ToString());
                            Frame.User.Address.Hostname = Frame.User.Auth.GetDomain();

                            string reply = Raws.Create(Server: Frame.Server, Raw: Raws.RPL_AUTH_SUCCESS, Data: new string[] { Frame.Message.Data[0], Frame.User.Address._address[1] }, IData: new int[] { (int)Frame.User.OID });
                            Frame.User.Send(reply);
                        }
                        else if (State == Authentication.SSP.state.SSP_CREDENTIALS)
                        {
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Raw: Raws.RPL_AUTH_SEC_REPLY, Data: new string[] { Frame.Message.Data[0], Resources.S_OK }));
                        }
                        else
                        {
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_AUTHENTICATIONFAILED_910, Data: new string[] { Frame.Message.Data[0] }));
                        }
                    }
                }
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
