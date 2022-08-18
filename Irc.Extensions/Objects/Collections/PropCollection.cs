using Irc.Extensions.Props;
using Irc.IO;

namespace Irc.Objects.Collections;

public class PropCollection
{
    protected Dictionary<string, PropRule> properties = new Dictionary<string, PropRule>();

    public PropCollection()
    {

    }
}