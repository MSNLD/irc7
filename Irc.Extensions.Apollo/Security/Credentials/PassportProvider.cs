using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.NTLM.Cryptography;
using Irc.Extensions.Security;
using Irc.Helpers.CSharpTools;
using Irc.Security;
using Microsoft.IdentityModel.Tokens;

namespace Irc.Extensions.Apollo.Security.Credentials;

public class PassportCredentials
{
    public DateTime Expiry;

    public byte[] Id;
    public string Nickname;
    public string Profile;
    public string RegCookie;
    public string Ticket;
}

public class PassportProvider : ICredentialProvider
{
    private const string Key = "DEFAULT_KEY";

    public ICredential ValidateTokens(Dictionary<string, string> tokens)
    {
        //var ticket = Decrypt(tokens["ticket"].Substring(1));
        //var profile = Decrypt(tokens["profile"].Substring(1));

        //if (ticket == null || profile == null) return null;

        return new Credential
        {
            //Username = ticket.puid,
            Domain = GetType().Name
        };
    }

    public ICredential GetUserCredentials(string domain, string username)
    {
        throw new NotImplementedException();
    }

    public PassportCredentials Encrypt(PassportCredentials passportCredentials)
    {
        var securityKey = RC4IdWithExpiryDate(ref passportCredentials.Id, passportCredentials.Expiry);

        var jwtToken = CreateJwtSecurityToken(passportCredentials.Id, passportCredentials.Expiry, securityKey);

        var parts = jwtToken.Split('.');
        var ticket = Base64.Encode(parts[1], Base64.B64MapType.MSPassport);
        var profile = Base64.Encode(parts[2], Base64.B64MapType.MSPassport);

        passportCredentials.Ticket = ticket;
        passportCredentials.Profile = profile;

        return passportCredentials;
    }

    private static string CreateJwtSecurityToken(byte[] id, DateTime expiry, SymmetricSecurityKey securityKey)
    {
        var claims = new[] {new Claim("id", id.ToAsciiString())};

        var credentials = new SigningCredentials
            (securityKey, SecurityAlgorithms.HmacSha256Signature);

        var secToken = new JwtSecurityToken(
            null,
            null,
            claims,
            DateTime.UtcNow.AddDays(-1),
            expiry,
            credentials
        );
        var handler = new JwtSecurityTokenHandler();

        // Token to String so you can use it in your client
        var jwtToken = handler.WriteToken(secToken);
        return jwtToken;
    }

    private static SymmetricSecurityKey RC4IdWithExpiryDate(ref byte[] id, DateTime expiry)
    {
        var securityKey = CreateKeySecurityKey(expiry, out var adjustedUnixExpiry);
        id = RC4.Apply(id, BitConverter.GetBytes(adjustedUnixExpiry));
        return securityKey;
    }

    private static SymmetricSecurityKey CreateKeySecurityKey(DateTime expiry, out long adjustedUnixExpiry)
    {
        var md5 = MD5.Create();
        var secretHash = md5.ComputeHash(Key.ToByteArray());

        SHA256 sha256 = new SHA256Managed();
        var key = sha256.ComputeHash(secretHash);

        var securityKey = new SymmetricSecurityKey(key);

        var unixSeconds = ((DateTimeOffset) expiry).ToUnixTimeSeconds();
        adjustedUnixExpiry = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).Ticks;
        return securityKey;
    }

    public PassportCredentials Decrypt(PassportCredentials passportCredentials)
    {
        var ticket = Base64.Decode(passportCredentials.Ticket, Base64.B64MapType.MSPassport);
        var profile = Base64.Decode(passportCredentials.Profile, Base64.B64MapType.MSPassport);

        var jwtHeader =
            "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9";
        var jwtToken = $"{jwtHeader}.{ticket}.{profile}";

        var md5 = MD5.Create();
        var secretHash = md5.ComputeHash(Key.ToByteArray());

        SHA256 sha256 = new SHA256Managed();
        var key = sha256.ComputeHash(secretHash);

        var securityKey = new SymmetricSecurityKey(key);

        var credentials = new SigningCredentials
            (securityKey, SecurityAlgorithms.HmacSha256Signature);

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(jwtToken);

        var claims = jwtSecurityToken.Claims.ToList();
        var cipherPuid = claims[0].Value.ToByteArray();

        var dtBytes = BitConverter.GetBytes(jwtSecurityToken.ValidTo.Ticks);
        cipherPuid = RC4.Apply(cipherPuid, dtBytes);

        passportCredentials.Expiry = jwtSecurityToken.ValidTo;
        passportCredentials.Id = cipherPuid;

        return passportCredentials;
    }
}