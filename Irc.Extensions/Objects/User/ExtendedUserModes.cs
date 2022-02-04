using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Objects;

namespace Irc.Extensions.Objects.User
{
    public class ExtendedUserModes: UserModes
    {
        public ExtendedUserModes()
        {
            modes.Add(ExtendedResources.UserModeIrcx, 0);
            modes.Add(ExtendedResources.UserModeGag, 0);
        }
    }
}
