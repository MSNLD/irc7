using Irc.Constants;
using Irc.Extensions.Interfaces;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using System.Text.RegularExpressions;

namespace Irc.Extensions.Objects.Channel;

public class ExtendedChannel : global::Irc.Objects.Channel.Channel, IExtendedChatObject
{
    private ChannelPropCollection _properties = new();
    public IPropCollection PropCollection => _properties;

    public ExtendedChannel(string name, IModeCollection modeCollection, IDataStore dataStore) : base(name,
        modeCollection, dataStore)
    {
        _properties.SetProp("NAME", name);
    }

    protected override IChannelMember AddMember(IUser user)
    {
        var member = new Member.ExtendedMember(user);
        //member.SetHost(true);
        //member.SetOwner(true);
        _members.Add(member);
        user.AddChannel(this, member);
        return member;
    }

    public static new bool ValidName(string channel)
    {
        var regex = new Regex(Resources.IrcChannelRegex);
        return regex.Match(channel).Success;
    }
}