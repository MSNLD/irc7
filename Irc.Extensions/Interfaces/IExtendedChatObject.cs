using Irc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Interfaces
{
    public interface IExtendedChatObject : IChatObject
    {
        IPropCollection PropCollection { get; }
    }
}
