using System;
using System.IO;
using Newtonsoft.Json;

namespace Irc.Worker;

public class ChannelSettings
{
    public int AuthOnly;
    public string Hostkey;
    public string Name;
    public string Ownerkey;
    public int Registered;
    public string Subject;
    public int Subscriber;
    public string Topic;
    public long TopicLastChanged;
    public int UserLimit;

    public ChannelSettings(string Name)
    {
        this.Name = Name;
    }
}

public class ServerSettings
{
    public string AdminEmail = "";
    public string AdminLoc1 = "";
    public string AdminLoc2 = "";
    public int BackLog = 1000;
    public string BindIP = "127.0.0.1";
    public int BindPort = 6667;
    public int BufferSize = 512;
    public int build = 0;
    public string ExternalIP = "127.0.0.1";
    public int major = 1;
    public int MaxClients = 1000;
    public int MaxClientsPerIP = 1000;
    public int MaxNickname = 64;
    public int MaxRealname = 64;
    public int MaxUsername = 31;
    public int minor = 0;
    public string NTLMDomain = "Domain"; // Used for NTLM Auth e.g. Domain\USERNAME
    public string NTLMFQDN = "Domain.COM";
    public string NTLMServerDomain = "SERVER.Domain.COM";
    public string PassportKey = "12345678abcdefhi";

    public int PingTimeout = 180;
    public string ServerFullName = "IRC7 Chat Network";
    public int ServerID = 0x1;
    public string ServerName = "IRC7";
    public string version = "1.0.0.0";
    public string WebIRCPassword = "password";
    public string WebIRCUsername = "webirc";
}

internal class Settings
{
    public static void SaveObject(object O, string filename)
    {
        var sData = JsonConvert.SerializeObject(O, Formatting.Indented);
        var sw = new StreamWriter(filename, false);
        sw.Write(sData);
        sw.Flush();
        sw.Close();
    }

    public static object LoadObject(string filename, Type T)
    {
        StreamReader sr;
        string sData;
        try
        {
            sr = new StreamReader(filename);
        }
        catch (Exception e)
        {
            return null;
        }

        sData = sr.ReadToEnd();
        sr.Close();

        try
        {
            return JsonConvert.DeserializeObject(sData, T);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}