using System.Text.Json.Serialization;

namespace Irc.Extensions.Security.Credentials;

public class RegCookie
{
    [JsonPropertyName("issueDate")]
    public long IssueDate;
    
    [JsonPropertyName("nickname")]
    public string Nickname;
    
    [JsonPropertyName("salt")]
    public string Salt;
    
    [JsonPropertyName("version")]
    public int Version;

    public RegCookie(int version)
    {
        this.Version = version;
    }
}