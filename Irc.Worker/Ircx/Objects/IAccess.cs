namespace Irc.Worker.Ircx.Objects;

public interface IAccess
{
    AccessLevel ResolveAccessLevel(string Data);
    EnumAccessOperator ResolveAccessOperator(string Data);
    AccessObjectResult GetAccess(Address Mask);
}