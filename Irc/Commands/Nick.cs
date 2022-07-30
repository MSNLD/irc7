using Irc.Enumerations;

namespace Irc.Commands;

internal class Nick : Command, ICommand
{
    public Nick() : base(1) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.Standard;

    public new void Execute(ChatFrame chatFrame)
    {
        // Is user not registered?
        // Set nickname according to regulations (should be available in user object and changes based on what they authenticated as)
        if (!chatFrame.User.IsRegistered()) HandleUnregisteredNicknameChange(chatFrame);
        else HandleRegisteredNicknameChange(chatFrame);
    }

    private bool HandleUnregisteredNicknameChange(ChatFrame chatFrame)
    {
        chatFrame.User.GetAddress().Nickname = chatFrame.Message.Parameters.First();
        chatFrame.User.Name = chatFrame.User.GetAddress().Nickname;
        return true;
    }

    private bool HandleRegisteredNicknameChange(ChatFrame chatFrame)
    {
        chatFrame.User.GetAddress().Nickname = chatFrame.Message.Parameters.First();
        chatFrame.User.Name = chatFrame.User.GetAddress().Nickname;
        return true;
    }
}