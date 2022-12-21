using Irc.Extensions.Apollo.Objects.Channel;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Modes;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Modes.User;

public class Host : ModeRule, IModeRule
{
    public Host() : base(ApolloResources.UserModeHost, true)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        // TODO: Write this better
        if (target == source && flag)
        {
            var user = (IUser)source;
            var channel = (ApolloChannel)user.GetChannels().LastOrDefault().Key;
            var member = user.GetChannels().LastOrDefault().Value;
            if (channel.PropCollection.GetProp(ExtendedResources.ChannelPropOwnerkey).GetValue() == parameter)
            {
                member.SetOwner(true);
                channel.Modes.GetMode('q').DispatchModeChange(source, channel, true, target.ToString());
            }
            else if (channel.PropCollection.GetProp(ExtendedResources.ChannelPropHostkey).GetValue() == parameter)
            {
                if (member.IsOwner())
                {
                    member.SetOwner(false);
                    channel.Modes.GetMode('q').DispatchModeChange(source, channel, false, target.ToString());
                }

                member.SetHost(true);
                channel.Modes.GetMode('o').DispatchModeChange(source, channel, true, target.ToString());
            }

            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_UNKNOWNMODEFLAG;
    }
}