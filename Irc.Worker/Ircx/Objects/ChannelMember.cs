using Irc.Extensions.Access;

namespace Irc.Worker.Ircx.Objects;

public class ChannelMember
{
    public ChanUserMode ChannelMode;
    private UserAccessLevel level;
    public User User;

    public ChannelMember(User User)
    {
        ChannelMode = new ChanUserMode();
        level = User.Level;
        if (level >= UserAccessLevel.ChatGuide) ChannelMode.SetAdmin(true);
        this.User = User;
    }

    public UserAccessLevel Level
    {
        get
        {
            if (User.Level >= UserAccessLevel.ChatGuide) return User.Level;

            if (ChannelMode.IsOwner())
                return UserAccessLevel.ChatOwner;
            if (ChannelMode.IsHost())
                return UserAccessLevel.ChatHost;
            return level;
            //OR just a normal level explaining what the user is
        }
        set => level = value;
    }
}