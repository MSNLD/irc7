using Irc.Enumerations;

namespace Irc.Commands;

internal class Nick : Command, ICommand
{
    public Nick()
    {
        _requiredMinimumParameters = 1;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public void Execute(ChatFrame chatFrame)
    {
        // Is user not registered?
        // Set nickname according to regulations (should be available in user object and changes based on what they authenticated as)
        if (!chatFrame.User.Registered) HandleUnregisteredNicknameChange(chatFrame);
        else HandleRegisteredNicknameChange(chatFrame);
    }

    private bool HandleUnregisteredNicknameChange(ChatFrame chatFrame)
    {
        chatFrame.User.Address.Nickname = chatFrame.Message.Parameters.First();
        chatFrame.User.Name = chatFrame.User.Address.Nickname;
        return true;
    }

    private bool HandleRegisteredNicknameChange(ChatFrame chatFrame)
    {
        chatFrame.User.Address.Nickname = chatFrame.Message.Parameters.First();
        chatFrame.User.Name = chatFrame.User.Address.Nickname;
        return true;
    }
}