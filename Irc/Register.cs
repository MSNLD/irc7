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