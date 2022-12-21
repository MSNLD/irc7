using Irc.Extensions.Security.Credentials;
using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc7d;

internal class NTLMCredentials : NtlmProvider, ICredentialProvider
{
    private readonly Dictionary<string, ICredential> _credentials = new();

    public NTLMCredentials()
    {
        _credentials.Add(@"DOMAIN\username", new Credential
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

    public new ICredential ValidateTokens(Dictionary<string, string> tokens)
    {
        throw new NotImplementedException();
    }

    public new ICredential GetUserCredentials(string domain, string username)
    {
        _credentials.TryGetValue($"{domain}\\{username}", out var credential);
        return credential;
    }
}