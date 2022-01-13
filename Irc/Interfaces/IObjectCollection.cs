using System.Collections.Generic;
using Irc.Objects;

namespace Irc.Worker.Ircx.Objects;

public interface IObjectCollection<T> where T : ChatObject
{
    List<T> ChatObjects { get; }
    int Length { get; }
    void Add(T chatObject);
    void Remove(T chatObject);
    void Clear();
    T IndexOf(int i);
    T FindObj(string Name, IrcHelper.ObjIdentifier objectType);
    T FindObjByOID(string objectId);
    T FindObjByHex(string Hex);
    T FindObjByName(string Name);
}