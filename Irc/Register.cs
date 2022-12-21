namespace Irc;

public static class Register
{
    public static void Execute(ChatFrame chatFrame)
    {
        if (CanRegister(chatFrame))
        {
            chatFrame.User.Register();
            chatFrame.User.Send(Raw.IRCX_RPL_WELCOME_001(chatFrame.Server, chatFrame.User));
            chatFrame.User.Send(Raw.IRCX_RPL_WELCOME_002(chatFrame.Server, chatFrame.User,
                chatFrame.Server.GetVersion()));
            chatFrame.User.Send(Raw.IRCX_RPL_WELCOME_003(chatFrame.Server, chatFrame.User));
            chatFrame.User.Send(Raw.IRCX_RPL_WELCOME_004(chatFrame.Server, chatFrame.User,
                chatFrame.Server.GetVersion()));

            chatFrame.User.Send(Raw.IRCX_RPL_LUSERCLIENT_251(chatFrame.Server, chatFrame.User, 0, 0, 0));
            chatFrame.User.Send(Raw.IRCX_RPL_LUSEROP_252(chatFrame.Server, chatFrame.User, 0));
            chatFrame.User.Send(Raw.IRCX_RPL_LUSERUNKNOWN_253(chatFrame.Server, chatFrame.User, 0));
            chatFrame.User.Send(Raw.IRCX_RPL_LUSERCHANNELS_254(chatFrame.Server, chatFrame.User));
            chatFrame.User.Send(Raw.IRCX_RPL_LUSERME_255(chatFrame.Server, chatFrame.User, 0, 1));
            chatFrame.User.Send(Raw.IRCX_RPL_LUSERS_265(chatFrame.Server, chatFrame.User,
                chatFrame.Server.GetUsers().Count, 10000));
            chatFrame.User.Send(Raw.IRCX_RPL_GUSERS_266(chatFrame.Server, chatFrame.User,
                chatFrame.Server.GetUsers().Count, 10000));

            var motd = chatFrame.Server.GetMotd();
            if (motd == null)
            {
                chatFrame.User.Send(Raw.IRCX_ERR_NOMOTD_422(chatFrame.Server, chatFrame.User));
            }
            else
            {
                chatFrame.User.Send(Raw.IRCX_RPL_RPL_MOTDSTART_375(chatFrame.Server, chatFrame.User));

                foreach (var line in motd)
                    chatFrame.User.Send(Raw.IRCX_RPL_RPL_MOTD_372(chatFrame.Server, chatFrame.User, line));

                chatFrame.User.Send(Raw.IRCX_RPL_RPL_ENDOFMOTD_376(chatFrame.Server, chatFrame.User));
            }

            var pass = chatFrame.User.GetDataStore().Get("pass");

            if (pass == "guide")
                chatFrame.User.PromoteToGuide();
            else if (pass == "sysop")
                chatFrame.User.PromoteToSysop();
            else if (pass == "admin") chatFrame.User.PromoteToAdministrator();
        }
    }

    public static bool CanRegister(ChatFrame chatFrame)
    {
        var authenticating = chatFrame.User.IsAuthenticated() != true && chatFrame.User.IsAnon() == false;
        var registered = chatFrame.User.IsRegistered();
        var hasNickname = !string.IsNullOrWhiteSpace(chatFrame.User.GetAddress().Nickname);
        var hasAddress = chatFrame.User.GetAddress().IsAddressPopulated();

        return !authenticating && !registered & hasNickname & hasAddress;
    }
}