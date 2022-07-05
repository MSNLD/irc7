using System.Text.Json;

namespace Irc.IO;

public class DataStore : IDataStore
{
    private readonly bool _persist;
    private readonly string _section;
    private readonly Dictionary<string, string> _sets = new(StringComparer.InvariantCultureIgnoreCase);
    private string _id;

    public DataStore(string id, string section, bool persist = true)
    {
        _id = id;
        _section = section;
        _persist = persist;
    }

    public DataStore(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            _sets = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
        }
    }

    public void SetId(string id)
    {
        _id = id;
    }

    public void Set(string key, string value)
    {
        _sets[key] = value;
        Dump();
    }

    public void SetAs<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        Set(key, json);
    }

    public string Get(string key)
    {
        _sets.TryGetValue(key, out var value);
        return value;
    }

    public T GetAs<T>(string key)
    {
        var json = Get(key);
        return JsonSerializer.Deserialize<T>(json) ?? default(T);
    }

    public List<KeyValuePair<string, string>> GetList()
    {
        return _sets.ToList();
    }

    public string GetName()
    {
        return _section;
    }

    private void Dump()
    {
        if (_persist)
        {
            if (!Directory.Exists("states")) Directory.CreateDirectory("states");
            var origFileName = $"{_section}_{_id}.json";
            var invalids = Path.GetInvalidFileNameChars();
            var newName = string.Join('_', origFileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries))
                .TrimEnd('.');
            File.WriteAllText($"states/{newName}", JsonSerializer.Serialize(_sets));
        }
    }
}