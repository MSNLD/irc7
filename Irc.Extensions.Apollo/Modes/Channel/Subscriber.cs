using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;

namespace Irc.Extensions.Apollo.Modes.Channel;

public class Subscriber : ModeRuleChannel, IModeRule
{
    public Subscriber() : base(ApolloResources.ChannelModeSubscriber)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}