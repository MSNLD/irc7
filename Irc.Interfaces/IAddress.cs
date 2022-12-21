namespace Irc.Interfaces;

public interface IAddress
{
    string Nickname { set; get; }
    string User { set; get; }
    string Host { set; get; }
    string Server { set; get; }
    string RealName { set; get; }
    string RemoteIp { set; get; }
    string GetUserHost();
    string GetAddress();
    string GetFullAddress();
    bool IsAddressPopulated();
    bool Parse(string address);
    string ToString();
    IUserHostPair UserHost { get; set; }
}