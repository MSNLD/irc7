using System.Text;
using System.Text.RegularExpressions;
using Irc.Constants;

namespace Irc.Worker.Ircx.Objects;

public class Address
{
    /* <nick> [ '!' <user> ] [ '@' <host> ]
       $       The  '$' prefix identifies a server on the network.
          The '$' character followed by a space or comma  may
          be used to represent the local server the client is
          connected to.
    */

    public string Nickname { set; get; }
    public string User { set; get; }
    public string Host { set; get; }
    public string Server { set; get; }

    public string GetUserHost() => $"{User}@{Host}";
    public string GetAddress() => $"{Nickname}!{User}@{Host}";
    public string GetFullAddress() => $"{Nickname}!{User}@{Host}${Server}";
    
    public bool Parse(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return false;

        // TODO: Check for bad characters

        Regex regex = new Regex(@"((?<nick>\w+)(?:\!)(?<user>\w+)(?:\@)(?<host>\w+)(?:\$)(?<server>\w*))|((?<nick>\w+)(?:\!)(?<user>\w+)(?:\@)(?<host>\w+))|((?<user>\w+)(?:\@)(?<host>\w+))|(?<nick>\w+)");
        Match match = regex.Match(address);

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
}