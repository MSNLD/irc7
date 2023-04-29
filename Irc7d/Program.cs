// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text.Json;
using Irc.Extensions.Apollo.Directory;
using Irc.Extensions.Apollo.Factories;
using Irc.Extensions.Apollo.Objects.Channel;
using Irc.Extensions.Apollo.Objects.Server;
using Irc.Extensions.Factories;
using Irc.Extensions.Objects.Channel;
using Irc.Extensions.Objects.Server;
using Irc.Extensions.Security.Packages;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects.Collections;
using Irc.Objects.Server;
using Irc.Security;
using Microsoft.Extensions.CommandLineUtils;

/*
 *  Server
 *      .Channels
 *          .Where(c in channels)
 *          .Where(c => c.Allows(user))
 *          .Join(user).SendTopic(user).SendNames(user)
 *
 */

namespace Irc7d;

internal class Program
{
    private enum IrcType { IRC, IRCX, MSN, DIR };
    private static void Main(string[] args)
    {
        var app = new CommandLineApplication();
        app.Name = AppDomain.CurrentDomain.FriendlyName;
        app.Description = "irc7 daemon";
        app.HelpOption("-h|--help");

        var configOption = app.Option("-c|--config <configfile>",
            "The path to the configuration json file (default ./config.json)", CommandOptionType.SingleValue);
        var bindIpOption = app.Option("-i|--ip <bindip>", "The ip for the server to bind on (default 0.0.0.0)",
            CommandOptionType.SingleValue);
        var bindOption = app.Option("-p|--port <bindport>", "The port for the server to bind on (default 6667)",
            CommandOptionType.SingleValue);
        var backlogOption = app.Option("-k|--backlog <backlogsize>",
            "The backlog for connecting sockets (default 1024)", CommandOptionType.SingleValue);
        var bufferSizeOption = app.Option("-z|--buffer <buffersize>", "The incoming buffer size in bytes (default 512)",
            CommandOptionType.SingleValue);
        var maxConnectionsPerIpOption = app.Option("-m|--maxconn <maxconnections>",
            "The maximum connections per IP that can connect (default 128)", CommandOptionType.SingleValue);
        var fqdnOption = app.Option("-f|--fqdn <fqdn>", "The FQDN of the machine (default localhost)",
            CommandOptionType.SingleValue);
        var serverType = app.Option("-t|--type <type>",
            "Type of server e.g. IRC, IRCX, MSN, DIR", CommandOptionType.SingleValue);
        var chatServerIP = app.Option("-s|--server", "The Chat Server IP (temporary, DIR mode only)", CommandOptionType.SingleValue);

        app.OnExecute(() =>
        {
            if (app.OptionHelp.HasValue())
            {
                app.ShowHint();
            }
            else
            {
                var ip = IPAddress.Any;
                if (bindIpOption.HasValue()) ip = IPAddress.Parse(bindIpOption.Value());

                var port = 6667;
                if (bindOption.HasValue()) port = Convert.ToInt32(bindOption.Value());

                var backlog = 1024;
                if (backlogOption.HasValue()) backlog = Convert.ToInt32(backlogOption.Value());

                var bufferSize = 512;
                if (bufferSizeOption.HasValue()) bufferSize = Convert.ToInt32(bufferSizeOption.Value());

                var maxConnections = 128;
                if (maxConnectionsPerIpOption.HasValue())
                    maxConnections = Convert.ToInt32(maxConnectionsPerIpOption.Value());

                var fqdn = "localhost";
                if (fqdnOption.HasValue()) fqdn = fqdnOption.Value();

                var forwardServer = "";
                if (chatServerIP.HasValue()) forwardServer = chatServerIP.Value();

                Enum.TryParse<IrcType>(serverType.Value(), true, out var type);

                var socketServer = new SocketServer(ip, port, backlog, maxConnections, bufferSize);
                socketServer.OnListen += (sender, server1) =>
                {
                    Console.WriteLine(
                        $"Listening on {ip}:{port} backlog={backlog} buffer={bufferSize} maxconn={maxConnections} fqdn={fqdn} type={type} {(chatServerIP.HasValue() ? "forwardserver=" : "")}{forwardServer}");
                };

                var securityManager = new SecurityManager();
                securityManager.AddSupportPackage(new NTLM(new NTLMCredentials()));

                IServer server = null;

                switch (type)
                {
                    case IrcType.IRC:
                        {
                            server = new Server(socketServer, securityManager, new FloodProtectionManager(),
                            new DataStore("DefaultServer.json"),
                            new List<IChannel>(), null, new UserFactory());
                            break;
                        }
                    case IrcType.IRCX:
                        {
                            server = new ExtendedServer(socketServer, securityManager, new FloodProtectionManager(),
                            new DataStore("DefaultServer.json"),
                            new List<IChannel>(), null, new ExtendedUserFactory());
                            break;
                        }
                    case IrcType.DIR:
                        {
                            server = new DirectoryServer(socketServer, securityManager, new FloodProtectionManager(),
                            new DataStore("DefaultServer.json"),
                            new List<IChannel>(), null, new ApolloUserFactory());

                            string motd = "\r\n** Welcome to the MSN.com Chat Service Network **\r\n";

                            server.SetMOTD(motd);
                            ((DirectoryServer)server).ChatServerIP = forwardServer;

                            break;
                        }
                    default:
                        {
                            server = new ApolloServer(socketServer, securityManager, new FloodProtectionManager(),
                            new DataStore("DefaultServer.json"),
                            new List<IChannel>(), null, new ApolloUserFactory());
                            break;
                        }
                }

                server.RemoteIP = fqdn;

                var defaultChannels = JsonSerializer.Deserialize<List<DefaultChannel>>(File.ReadAllText("DefaultChannels.json"));
                foreach (var defaultChannel in defaultChannels)
                {
                    var name = type == IrcType.IRC ? $"#{defaultChannel.Name}" : $"%#{defaultChannel.Name}";
                    var channel = server.CreateChannel(name);
                    channel.ChannelStore.Set("topic", defaultChannel.Topic);
                    foreach (KeyValuePair<char, int> keyValuePair in defaultChannel.Modes) {
                        channel.GetModes().SetModeChar(keyValuePair.Key, keyValuePair.Value);
                    }

                    if (channel is ExtendedChannel || channel is ApolloChannel)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in defaultChannel.Props)
                        {
                            ((ExtendedChannel)channel).PropCollection.GetProp(keyValuePair.Key).SetValue(keyValuePair.Value);
                        }
                    }

                    server.AddChannel(channel);
                }

                Console.ReadLine();
                server.Shutdown();
            }

            return 0;
        });

        app.Execute(args);
    }
}