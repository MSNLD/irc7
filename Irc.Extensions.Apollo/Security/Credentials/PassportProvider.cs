using Irc.Extensions.Apollo.Security.Passport;
using Irc.Security;

namespace Irc.Extensions.Apollo.Security.Credentials;

public class PassportProvider : ICredentialProvider
{
    private readonly string appid;
    private readonly PassportV4 passportV4;
    private readonly string secret;

    public PassportProvider(PassportV4 passportV4)
    {
        appid = appid;
        secret = secret;
        this.passportV4 = passportV4;
    }

    public ICredential GetUserCredentials(string domain, string username)
    {
        throw new NotImplementedException();
    }

    public ICredential ValidateTokens(Dictionary<string, string> tokens)
    {
        var ticket = tokens["ticket"];
        var profile = tokens["profile"];

        var passportCredentials = passportV4.ValidateTicketAndProfile(ticket, profile);

        if (passportCredentials == null) return null;

        var credential = new Credential();
        credential.Username = passportCredentials.PUID; //.Substring(16).ToUpper();
        credential.Domain = passportCredentials.Domain;
        credential.IssuedAt = passportCredentials.IssuedAt;
        //credential.Domain = "GateKeeperPassport";
        return credential;
    }
}