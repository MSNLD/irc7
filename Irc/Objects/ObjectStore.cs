using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Worker.Ircx.Objects
{
    public class ObjectStore : IObjectStore
    {
        Dictionary<int, Dictionary<string, object>> _store = new ();

        public T Get<T>(string name)
        {
            if (_store.TryGetValue(typeof(T).GetHashCode(), out var _objectDict))
            {
                if (_objectDict.TryGetValue(name, out var objectValue))
                {
                    return (T)objectValue;
                }
            }

            return default(T);
        }

        public void Set<T>(string name, T objectValue)
        {
            if (!_store.TryGetValue(typeof(T).GetHashCode(), out var _objectDict))
                _objectDict = new Dictionary<string, object>();

            _objectDict[name] = objectValue;
            _store[objectValue.GetType().GetHashCode()] = _objectDict;
        }
    }
}
