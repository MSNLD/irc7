using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Irc.Objects;

namespace Irc.Extensions.Objects.Channel
{
    public class ExtendedChannelModes: ChannelModes
    {
        public ExtendedChannelModes(): base()
        {
            modes.Add(ExtendedResources.ChannelModeAuthOnly, 0);
            modes.Add(ExtendedResources.ChannelModeProfanity, 0);
            modes.Add(ExtendedResources.ChannelModeHidden, 0);
            modes.Add(ExtendedResources.ChannelModeRegistered, 0);
            modes.Add(ExtendedResources.ChannelModeKnock, 0);
            modes.Add(ExtendedResources.ChannelModeNoWhisper, 0);
            modes.Add(ExtendedResources.ChannelModeAuditorium, 0);
            modes.Add(ExtendedResources.ChannelModeCloneable, 0);
            modes.Add(ExtendedResources.ChannelModeClone, 0);
            modes.Add(ExtendedResources.ChannelModeService, 0);
        }
    }
}
