using Irc.Objects;

namespace Irc.Extensions.Interfaces;

internal interface IExtendedServerObject
{
    void ProcessCookie(IUser user, string name, string value);
}