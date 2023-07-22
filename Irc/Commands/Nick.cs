using Irc.Constants;
using Irc.Enumerations;
using Irc.Helpers.CSharpTools;
using Irc.Interfaces;

namespace Irc.Commands;

public class Nick : Command, ICommand
{
    public Nick() : base(1, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        var hopcount = string.Empty;
        if (chatFrame.Message.Parameters.Count > 1) hopcount = chatFrame.Message.Parameters[1];

        // Is user not registered?
        // Set nickname according to regulations (should be available in user object and changes based on what they authenticated as)
        if (!chatFrame.User.IsRegistered()) HandleUnregisteredNicknameChange(chatFrame);
        else HandleRegisteredNicknameChange(chatFrame);
    }

    public static bool ValidateNickname(string nickname, bool registered, bool guest, bool utf8)
    {
        var nickMask = utf8 ? Resources.StandardUtf8Nickname : Resources.StandardNickname;

        if (!registered)
            nickMask = utf8 ? Resources.AnyUtf8Nickname : Resources.AnyNickname;
        else if (guest) nickMask = Resources.GuestNicknameMask;

        return nickname.Length <= Resources.MaxFieldLen &&
               RegularExpressions.Match(nickMask, nickname, true);
    }

    private bool HandleUnregisteredNicknameChange(IChatFrame chatFrame)
    {
        var nickname = chatFrame.Message.Parameters.First();
        if (!ValidateNickname(nickname, false, chatFrame.User.IsGuest(), chatFrame.User.Utf8))
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        chatFrame.User.Nickname = nickname;
        return true;
    }

    private bool HandleRegisteredNicknameChange(IChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var nickname = chatFrame.Message.Parameters.First();

        if (!user.IsGuest())
        {
            chatFrame.User.Send(Raw.IRCX_ERR_NONICKCHANGES_439(server, user, nickname));
            return false;
        }

        if (!ValidateNickname(nickname, true,
                chatFrame.User.IsGuest() && chatFrame.User.GetLevel() < EnumUserAccessLevel.Guide, chatFrame.User.Utf8))
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ERRONEOUSNICK_432(server, user, nickname));
            return false;
        }

        var channels = user.GetChannels();
        foreach (var channel in channels)
        foreach (var member in channel.Key.GetMembers())
            if (member.GetUser().Nickname == nickname)
            {
                chatFrame.User.Send(Raw.IRCX_ERR_NICKINUSE_433(server, user));
                return false;
            }

        user.ChangeNickname(nickname, user.GetLevel() > EnumUserAccessLevel.Guide);
        return true;
    }
}