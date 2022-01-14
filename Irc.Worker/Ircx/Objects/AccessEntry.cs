using System;
using Irc.Extensions.Access;

namespace Irc.Worker.Ircx.Objects;

public class AccessEntry
{
    public int Duration;
    public string EntryAddress;
    public UserAccessLevel EntryLevel;
    public bool Fixed;
    public AccessLevel Level;
    public Address Mask;
    public string Reason;

    public int DurationInSeconds => (int) Math.Ceiling((double) Duration / 60);
}