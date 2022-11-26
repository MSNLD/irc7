using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Interfaces
{
    internal interface IExtendedServerObject
    {
        void ProcessCookie(IUser user, string name, string value);
    }
}
