using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Security.Packages;

namespace Irc.Commands
{
    public class Auth : Command, ICommand
    {
        public Auth() : base(3, false)
        {
        }

        public new EnumCommandDataType GetDataType()
        {
            return EnumCommandDataType.None;
        }

        public new void Execute(IChatFrame chatFrame)
        {
            if (chatFrame.User.IsRegistered())
            {
                chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
            }
            else if (chatFrame.User.IsAuthenticated())
            {
                chatFrame.User.Send(Raw.IRCX_ERR_ALREADYAUTHENTICATED_909(chatFrame.Server, chatFrame.User));
            }
            else
            {
                var parameters = chatFrame.Message.Parameters;

                var supportPackage = chatFrame.User.GetSupportPackage();
                var packageName = parameters[0];
                var sequence = parameters[1].ToUpper();
                var token = parameters[2].ToLiteral();

                if (sequence == "I")
                {
                    var targetPackage = chatFrame.Server.GetSecurityManager()
                        .CreatePackageInstance(packageName, chatFrame.Server.GetCredentialManager());

                    if (targetPackage == null)
                    {
                        chatFrame.User.Send(Raw.IRCX_ERR_UNKNOWNPACKAGE_912(chatFrame.Server, chatFrame.User, packageName));
                        return;
                    }

                    if (supportPackage == null || supportPackage is ANON)
                    {
                        supportPackage = chatFrame.Server.GetSecurityManager()
                            .CreatePackageInstance(packageName, chatFrame.Server.GetCredentialManager());

                        chatFrame.User.SetSupportPackage(supportPackage);

                        var supportPackageSequence =
                            supportPackage.InitializeSecurityContext(token, chatFrame.Server.RemoteIp);

                        if (supportPackageSequence == EnumSupportPackageSequence.SSP_OK)
                        {
                            var securityToken = supportPackage.CreateSecurityChallenge().ToEscape();
                            chatFrame.User.Send(Raw.RPL_AUTH_SEC_REPLY(packageName, securityToken));
                            // Send reply
                            return;
                        }
                    }
                }
                else if (sequence == "S")
                {
                    var supportPackageSequence =
                        chatFrame.User.GetSupportPackage().AcceptSecurityContext(token, chatFrame.Server.RemoteIp);
                    if (supportPackageSequence == EnumSupportPackageSequence.SSP_OK)
                    {
                        chatFrame.User.Authenticate();

                        var credentials = chatFrame.User.GetSupportPackage().GetCredentials();
                        if (credentials == null)
                        {
                            // Invalid credentials handle
                        }
                        else
                        {
                            var user = chatFrame.User.GetSupportPackage().GetCredentials().GetUsername();
                            var domain = chatFrame.User.GetSupportPackage().GetCredentials().GetDomain();

                            var userAddress = chatFrame.User.GetAddress();
                            chatFrame.User.Name = credentials.GetNickname();
                            userAddress.User = credentials.GetUsername();
                            userAddress.Host = credentials.GetDomain();
                            userAddress.Server = chatFrame.Server.RemoteIp;
                            userAddress.RealName = string.Empty;

                            // Send reply
                            chatFrame.User.Send(Raw.RPL_AUTH_SUCCESS(packageName, $"{user}@{domain}", 0));
                        }

                        return;
                    }

                    if (supportPackageSequence == EnumSupportPackageSequence.SSP_CREDENTIALS)
                    {
                        chatFrame.User.Send(Raw.RPL_AUTH_SEC_REPLY(packageName, "OK"));
                        return;
                    }
                }

                // auth failed
                chatFrame.User.Disconnect(
                    Raw.IRCX_ERR_AUTHENTICATIONFAILED_910(chatFrame.Server, chatFrame.User, packageName));
            }
        }
    }
}