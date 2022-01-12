using System.Text;
using Irc.ClassExtensions.CSharpTools;

namespace Irc.Extensions.Apollo.Security.Credentials;

public interface ICredentialProvider
{
    string Encrypt(object o, Base64.B64MapType BaseMap, bool IncludeVersion);
    string Encrypt(object o, byte[] iv, Base64.B64MapType BaseMap, bool IncludeVersion);
    Ticket Decrypt(StringBuilder cookie);
    Profile Decrypt(StringBuilder cookie, byte[] IV);
    RegCookie DecryptRegCookie(string cookie);
    string CreatePassportID(string provider, string id);
}