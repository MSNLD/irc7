using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Commands;

internal class Names : Command, ICommand
{
    public Names()
    {
        _requiredMinimumParameters = 1;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        var channelName = chatFrame.Message.Parameters.First();

        var channel = chatFrame.Server.GetChannelByName(channelName);

        if (channel != null)
            ProcessNamesReply(chatFrame.User, channel);
        else
            // TODO: To make common function for this
            chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, channelName));
    }

/*
 // TODO: MSNPROFILE
    (Code) - (MSNPROFILE) - (Description)
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

public static void ProcessNamesReply(User user, IChannel channel)
    {
        // TODO: IRC4 <H|G>,<A|S|G|U>,<G|R>,[.|@|+]<nickname>
        // TODO: 
        user.Send(Raw.IRCX_RPL_NAMEREPLY_353(user.Server, user, channel,
            string.Join(' ', channel.GetMembers().Select(m => m.GetUser()))));
    }
}