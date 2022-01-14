namespace Irc.Worker.Ircx.Objects;

public class AccessObjectResult
{
    public AccessEntry Entry;
    public AccessResultEnum Result;

    public AccessObjectResult()
    {
        Result = AccessResultEnum.NONE;
    }
}