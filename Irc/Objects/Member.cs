using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Worker.Ircx.Objects;

public class Member : IChannelMember
{
    private readonly IChatMemberModes _channelMode;
    private EnumUserAccessLevel _level;
    private readonly User _user;

    public Member(User User, IChatMemberModes channelMode)
    {
        _channelMode = channelMode;
        SetLevel(User.Level);
        
        if (_level >= EnumUserAccessLevel.ChatGuide) _channelMode.SetAdmin(true);
        this._user = User;
    }

    public EnumUserAccessLevel GetLevel()
    {
        if (_user.Level >= EnumUserAccessLevel.ChatGuide) return _user.Level;

        if (_channelMode.IsOwner())
            return EnumUserAccessLevel.ChatOwner;
        if (_channelMode.IsHost())
            return EnumUserAccessLevel.ChatHost;
        return _level;
        //OR just a normal level explaining what the user is
    }

    public void SetLevel(EnumUserAccessLevel level)
    {
        _level = level;
    }

    public IChatMemberModes GetChanUserMode()
    {
        return _channelMode;
    }

    public User GetUser()
    {
        return _user;
    }

}