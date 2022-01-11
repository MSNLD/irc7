using System.Text;

namespace Core.Ircx.Objects;
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

public enum ProfileUserMode
{
    Admin,
    Sysop,
    Guide,
    User
}

public enum ProfileUserType
{
    Guest,
    Registered,
    ProfileOnly,
    Male,
    Female
}

public class Profile
{
    private readonly StringBuilder _profileData = new(new string('\0', 7));
    public string AwayReason;
    private bool haspic, isreg, isaway;
    private int lastversion;
    private ProfileUserMode usermode;
    private ProfileUserType usertype;

    public Profile()
    {
        Ircvers = -1;

        Away = false;
        _profileData[1] = ',';
        UserMode = ProfileUserMode.User;
        _profileData[3] = ',';
        UserType = ProfileUserType.Guest;
        Picture = false;
        Registered = false;
    }

    private string ProfileData => _profileData.ToString();

    public int Ircvers { get; set; }

    public bool Away
    {
        get => isaway;
        set
        {
            isaway = value;
            _profileData[0] = !isaway ? 'H' : 'G';
        }
    }

    public bool Picture
    {
        get => haspic;
        set
        {
            haspic = value;
            _profileData[5] = haspic ? 'Y' : 'X';
        }
    }

    public bool Registered
    {
        get => isreg;
        set
        {
            isreg = value;
            _profileData[6] = isreg ? 'B' : 'O';
        }
    }

    public ProfileUserMode UserMode
    {
        get => usermode;
        set
        {
            usermode = value;
            var c = 'U';
            switch (usermode)
            {
                case ProfileUserMode.User:
                {
                    c = 'U';
                    break;
                }
                case ProfileUserMode.Guide:
                {
                    c = 'G';
                    break;
                }
                case ProfileUserMode.Sysop:
                {
                    c = 'S';
                    break;
                }
                case ProfileUserMode.Admin:
                {
                    c = 'A';
                    break;
                }
            }

            _profileData[2] = c;
        }
    }

    public ProfileUserType UserType
    {
        get => usertype;
        set
        {
            usertype = value;
            var c = 'R';
            switch (usertype)
            {
                case ProfileUserType.Guest:
                {
                    c = 'G';
                    break;
                }
                case ProfileUserType.Registered:
                {
                    c = 'R';
                    break;
                }
                case ProfileUserType.ProfileOnly:
                {
                    c = 'P';
                    break;
                }
                case ProfileUserType.Male:
                {
                    c = 'M';
                    break;
                }
                case ProfileUserType.Female:
                {
                    c = 'F';
                    break;
                }
            }

            _profileData[4] = c;
        }
    }

    public string GetProfile(int version)
    {
        if (version <= 3) return string.Empty;

        if (lastversion == version)
        {
            return ProfileData;
        }

        if (UserMode != ProfileUserMode.User)
        {
            //Admin handling
            _profileData[5] = 'O';
            if (version < 8)
                _profileData.Length = 5;
            else
                _profileData.Length = 6;
        }
        else
        {
            if (version <= 6)
            {
                _profileData[4] = UserType != ProfileUserType.Guest ? 'R' : 'G';
                _profileData.Length = 5;
            } //e.g. H,U,R
            else
            {
                if (UserType == ProfileUserType.Guest)
                {
                    _profileData[5] = 'O';
                    _profileData.Length = version == 7 ? 5 : 6; // 7 is H,U,G not H,U,GO
                }
                else
                {
                    UserType = usertype;
                    if (version < 8)
                    {
                        _profileData.Length = 6;
                    }
                    else
                    {
                        _profileData[6] = Registered ? 'B' : 'O';
                        _profileData.Length = 7;
                    }
                }
            } //resets to default
        }

        return _profileData.ToString();
    }
    //
}
////