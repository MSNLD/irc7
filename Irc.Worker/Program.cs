using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.Apollo.Security.Credentials;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;
using Irc.Worker.Ircx.Objects;
using Irc.Worker.Ircx.Runtime;

namespace Irc.Worker;

public class Program
{
    public static ServerSettings Config;
    public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

    public static List<Credentials> Credentials = new();
    public static SupportProvider Providers = new SupportProvider();

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

        Credentials =
            (List<Credentials>) Settings.LoadObject(BasePath + "credentials.json", typeof(List<Credentials>));
        if (Credentials == null) Credentials = new List<Credentials>();

        Providers.AddSupportProvider(new NTLM());
        Providers.AddSupportProvider(new GateKeeper());
        Providers.AddSupportProvider(new GateKeeperPassport(new Passport(Program.Config.PassportKey)));
        Providers.AddSupportProvider(new ANON());

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

        BaseServer = new Server(new Access(Config.ServerName, false), new PropCollection(), new List<Channel>());
        BaseServer.ServerFields.FullName = Config.ServerFullName;
        BaseServer.Name = Config.ServerName;

        var Channels =
            (List<ChannelSettings>) Settings.LoadObject(BasePath + "channels.json",
                typeof(List<ChannelSettings>)); //new List<ChannelSettings>();
        if (Channels == null) Channels = new List<ChannelSettings>();
        for (var c = 0; c < Channels.Count; c++)
        {
            var channel = BaseServer.AddChannel(Channels[c].Name);
            if (Channels[c].Topic != null)
                channel.Properties.Set("Topic", Channels[c].Topic);

            channel.TopicLastChanged = Channels[c].TopicLastChanged;
            channel.Properties.Set("Ownerkey", Channels[c].Ownerkey);
            channel.Properties.Set("Hostkey", Channels[c].Hostkey);
            channel.Properties.Set("Subject", Channels[c].Subject);
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