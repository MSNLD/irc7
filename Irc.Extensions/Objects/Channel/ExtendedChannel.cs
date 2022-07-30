using Irc.Constants;
using Irc.IO;
using Irc.Objects;
using System.Text.RegularExpressions;

namespace Irc.Extensions.Objects.Channel;

public class ExtendedChannel : global::Irc.Objects.Channel.Channel
{
    public ExtendedChannel(string name, IModeCollection modeCollection, IDataStore dataStore) : base(name,
        modeCollection, dataStore)
    {
    }

    public static new bool ValidName(string channel)
    {
        var regex = new Regex(Resources.IrcChannelRegex);
        return regex.Match(channel).Success;
    }
}