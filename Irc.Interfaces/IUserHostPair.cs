namespace Irc.Interfaces;

public interface IUserHostPair
{
    string User { get; set; }
    string Host { get; set; }
    string ToString();
}