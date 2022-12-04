using System.Text.RegularExpressions;

namespace Irc.Objects;

public class Address
{
    public UserHostPair UserHost = new();

    public string Nickname { set; get; }

    public string User
    {
        set => UserHost.User = value;
        get => UserHost.User;
    }

    // TODO: NOTE: In Apollo, domain names are not supported in the host field; it must be a valid IP address.
    public string Host
    {
        set => UserHost.Host = value;
        get => UserHost.Host;
    }

    public string Server { set; get; }

    public string RealName { set; get; }
    public string RemoteIP { set; get; }

    public string GetUserHost()
    {
        return UserHost.ToString();
    }

    public string GetAddress()
    {
        return $"{Nickname}!{User}@{Host}";
    }

    public string GetFullAddress()
    {
        return $"{Nickname}!{User}@{Host}${Server}";
    }

    public bool IsAddressPopulated()
    {
        return !string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(Host) &&
               !string.IsNullOrWhiteSpace(Server) && RealName != null;
    }

    public bool Parse(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return false;

        // TODO: Check for bad characters

        var regex = new Regex(
            @"((?<nick>\w+)(?:\!)(?<user>\w+)(?:\@)(?<host>\w+)(?:\$)(?<server>\w*))|((?<nick>\w+)(?:\!)(?<user>\w+)(?:\@)(?<host>\w+))|((?<user>\w+)(?:\@)(?<host>\w+))|(?<nick>\w+)");
        var match = regex.Match(address);

        if (match.Groups.Count > 0)
        {
            if (match.Groups.ContainsKey("nick")) Nickname = match.Groups["nick"].Value;
            if (match.Groups.ContainsKey("user")) User = match.Groups["user"].Value;
            if (match.Groups.ContainsKey("host")) Host = match.Groups["host"].Value;
            if (match.Groups.ContainsKey("server")) Server = match.Groups["server"].Value;
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return GetAddress();
    }
    /* <nick> [ '!' <user> ] [ '@' <host> ]
       $       The  '$' prefix identifies a server on the network.
          The '$' character followed by a space or comma  may
          be used to represent the local server the client is
          connected to.
    */

    public record UserHostPair
    {
        public string User { get; set; }
        public string Host { get; set; }

        public override string ToString()
        {
            return $"{User}@{Host}";
        }
    }
}