using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Objects.Channel;
using Irc.Interfaces;
using Irc.IO;
using Irc.Modes;
using Irc.Objects;
using Irc.Objects.Member;

namespace Irc.Extensions.Apollo.Objects.Channel;

public class ApolloChannel : ExtendedChannel
{
    public ApolloChannel(string name, IChannelModeCollection modeCollection, IDataStore dataStore) : base(name, modeCollection,
        dataStore)
    {
    }

    public override IChannel Join(IUser user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE)
    {
        var joinMember = AddMember(user, accessResult);
        foreach (var channelMember in GetMembers())
        {
            var channelUser = channelMember.GetUser();
            if (channelUser.GetProtocol().GetProtocolType() <= Enumerations.EnumProtocolType.IRC3) {
                channelMember.GetUser().Send(IrcRaws.RPL_JOIN(user, this));

                if (!joinMember.IsNormal()) {
                    char modeChar = joinMember.IsOwner() ? 'q' : (joinMember.IsHost() ? 'o' : 'v');
                    ((ModeRule)Modes.GetMode(modeChar)).DispatchModeChange((ChatObject)channelUser, modeChar, (ChatObject)user, (ChatObject)this, true, user.ToString());
                }
            }
            else {
                channelUser.Send(ApolloRaws.RPL_JOIN_MSN(channelMember, this, joinMember));
            }
        }
        
        return this;
    }
}