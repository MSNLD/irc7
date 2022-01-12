namespace Irc.Extensions.Apollo.Security.Credentials;

public class RegCookie
{
    public long issueDate;
    public string nickname;
    public string salt;
    public int version;

    public RegCookie(int version)
    {
        this.version = version;
    }
}