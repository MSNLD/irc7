using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Extensions.Security;

namespace Irc.Security
{
    public abstract class CredentialProvider : ICredentialProvider
    {
        public ICredential ValidateTokens(Dictionary<string, string> tokens)
        {
            throw new NotImplementedException();
        }

        public ICredential GetUserCredentials(string domain, string username)
        {
            throw new NotImplementedException();
        }
    }
}
