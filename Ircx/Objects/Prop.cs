using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;

namespace Core.Ircx.Objects
{
    public class Prop
    {
        public String8 Name;
        public String8 Value;
        public int Limit;
        public int Permissions; //FFFF    F 	    F	    F   	F
                                //        WriteLvl  ReadLvl	Hidden	ReadOnly 0-1
                                //public UserAccessLevel ReadLevel;
                                //public UserAccessLevel WriteLevel;

        public void SetPermissions(UserAccessLevel Read, UserAccessLevel Write, bool ReadOnly, bool Hidden)
        {
            Permissions = (((int)Write & 0xFF) << 16) + (((int)Read & 0xFF) << 24) + (Hidden ? 0xFF00 : 0) + (ReadOnly ? 0xFF : 0);
        }

        public Prop(String8 Name, String8 Value, int Limit, UserAccessLevel Read, UserAccessLevel Write, bool ReadOnly, bool Hidden)
        {
            this.Name = Name;
            this.Value = Value;
            this.Limit = Limit;
            SetPermissions(Read, Write, ReadOnly, Hidden);
        }
        public bool ReadOnly { get { return (0x000000FF == (0x000000FF & Permissions)); } }
        public bool Hidden { get { return (0x0000FF00 == (0x0000FF00 & Permissions)); } }
        public UserAccessLevel ReadLevel { get { return (UserAccessLevel)((Permissions >> 24) & 0xFF); } }
        public UserAccessLevel WriteLevel { get { return (UserAccessLevel)((Permissions >> 16) & 0xFF); } }

    };

    public class PropCollection {
        protected List<Prop> Properties = new List<Prop>();
        public Prop Name = new Prop("NAME", Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None, true, false);

        public List<Prop> List { get { return Properties; } }
        public void Add(Prop prop) { List.Add(prop); }

        public PropCollection(Obj obj)
        {
            Properties.Add(new Prop("OID", obj.OIDX8, 0, UserAccessLevel.None, UserAccessLevel.None, true, false));
            Properties.Add(Name);
            //obj.Name = name.Value;
        }
        public Prop GetPropByName(String8 PropName)
        {
            for (int c = 0; c < Properties.Count; c++)
            {
                if (Properties[c].Name == PropName)
                {
                    return Properties[c];
                }
            }
            return null;
        }
    }

}
