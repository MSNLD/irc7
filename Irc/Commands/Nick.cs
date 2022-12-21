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

    private bool HandleUnregisteredNicknameChange(IChatFrame chatFrame)
    {
        chatFrame.User.GetAddress().Nickname = chatFrame.Message.Parameters.First();
        chatFrame.User.Name = chatFrame.User.GetAddress().Nickname;
        return true;
    }

    private bool HandleRegisteredNicknameChange(IChatFrame chatFrame)
    {
        chatFrame.User.GetAddress().Nickname = chatFrame.Message.Parameters.First();
        chatFrame.User.Name = chatFrame.User.GetAddress().Nickname;
        return true;
    }
}