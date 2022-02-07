// See https://aka.ms/new-console-template for more information

using System.Net;
using Irc.Extensions.Apollo.Objects.Server;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects.Collections;
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

                var socketServer = new SocketServer(ip, port, backlog, maxConnections, bufferSize);
                socketServer.OnListen += (sender, server1) =>
                {
                    Console.WriteLine(
                        $"Listening on {ip}:{port} backlog={backlog} buffer={bufferSize} maxconn={maxConnections} fqdn={fqdn}");
                };

                var securityManager = new SecurityManager();

                var server = new ApolloServer(socketServer, securityManager, new FloodProtectionManager(),
                    new PropCollection("TK2CHATCHATA01"),
                    new List<IChannel>(), null);
                server.RemoteIP = fqdn;


                var theLobby = server.CreateChannel("%#The\\bLobby");
                theLobby.ChannelStore.Set("topic", "Welcome to The Lobby");
                theLobby.GetModes().SetModeChar('n', 1);
                theLobby.GetModes().SetModeChar('t', 1);
                server.AddChannel(theLobby);

                var test = server.CreateChannel("%#Test");
                test.ChannelStore.Set("topic", "Test");
                test.GetModes().SetModeChar('n', 1);
                test.GetModes().SetModeChar('t', 1);
                test.GetModes().SetModeChar('S', 1);
                server.AddChannel(test);

                var channels = new List<IChannel>();
                channels.Add(theLobby);
                channels.Add(test);

                Console.ReadLine();
                server.Shutdown();
            }

            return 0;
        });

        app.Execute(args);
    }
}