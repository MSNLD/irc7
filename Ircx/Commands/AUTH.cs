using Core.Authentication;
using Core.Authentication.Package;
using Core.CSharpTools;
using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

internal class AUTH : Command
{
    //AUTH <Package> <Seg> <Token>
    public AUTH(CommandCode Code) : base(Code)
    {
        MinParamCount = 3;
        DataType = CommandDataType.None;
        ForceFloodCheck = true;
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
                    Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                        Raw: Raws.IRCX_ERR_ALREADYAUTHENTICATED_909, Data: new[] {Frame.Message.Data[0]}));
                }
                else
                {
                    Frame.User.Auth = SSP.GetPackage(Frame.Message.Data[0]);
                    if (Frame.User.Auth == null)
                    {
                        // No such authentication package
                        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_UNKNOWNPACKAGE_912, Data: new[] {Frame.Message.Data[0]}));
                    }
                    else
                    {
                        if (Frame.User.Auth.InitializeSecurityContext(Frame.Message.Data[2],
                                Program.Config.ExternalIP) == SSP.state.SSP_OK)
                        {
                            var data = StringExtensions.ToEscape(
                                Frame.User.Auth.CreateSecurityChallenge(SSP.state.SSP_SEC));
                            var reply = Raws.Create(Frame.Server, Raw: Raws.RPL_AUTH_SEC_REPLY,
                                Data: new[] {Frame.Message.Data[0], data});
                            Frame.User.Send(reply);
                        }
                        else
                        {
                            // Authentication failed
                            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                                Raw: Raws.IRCX_ERR_AUTHENTICATIONFAILED_910, Data: new[] {Frame.Message.Data[0]}));
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
                    Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                        Raw: Raws.IRCX_ERR_ALREADYAUTHENTICATED_909, Data: new[] {Frame.Message.Data[0]}));
                }
                else
                {
                    var State = Frame.User.Auth.AcceptSecurityContext(Frame.Message.Data[2], Program.Config.ExternalIP);
                    if (State == SSP.state.SSP_OK)
                    {
                        if (Frame.User.Auth.Signature == GateKeeperPassport.SIGNATURE)
                        {
                            Frame.User.Properties.Puid.Value = ((GateKeeperPassport) Frame.User.Auth).puid;
                            if (Frame.User.Modes.Secure.Value == 0x1)
                                Frame.User.Profile.Registered = true; // For subscriber
                            Frame.User.Properties.MsnRegCookie.SetPermissions(UserAccessLevel.None,
                                UserAccessLevel.None, false, true);
                            Frame.User.Profile.UserType = ProfileUserType.Registered;
                            Frame.User.Level = UserAccessLevel.ChatUser;
                        }

                        Frame.User.Address.Userhost =
                            new string(StringExtensions.FromBytes(Frame.User.Auth.uuid).ToString());
                        Frame.User.Address.Hostname = Frame.User.Auth.GetDomain();

                        var reply = Raws.Create(Frame.Server, Raw: Raws.RPL_AUTH_SUCCESS,
                            Data: new[] {Frame.Message.Data[0], Frame.User.Address._address[1]},
                            IData: new[] {(int) Frame.User.OID});
                        Frame.User.Send(reply);
                    }
                    else if (State == SSP.state.SSP_CREDENTIALS)
                    {
                        Frame.User.Send(Raws.Create(Frame.Server, Raw: Raws.RPL_AUTH_SEC_REPLY,
                            Data: new[] {Frame.Message.Data[0], Resources.S_OK}));
                    }
                    else
                    {
                        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_AUTHENTICATIONFAILED_910, Data: new[] {Frame.Message.Data[0]}));
                    }
                }
            }
        }

        return COM_RESULT.COM_SUCCESS;
    }
}