using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Extensions.Apollo.Security.Credentials;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Security;
using Irc.Helpers;
using Irc.Helpers.CSharpTools;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class AUTH : Command
{
    //AUTH <Package> <Seg> <Token>
    public AUTH(CommandCode Code) : base(Code)
    {
        MinParamCount = 3;
        DataType = CommandDataType.None;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters[1] == "I")
        {
            if (Frame.User.Auth == null)
            {
                if (Frame.User.Registered)
                {
                    // You are already authenticated
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                        Raw: Raws.IRCX_ERR_ALREADYAUTHENTICATED_909, Data: new[] {Frame.Message.Parameters[0]}));
                }
                else
                {
                    Frame.User.Auth = Program.Providers.CreatePackageInstance(Frame.Message.Parameters[0], new Passport(Program.Config.PassportKey));
                    if (Frame.User.Auth == null)
                    {
                        // No such authentication package
                        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_UNKNOWNPACKAGE_912, Data: new[] {Frame.Message.Parameters[0]}));
                    }
                    else
                    {
                        if (Frame.User.Auth.InitializeSecurityContext(Frame.Message.Parameters[2],
                                Program.Config.ExternalIP) == SupportPackage.EnumSupportPackageSequence.SSP_OK)
                        {
                            var data = StringExtensions.ToEscape(
                                Frame.User.Auth.CreateSecurityChallenge(SupportPackage.EnumSupportPackageSequence.SSP_SEC));
                            var reply = RawBuilder.Create(Frame.Server, Raw: Raws.RPL_AUTH_SEC_REPLY,
                                Data: new[] {Frame.Message.Parameters[0], data});
                            Frame.User.Send(reply);
                        }
                        else
                        {
                            // Authentication failed
                            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                                Raw: Raws.IRCX_ERR_AUTHENTICATIONFAILED_910, Data: new[] {Frame.Message.Parameters[0]}));
                        }
                    }
                }
            }
        }
        else if (Frame.Message.Parameters[1] == "S")
        {
            if (Frame.User.Auth != null)
            {
                if (Frame.User.Registered)
                {
                    // You are already authenticated
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                        Raw: Raws.IRCX_ERR_ALREADYAUTHENTICATED_909, Data: new[] {Frame.Message.Parameters[0]}));
                }
                else
                {
                    var State = Frame.User.Auth.AcceptSecurityContext(Frame.Message.Parameters[2], Program.Config.ExternalIP);
                    if (State == SupportPackage.EnumSupportPackageSequence.SSP_OK)
                    {
                        if (Frame.User.Auth is GateKeeperPassport)
                        {
                            Frame.User.Properties.Set("Puid", ((GateKeeperPassport) Frame.User.Auth).Puid);
                            if (Frame.User.Modes.Secure.Value == 0x1)
                                Frame.User.Profile.Registered = true; // For subscriber

                            // TODO: Fix below to remove set permissions
                            //Frame.User.Properties.Properties["MsnRegCookie"].SetPermissions(UserAccessLevel.None,
                            //    UserAccessLevel.None, false, true);
                            Frame.User.Profile.UserType = ProfileUserType.Registered;
                            Frame.User.Level = UserAccessLevel.ChatUser;
                        }

                        Frame.User.Address.User =
                            new string(Frame.User.Auth.Guid.ToUnformattedString().ToUpper());
                        Frame.User.Address.Host = Frame.User.Auth.GetDomain();

                        var reply = RawBuilder.Create(Frame.Server, Raw: Raws.RPL_AUTH_SUCCESS,
                            Data: new[] {Frame.Message.Parameters[0], Frame.User.Address.GetUserHost()},
                            IData: new[] { 0 });
                        Frame.User.Send(reply);
                    }
                    else if (State == SupportPackage.EnumSupportPackageSequence.SSP_CREDENTIALS)
                    {
                        Frame.User.Send(RawBuilder.Create(Frame.Server, Raw: Raws.RPL_AUTH_SEC_REPLY,
                            Data: new[] {Frame.Message.Parameters[0], Resources.S_OK}));
                    }
                    else
                    {
                        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                            Raw: Raws.IRCX_ERR_AUTHENTICATIONFAILED_910, Data: new[] {Frame.Message.Parameters[0]}));
                    }
                }
            }
        }

        return true;
    }
}