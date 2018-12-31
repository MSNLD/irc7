using System;
using CSharpTools;

namespace Core.Ircx.Objects
{
    /*(Code) - (MSNPROFILE) - (Description)
    FY - 13 - Female, has picture in profile.
    MY - 11 - Male, has picture in profile.
    PY - 9 - Gender not specified, has picture in profile.
    FX - 5 - Female, no picture in profile.
    MX - 3 - Male, no picture in profile.
    PX - 1 - Gender not specified, and no picture, but has a profile.
    RX - 0 - No profile at all.
    G - 0 - Guest user (Guests can't have profiles).*/

    /*where:
    <H|G> is the away status using the same notation as the WHO message  (H=here, G=gone) (IRC4+)
    <A|S|G|U> is the user mode (Admin, Sysop, Guide, User) (IRC4+)
    <G|R|P|M|F> is the user type (G = Guest IRC4+, R = Registered or Passport user IRC4+, P = No Gender IRC7, M = Male IRC7, F = Female IRC7)
    <X|Y> (X = No Pic, Y = Has Pic) (IRC7)
    <B|O> (B = Registered, O = Unregistered) (IRC8)
    [.|@|+] : This is optional depending on the user. Owners get a [.] prefix, Hosts 
    has a [@] prefix, and [+] prefix means the user has voice.
    <nickname> = Nickname of up to 24 characters.
    
    
    Admin,Sysop,Guide status was Guest as no profile was present. Therefore:
    IRC8 H,A,GO is correct as well as 
         H,U,GO etc
         
    */

    public enum ProfileUserMode { Admin, Sysop, Guide, User };
    public enum ProfileUserType { Guest, Registered, ProfileOnly, Male, Female };

    public class Profile
    {
        int ircvers, lastversion;
        ProfileUserMode usermode;
        ProfileUserType usertype;
        bool haspic, isreg, isaway;
        String8 ProfileData = new String8(7);
        public String8 AwayReason;

        public Profile()
        {
            ircvers = -1;

            Away = false;
            ProfileData.bytes[1] = 44;
            UserMode = ProfileUserMode.User;
            ProfileData.bytes[3] = 44;
            UserType = ProfileUserType.Guest;
            Picture = false;
            Registered = false;
        }
        public int Ircvers
        {
            get { return ircvers; }
            set { ircvers = value; }
        }
        public bool Away
        {
            get { return isaway; }
            set
            {
                isaway = value;
                ProfileData.bytes[0] = (!isaway ? (byte)'H' : (byte)'G');
            }
        }
        public bool Picture
        {
            get { return haspic; }
            set
            {
                haspic = value;
                ProfileData.bytes[5] = haspic ? (byte)'Y' : (byte)'X';
            }
        }
        public bool Registered
        {
            get { return isreg; }
            set
            {
                isreg = value;
                ProfileData.bytes[6] = isreg ? (byte)'B' : (byte)'O';
            }
        }
        public ProfileUserMode UserMode
        {
            get { return usermode; }
            set
            {
                usermode = value;
                char c = 'U';
                switch (usermode)
                {
                    case ProfileUserMode.User: { c = 'U'; break; }
                    case ProfileUserMode.Guide: { c = 'G'; break; }
                    case ProfileUserMode.Sysop: { c = 'S'; break; }
                    case ProfileUserMode.Admin: { c = 'A'; break; }
                }
                ProfileData.bytes[2] = (byte)c;
            }
        }
        public ProfileUserType UserType
        {
            get { return usertype; }
            set
            {
                usertype = value;
                char c = 'R';
                switch (usertype)
                {
                    case ProfileUserType.Guest: { c = 'G'; break; }
                    case ProfileUserType.Registered: { c = 'R'; break; }
                    case ProfileUserType.ProfileOnly: { c = 'P'; break; }
                    case ProfileUserType.Male: { c = 'M'; break; }
                    case ProfileUserType.Female: { c = 'F'; break; }
                }
                ProfileData.bytes[4] = (byte)c;
            }
        }
        public String8 GetProfile(int version)
        {
            if (version <= 3)
            {
                return Resources.Null;
                //return null;
            }
            else
            {
                if (lastversion == version)
                {
                    return ProfileData;
                }
                else
                {
                    if (UserMode != ProfileUserMode.User)
                    {
                        //Admin handling
                        ProfileData.bytes[5] = (byte)'O';
                        if (version < 8) { ProfileData.length = 5; }
                        else { ProfileData.length = 6; }
                    }
                    else
                    {

                        if (version <= 6) { ProfileData.bytes[4] = (UserType != ProfileUserType.Guest ? (byte)'R' : (byte)'G'); ProfileData.length = 5; } //e.g. H,U,R
                        else
                        {
                            if (UserType == ProfileUserType.Guest)
                            {
                                ProfileData.bytes[5] = (byte)'O';
                                ProfileData.length = (version == 7 ? 5 : 6); // 7 is H,U,G not H,U,GO
                            }
                            else
                            {
                                UserType = usertype;
                                if (version < 8) { ProfileData.length = 6; }
                                else { ProfileData.bytes[6] = (Registered ? (byte)'B' : (byte)'O'); ProfileData.length = 7; }
                            }
                        } //resets to default
                    }
                    return ProfileData;
                }
            }
        }
        //
    }
    ////
}
