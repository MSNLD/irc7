using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Security;

namespace Irc.Extensions.Security.Credentials
{
    public class NtlmProvider: ICredentialProvider
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
