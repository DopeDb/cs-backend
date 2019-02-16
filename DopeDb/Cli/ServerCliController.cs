using DopeDb.Shared.Cli;
using DopeDb.Shared.Plugins;
using DopeDb.Shared.Configuration;
using DopeDb.Http;
using DopeDb.Mvc.Routing;
using System.Net;

namespace DopeDb.Cli
{
    class ServerCliController : AbstractCliController
    {
        public void StartCommand(int port = 8080)
        {
            var pluginManager = new PluginManager();
            var configurationManager = new ConfigurationManager(pluginManager);
            var routeResolver = new RouteResolver(configurationManager);
            var requestHandler = new HttpRequestHandler(routeResolver);
            var server = new Server(port);
            server.SetRequestHandler(requestHandler.HandleRequest);
            server.Run();
            var running = true;
            while (running)
            {
                var input = System.Console.ReadLine();
                if (input == "quit")
                {
                    running = false;
                    break;
                }
            }
            server.Stop();
        }
    }
}