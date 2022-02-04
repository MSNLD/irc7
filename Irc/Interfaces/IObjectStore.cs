namespace Irc.Worker.Ircx.Objects;

public interface IObjectStore
{
    T Get<T>(string name);
    void Set<T>(string name, T objectValue);
}