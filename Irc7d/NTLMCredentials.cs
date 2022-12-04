using Irc.Enumerations;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Credentials;
using Irc.Security;

namespace Irc7d;

internal class NTLMCredentials : NtlmProvider, ICredentialProvider
{
    private readonly Dictionary<string, ICredential> credentials = new();

    public NTLMCredentials()
    {
        credentials.Add(@"DOMAIN\username", new Credential
        {
            Domain = "DOMAIN",
            Username = "username",
            Password = "password",
            Nickname = "username",
            UserGroup = "group",
            Modes = "a",
            Level = EnumUserAccessLevel.Administrator
        });
    }

    public ICredential ValidateTokens(Dictionary<string, string> tokens)
    {
        throw new NotImplementedException();
    }

    public ICredential GetUserCredentials(string domain, string username)
    {
        credentials.TryGetValue($"{domain}\\{username}", out var credential);
        return credential;
    }
}