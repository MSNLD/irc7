﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Irc.ClassExtensions.CSharpTools;
using Irc.Enumerations;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;
using Irc.Worker.Ircx.Objects;

namespace Irc.Commands
{
    internal class Auth : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Auth()
        {
            _requiredMinimumParameters = 3;
        }

        public void Execute(ChatFrame chatFrame)
        {
            if (chatFrame.User.Registered) chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
            else if (chatFrame.User.Authenticated) chatFrame.User.Send(Raw.IRCX_ERR_ALREADYAUTHENTICATED_909(chatFrame.Server, chatFrame.User));
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

                        EnumSupportPackageSequence supportPackageSequence =
                            supportPackage.InitializeSecurityContext(token, chatFrame.Server.RemoteIP);

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
                        chatFrame.User.GetSupportPackage().AcceptSecurityContext(token, chatFrame.Server.RemoteIP);
                    if (supportPackageSequence == EnumSupportPackageSequence.SSP_OK)
                    {
                        chatFrame.User.Authenticated = true;

                        var user = chatFrame.User.GetSupportPackage().GetCredentials().GetUsername();
                        var domain = chatFrame.User.GetSupportPackage().GetCredentials().GetDomain();

                        chatFrame.User.Send(Raw.RPL_AUTH_SUCCESS(packageName,  $"{user}@{domain}", 0));
                        // Send reply
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