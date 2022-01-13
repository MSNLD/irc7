namespace Irc.Interfaces
{
    public interface IPropStore
    {
        string? Get(string key);
        void Set(string key, string value);

        List<KeyValuePair<string, string>> GetList();
    }
}
