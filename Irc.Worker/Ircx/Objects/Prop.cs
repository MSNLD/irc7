using Irc.Extensions.Access;

namespace Irc.Worker.Ircx.Objects;

public class Prop
{
    public int Limit;
    public string Name;
    public int Permissions; //FFFF    F 	    F	    F   	F
    public string Value;

    public Prop(string Name, string Value, int Limit, UserAccessLevel Read, UserAccessLevel Write, bool ReadOnly,
        bool Hidden)
    {
        this.Name = Name;
        this.Value = Value;
        this.Limit = Limit;
        SetPermissions(Read, Write, ReadOnly, Hidden);
    }

    public bool ReadOnly => 0x000000FF == (0x000000FF & Permissions);
    public bool Hidden => 0x0000FF00 == (0x0000FF00 & Permissions);
    public UserAccessLevel ReadLevel => (UserAccessLevel) ((Permissions >> 24) & 0xFF);

    public UserAccessLevel WriteLevel => (UserAccessLevel) ((Permissions >> 16) & 0xFF);
    //        WriteLvl  ReadLvl	Hidden	ReadOnly 0-1
    //public UserAccessLevel ReadLevel;
    //public UserAccessLevel WriteLevel;

    public void SetPermissions(UserAccessLevel Read, UserAccessLevel Write, bool ReadOnly, bool Hidden)
    {
        Permissions = (((int) Write & 0xFF) << 16) + (((int) Read & 0xFF) << 24) + (Hidden ? 0xFF00 : 0) +
                      (ReadOnly ? 0xFF : 0);
    }
}