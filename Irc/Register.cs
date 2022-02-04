namespace Irc;

internal static class Register
{
    public static void Execute(ChatFrame chatFrame)
    {
        if (CanRegister(chatFrame))
        {
            chatFrame.User.Registered = true;
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
        var authenticating = chatFrame.User.Authenticated != true && chatFrame.User.IsAnon() == false;
        var registered = chatFrame.User.Registered;
        var hasNickname = !string.IsNullOrWhiteSpace(chatFrame.User.Address.Nickname);
        var hasAddress = chatFrame.User.Address.IsAddressPopulated();

        return !authenticating && !registered & hasNickname & hasAddress;
    }
}