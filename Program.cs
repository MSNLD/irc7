using System;
using System.Threading.Tasks;
using Core.Net;
using Core.Ircx.Objects;
using Core.Ircx;
using CSharpTools;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Core
{
    public class Program
    {
        public static ServerSettings Config;
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static List<Authentication.SSPCredentials> Credentials = new List<Authentication.SSPCredentials>();

        public static Server BaseServer;
        public static Engine engine;
        public static void Main(string[] args)
        {
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += Default_Unloading;

            Config = (ServerSettings)Settings.LoadObject(BasePath + "settings.json", typeof(ServerSettings));
            if (Config == null) {
                Config = new ServerSettings();
                Config.version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                Config.major = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Major;
                Config.minor = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Minor;
                Config.build = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Build;
            }
            ObjIDGenerator.ServerID = Config.ServerID;

            Credentials = (List<Authentication.SSPCredentials>)Settings.LoadObject(BasePath + "credentials.json", typeof(List<Authentication.SSPCredentials>));
            if (Credentials == null) { Credentials = new List<Authentication.SSPCredentials>(); }

            Core.Authentication.SSP.EnumerateSupportPackages();
            Core.Authentication.Package.GateKeeper gkp = new Authentication.Package.GateKeeper();

            for (int x = 0; x < args.Length; x++)
            {
                switch (args[x])
                {
                    case "-console":
                        {
                            Debug.EnableVerbose();
                            break;
                        }
                    case "-bindip":
                        {
                            if (args.Length > x + 1) // base 0 (plus 1 for base 1), plus 1
                            {
                                Config.BindIP = args[x + 1];
                            }
                            break;
                        }
                    case "-remoteip":
                        {
                            if (args.Length > x + 1) // base 0 (plus 1 for base 1), plus 1
                            {
                                Config.ExternalIP = args[x + 1];
                            }
                            break;
                        }
                    case "-debug":
                        {
                            Debug.Enable();
                            break;
                        }
                    case "-bindport":
                        {
                            if (args.Length > x + 1) // base 0 (plus 1 for base 1), plus 1
                            {
                                int.TryParse(args[x + 1], out Config.BindPort);
                            }

                            break;
                        }
                }
            }

            Debug.Out(String.Format("port: {0} buffSize: {1} backLog: {2} maxClients: {3} maxClientsPerIP: {4}", Config.BindPort, Config.BufferSize, Config.BackLog, Config.MaxClients, Config.MaxClientsPerIP));
            
            BaseServer = new Server(Program.Config.ServerName);

            List<ChannelSettings> Channels = (List<ChannelSettings>)Settings.LoadObject(BasePath + "channels.json", typeof(List<ChannelSettings>)); //new List<ChannelSettings>();
            if (Channels == null) { Channels = new List<ChannelSettings>(); }
            for (int c = 0; c < Channels.Count; c++)
            {
                Channel channel = BaseServer.AddChannel(Channels[c].Name);
                if (Channels[c].Topic != null) { channel.Properties.Topic.Value = Channels[c].Topic; }
                else { channel.Properties.Topic.Value = Resources.Null; }
                channel.Properties.TopicLastChanged = Channels[c].TopicLastChanged;
                channel.Properties.Ownerkey.Value = Channels[c].Ownerkey;
                channel.Properties.Hostkey.Value = Channels[c].Hostkey;
                channel.Properties.Subject.Value = Channels[c].Subject;
                channel.Modes.Registered.Value = Channels[c].Registered;
                channel.Modes.UserLimit.Value = Channels[c].UserLimit;
                channel.Modes.Subscriber.Value = Channels[c].Subscriber;
                channel.Modes.AuthOnly.Value = Channels[c].AuthOnly;
            }
            
            Core.Ircx.Runtime.Stats.ExportCategories(BaseServer);

            System.Net.IPAddress ip = null;
            if (Config.BindIP != null) { 
                System.Net.IPAddress.TryParse(Config.BindIP, out ip);
            }
            if (ip == null) { ip = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }); }

            Debug.Out(String.Format("Binding on {0}:{1}", ip.ToString(), Config.BindPort));

            engine = new Engine(BaseServer, ip, Config.BufferSize, Config.MaxClientsPerIP);
            engine.Start(ip, Config.BindPort, Config.BufferSize, Config.BackLog);
        }

        private static void Default_Unloading(System.Runtime.Loader.AssemblyLoadContext obj)
        {
            if (engine != null) { engine.Stop(); }
        }

    }
}
