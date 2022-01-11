using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using Core.Authentication;
using Core.Authentication.Package;
using Core.Ircx.Objects;
using Core.Ircx.Runtime;
using CSharpTools;

namespace Core;

public class Program
{
    public static ServerSettings Config;
    public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

    public static List<SSPCredentials> Credentials = new();

    public static Server BaseServer;
    public static Engine engine;

    public static void Main(string[] args)
    {
        AssemblyLoadContext.Default.Unloading += Default_Unloading;

        Config = (ServerSettings) Settings.LoadObject(BasePath + "settings.json", typeof(ServerSettings));
        if (Config == null)
        {
            Config = new ServerSettings();
            Config.version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            Config.major = Assembly.GetEntryAssembly().GetName().Version.Major;
            Config.minor = Assembly.GetEntryAssembly().GetName().Version.Minor;
            Config.build = Assembly.GetEntryAssembly().GetName().Version.Build;
        }

        ObjIDGenerator.ServerID = Config.ServerID;

        Credentials =
            (List<SSPCredentials>) Settings.LoadObject(BasePath + "credentials.json", typeof(List<SSPCredentials>));
        if (Credentials == null) Credentials = new List<SSPCredentials>();

        SSP.EnumerateSupportPackages();
        var gkp = new GateKeeper();

        for (var x = 0; x < args.Length; x++)
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
                        Config.BindIP = args[x + 1];
                    break;
                }
                case "-remoteip":
                {
                    if (args.Length > x + 1) // base 0 (plus 1 for base 1), plus 1
                        Config.ExternalIP = args[x + 1];
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
                        int.TryParse(args[x + 1], out Config.BindPort);

                    break;
                }
            }

        Debug.Out(string.Format("port: {0} buffSize: {1} backLog: {2} maxClients: {3} maxClientsPerIP: {4}",
            Config.BindPort, Config.BufferSize, Config.BackLog, Config.MaxClients, Config.MaxClientsPerIP));

        BaseServer = new Server(Config.ServerName);

        var Channels =
            (List<ChannelSettings>) Settings.LoadObject(BasePath + "channels.json",
                typeof(List<ChannelSettings>)); //new List<ChannelSettings>();
        if (Channels == null) Channels = new List<ChannelSettings>();
        for (var c = 0; c < Channels.Count; c++)
        {
            var channel = BaseServer.AddChannel(Channels[c].Name);
            if (Channels[c].Topic != null)
                channel.Properties.Topic.Value = Channels[c].Topic;
            else
                channel.Properties.Topic.Value = Resources.Null;
            channel.Properties.TopicLastChanged = Channels[c].TopicLastChanged;
            channel.Properties.Ownerkey.Value = Channels[c].Ownerkey;
            channel.Properties.Hostkey.Value = Channels[c].Hostkey;
            channel.Properties.Subject.Value = Channels[c].Subject;
            channel.Modes.Registered.Value = Channels[c].Registered;
            channel.Modes.UserLimit.Value = Channels[c].UserLimit;
            channel.Modes.Subscriber.Value = Channels[c].Subscriber;
            channel.Modes.AuthOnly.Value = Channels[c].AuthOnly;
        }

        Stats.ExportCategories(BaseServer);

        IPAddress ip = null;
        if (Config.BindIP != null) IPAddress.TryParse(Config.BindIP, out ip);
        if (ip == null) ip = new IPAddress(new byte[] {127, 0, 0, 1});

        Debug.Out(string.Format("Binding on {0}:{1}", ip, Config.BindPort));

        engine = new Engine(BaseServer, ip, Config.BufferSize, Config.MaxClientsPerIP);
        engine.Start(ip, Config.BindPort, Config.BufferSize, Config.BackLog);
    }

    private static void Default_Unloading(AssemblyLoadContext obj)
    {
        if (engine != null) engine.Stop();
    }
}