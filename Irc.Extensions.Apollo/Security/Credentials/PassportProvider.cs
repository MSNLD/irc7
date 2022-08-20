using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.Apollo.Security.Passport;
using Irc.Extensions.NTLM.Cryptography;
using Irc.Extensions.Security;
using Irc.Helpers.CSharpTools;
using Irc.Security;
using Microsoft.IdentityModel.Tokens;

namespace Irc.Extensions.Apollo.Security.Credentials;



public class PassportProvider : ICredentialProvider
{
    private readonly string appid;
    private readonly string secret;
    private readonly PassportV4 passportV4;

    public PassportProvider(PassportV4 passportV4)
    {
        this.appid = appid;
        this.secret = secret;
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