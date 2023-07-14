using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;

namespace Irc.Extensions.Apollo.Modes.Channel;

public class NoGuestWhisper : ModeRuleChannel, IModeRule
{
    public NoGuestWhisper() : base(ApolloResources.ChannelModeNoGuestWhisper)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}