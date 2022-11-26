using Irc.Extensions.Security;
using Irc.Extensions.Security.Credentials;
using Irc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc7d
{
    internal class NTLMCredentials: NtlmProvider, ICredentialProvider
    {
        Dictionary<string, ICredential> credentials = new Dictionary<string, ICredential>();

        public NTLMCredentials()
        {
            credentials.Add(@"DOMAIN\username", new Credential()
            {
                Domain = "DOMAIN",
                Username = "username",
                Password = "password",
                Nickname = "username",
                UserGroup = "group",
                Modes = "a",
                Level = Irc.Enumerations.EnumUserAccessLevel.Administrator
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
}
