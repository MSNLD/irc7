namespace Irc.Extensions.Apollo.Security.Credentials;

public class Ticket
{
    public long issueDate;
    public byte[] iv;
    public string puid;
    public int version;

    public Ticket(int version)
    {
        this.version = version;
    }
}