using Irc.Constants;
using Irc.Enumerations;
using Irc.Helpers.CSharpTools;

namespace Irc.Commands;

public class Nick : Command, ICommand
{
    public Nick() : base(1, false) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.Standard;

    public new void Execute(ChatFrame chatFrame)
    {
        var hopcount = string.Empty;
        if (chatFrame.Message.Parameters.Count > 1) { hopcount = chatFrame.Message.Parameters[1]; }

        // Is user not registered?
        // Set nickname according to regulations (should be available in user object and changes based on what they authenticated as)
        if (!chatFrame.User.IsRegistered()) HandleUnregisteredNicknameChange(chatFrame);
        else HandleRegisteredNicknameChange(chatFrame);
    }

    private bool ValidateNickname(string nickname, EnumUserAccessLevel level) {

        var nickMask = Resources.NicknameMask;

        if (level == EnumUserAccessLevel.Guest) {
            nickMask = Resources.GuestNicknameMask;
        }
        else if (level >= EnumUserAccessLevel.Guide) {
            nickMask = Resources.OperMask;
        }

        return nickname.Length <= Resources.MaxFieldLen &&
         RegularExpressions.Match(nickMask, nickname, true);
    }

    private bool HandleUnregisteredNicknameChange(ChatFrame chatFrame)
    {
        var nickname = chatFrame.Message.Parameters.First();
        if (!ValidateNickname(nickname, chatFrame.User.GetLevel())) {
            chatFrame.User.Send(Raw.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        chatFrame.User.Nickname = nickname;
        return true;
    }

    private bool HandleRegisteredNicknameChange(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var nickname = chatFrame.Message.Parameters.First();
        if (!ValidateNickname(nickname, user.GetLevel())) {
            chatFrame.User.Send(Raw.IRCX_ERR_ERRONEOUSNICK_432(server, user, nickname));
            return false;
        }

        var channels = user.GetChannels();
        foreach (var channel in channels) {
            foreach (var member in channel.Key.GetMembers()) {
                if (member.GetUser().Nickname == nickname) {
                    chatFrame.User.Send(Raw.IRCX_ERR_NICKINUSE_433(server, user));
                    return false;
                }
            }
        }

        user.ChangeNickname(nickname, user.GetLevel() > EnumUserAccessLevel.Guide);
        return true;
    }
}