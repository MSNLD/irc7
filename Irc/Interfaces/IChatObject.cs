using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IChatObject
{
    public Guid Id { get; }
    public EnumUserAccessLevel Level { get; }
    public IModeCollection Modes { get; }
    public string Name { get; set; }
    public string ShortId { get; }

    IModeCollection GetModes();
    public void Send(string message);
    public void Send(string message, ChatObject except = null);
    public string ToString();
}