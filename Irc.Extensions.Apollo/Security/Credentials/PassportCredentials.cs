namespace Irc.Extensions.Apollo.Security.Credentials;

public class PassportCredentials
{
    public long IssuedAt { get; set; }
    public string PUID { get; set; }
    public string ProfileId { get; set; }
    public string Domain { get; set; }
}