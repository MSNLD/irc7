using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Objects.Channel;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Objects.Channel;

public class ApolloChannel : ExtendedChannel
{
    public ApolloChannel(string name, IModeCollection modeCollection, IDataStore dataStore) : base(name, modeCollection,
        dataStore)
    {
    }

    public override IChannel Join(IUser user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE)
    {
        var sourceMember = AddMember(user, accessResult);
        foreach (var member in GetMembers())
        {
            var targetUser = member.GetUser();
            if (targetUser.GetProtocol().GetProtocolType() <= EnumProtocolType.IRC3)
            {
                member.GetUser().Send(IrcRaws.RPL_JOIN(user, this));

                if (!sourceMember.IsNormal())
                {
                    var modeChar = sourceMember.IsOwner() ? 'q' : sourceMember.IsHost() ? 'o' : 'v';
                    Modes.GetMode(modeChar).DispatchModeChange((ChatObject)user, this, true, user.ToString());
                }
            }
            else
            {
                member.GetUser().Send(ApolloRaws.RPL_JOIN_MSN(member.GetUser().GetProtocol(), (ApolloUser)user, this));
            }
        }

        return this;
    }
}