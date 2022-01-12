namespace Irc.Extensions.Apollo.Security.Credentials;

public class Profile
{
    public string origId;
    public string profileId;
    public string provider;
    public int version;

    public Profile(int version)
    {
        this.version = version;
    }
}