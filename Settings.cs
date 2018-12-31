using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;

namespace Core
{
    public class ChannelSettings
    {
        public string Name;
        public string Topic;
        public long TopicLastChanged;
        public string Ownerkey;
        public string Hostkey;
        public string Subject;
        public int Registered;
        public int UserLimit;
        public int AuthOnly;
        public int Subscriber;

        public ChannelSettings(string Name) { this.Name = Name; }
    }

    public class ServerSettings
    {
        public string version = "1.0.0.0";
        public int major = 1;
        public int minor = 0;
        public int build = 0;
        public string ExternalIP = "127.0.0.1";
        public string BindIP = "127.0.0.1";
        public int BindPort = 6667;
        public int BufferSize = 512;
        public int BackLog = 1000;
        public int MaxClients = 1000;
        public int MaxClientsPerIP = 1000;

        public int PingTimeout = 180;
        public int MaxNickname = 64;
        public int MaxRealname = 64;
        public int MaxUsername = 31;
        public int ServerID = 0x1;
        public string ServerName = "IRC7";
        public string ServerFullName = "IRC7 Chat Network";
        public string PassportKey = "12345678abcdefhi";
        public string WebIRCUsername = "webirc";
        public string WebIRCPassword = "password";
        public string NTLMDomain = "DOMAIN"; // Used for NTLM Auth e.g. DOMAIN\USERNAME
        public string NTLMFQDN = "DOMAIN.COM";
        public string NTLMServerDomain = "SERVER.DOMAIN.COM";
        public string AdminLoc1 = "";
        public string AdminLoc2 = "";
        public string AdminEmail = "";
    }

    class Settings
    {
        public static void SaveObject(object O, string filename)
        {
            string sData = Newtonsoft.Json.JsonConvert.SerializeObject(O, Newtonsoft.Json.Formatting.Indented);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false);
            sw.Write(sData);
            sw.Flush();
            sw.Close();
        }
        public static object LoadObject(string filename, Type T)
        {
            System.IO.StreamReader sr;
            string sData;
            try { 
                sr = new System.IO.StreamReader(filename);
            }
            catch (Exception e) { return null; }

            sData = sr.ReadToEnd();
            sr.Close();

            try { 
                return Newtonsoft.Json.JsonConvert.DeserializeObject(sData, T);
            }
            catch (Exception e) { return null; }

        }
    }
}
