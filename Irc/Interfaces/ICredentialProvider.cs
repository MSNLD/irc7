using Irc.Extensions.Security;

namespace Irc.Security;

public interface ICredentialProvider
{
    ICredential ValidateTokens(Dictionary<string, string> tokens);
    ICredential GetUserCredentials(string domain, string username);
}