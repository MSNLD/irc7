namespace Irc.Worker.Ircx.Objects;

public class UserCollection : ObjCollection
{
    public UserCollection() : base(ObjType.UserObject)
    {
    }

    public User this[int c] => (User) IndexOf(c);

    public User GetUser(string TargetUser)
    {
        var objIdentifier = Obj.IdentifyObject(TargetUser);
        return (User) FindObj(TargetUser, objIdentifier);
    }
}