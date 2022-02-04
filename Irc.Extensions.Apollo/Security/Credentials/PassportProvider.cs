using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.NTLM.Cryptography;
using Irc.Extensions.Security;
using Irc.Helpers.CSharpTools;
using Irc.Security;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Irc.Extensions.Apollo.Security.Credentials;

public class PassportCredentials
{
    public string Ticket;
    public string Profile;
    public string RegCookie;

    public byte[] Id;
    public DateTime Expiry;
    public string Nickname;
}

public class PassportProvider : ICredentialProvider
{
    private const string Key = "DEFAULT_KEY";

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

        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
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
        MD5 md5 = MD5.Create();
        var secretHash = md5.ComputeHash(Key.ToByteArray());

        SHA256 sha256 = new SHA256Managed();
        byte[] key = sha256.ComputeHash(secretHash);

        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);

        long unixSeconds = ((DateTimeOffset) expiry).ToUnixTimeSeconds();
        adjustedUnixExpiry = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).Ticks;
        return securityKey;
    }

    public PassportCredentials Decrypt(PassportCredentials passportCredentials)
    {
        var ticket = Base64.Decode(passportCredentials.Ticket, Base64.B64MapType.MSPassport);
        var profile = Base64.Decode(passportCredentials.Profile, Base64.B64MapType.MSPassport);

        string jwtHeader =
            "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9";
        string jwtToken = $"{jwtHeader}.{ticket}.{profile}";

        MD5 md5 = MD5.Create();
        var secretHash = md5.ComputeHash(Key.ToByteArray());

        SHA256 sha256 = new SHA256Managed();
        byte[] key = sha256.ComputeHash(secretHash);

        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);
        
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
            (securityKey, SecurityAlgorithms.HmacSha256Signature);

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(jwtToken);

        var claims = jwtSecurityToken.Claims.ToList();
        byte[] cipherPuid = claims[0].Value.ToByteArray();

        byte[] dtBytes = BitConverter.GetBytes(jwtSecurityToken.ValidTo.Ticks);
        cipherPuid = RC4.Apply(cipherPuid, dtBytes);

        passportCredentials.Expiry = jwtSecurityToken.ValidTo;
        passportCredentials.Id = cipherPuid;

        return passportCredentials;
    }

    public ICredential ValidateTokens(Dictionary<string, string> tokens)
    {
        //var ticket = Decrypt(tokens["ticket"].Substring(1));
        //var profile = Decrypt(tokens["profile"].Substring(1));

        //if (ticket == null || profile == null) return null;

        return new Credential()
        {
            //Username = ticket.puid,
            Domain = GetType().Name
        };
    }

    public ICredential GetUserCredentials(string domain, string username)
    {
        throw new NotImplementedException();
    }
}