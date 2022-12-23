using System.Text.RegularExpressions;
using Irc.Constants;
using Irc.Interfaces;
using Irc.Models.Enumerations;

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

    private bool ValidateNickname(string nickname)
    {
        return Regex.Match(nickname, Resources.NicknameMask).Success;
    }

    private bool HandleUnregisteredNicknameChange(IChatFrame chatFrame)
    {
        var newNick = chatFrame.Message.Parameters.First();

        var isValid = ValidateNickname(newNick);
        if (!isValid)
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, newNick));
            // Invalid Nickname
            return false;
        }
        
        chatFrame.User.GetAddress().Nickname = chatFrame.Message.Parameters.First();
        chatFrame.User.Name = chatFrame.User.GetAddress().Nickname;
        return true;
    }

    private bool HandleRegisteredNicknameChange(IChatFrame chatFrame)
    {
        var newNick = chatFrame.Message.Parameters.First();

        var isValid = ValidateNickname(newNick);
        if (!isValid)
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, newNick));
            // Invalid Nickname
            return false;
        }
        
        var channels = chatFrame.User.GetChannels();
        var inUse = channels.Count(kvp => kvp.Key.GetMemberByNickname(newNick) != null);
        if (inUse > 0)
        {
            chatFrame.User.Send(Raw.IRCX_ERR_NICKINUSE_433(chatFrame.User.Server, chatFrame.User, newNick));
            // Nickname in use
            return false;
        }

        var raw = Raw.RPL_NICK(chatFrame.Server, chatFrame.User, newNick);
        chatFrame.User.GetAddress().Nickname = newNick;
        chatFrame.User.Name = newNick;
        chatFrame.User.Send(raw);
        chatFrame.User.BroadcastToChannels(raw, true);
        return true;
    }
}