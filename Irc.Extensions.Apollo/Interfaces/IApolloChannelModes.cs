using Irc.Extensions.Interfaces;

namespace Irc.Extensions.Apollo.Interfaces;

public interface IApolloChannelModes : IExtendedChannelModes
{
    bool NoGuestWhisper { get; set; }
    bool OnStage { get; set; }
    bool Subscriber { get; set; }
}