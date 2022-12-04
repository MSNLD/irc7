namespace Irc.Extensions.Interfaces;

public interface IPropCollection
{
    IPropRule GetProp(string name);
    List<IPropRule> GetProps();
}