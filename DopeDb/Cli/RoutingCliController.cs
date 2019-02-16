using DopeDb.Shared.Cli;
using DopeDb.Shared.Configuration;
using DopeDb.Shared.Plugins;
using DopeDb.Mvc.Routing;
using DopeDb.Shared.Mvc.Routing;

namespace DopeDb.Cli
{
    class RoutingCliController : AbstractCliController
    {
        protected RouteResolver routeResolver;

        public RoutingCliController()
        {
            var pluginManager = new PluginManager();
            var configurationManager = new ConfigurationManager(pluginManager);
            this.routeResolver = new RouteResolver(configurationManager);
        }

        public void ResolveCommand(string uri)
        {
            Util.WriteLine(routeResolver.ResolveRoute(uri));
        }

        public void ListCommand()
        {
            Util.WriteLine(routeResolver.GetAllRoutes());
        }
    }
}