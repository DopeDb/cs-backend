using System.Collections.Generic;
using DopeDb.Shared.Mvc.Routing;
using DopeDb.Shared.Configuration;
using Microsoft.Extensions.Configuration;

namespace DopeDb.Mvc.Routing
{
    class RouteResolver
    {
        protected List<Route> routes = new List<Route>();

        protected ConfigurationManager configurationManager;

        public RouteResolver(ConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            this.LoadRoutes();
        }

        protected void LoadRoutes()
        {
            var routeConfiguration = configurationManager.GetConfiguration(ConfigurationType.Routes);
            foreach (var routeData in routeConfiguration.GetChildren())
            {
                try
                {
                    var route = CreateRouteFromConfiguration(routeData);
                    routes.Add(route);
                }
                catch (System.ArgumentException e)
                {
                    Shared.Cli.Util.Write(e);
                }
            }
            this.SortRoutes();
        }

        protected Route CreateRouteFromConfiguration(IConfigurationSection configuration)
        {
            var route = new Route();
            route.Identifier = configuration.Key;
            var uriPattern = configuration.GetSection("uriPattern").Value;
            if (uriPattern == null)
            {
                throw new System.ArgumentException("Missing uriPattern");
            }
            route.UriPattern = uriPattern;
            var controllerName = configuration.GetSection("controllerName").Value;
            if (controllerName == null)
            {
                throw new System.ArgumentException("Missing controllerName");
            }
            route.ControllerName = controllerName;
            route.UriPattern = uriPattern;
            var controllerAction = configuration.GetSection("controllerAction").Value;
            if (controllerAction == null)
            {
                throw new System.ArgumentException("Missing controllerAction");
            }
            route.ControllerAction = controllerAction;
            // TODO map controllerOptions
            return route;
        }

        protected void SortRoutes()
        {
            this.routes.Sort((x, y) => y.UriPattern.CompareTo(x.UriPattern));
        }

        public Route[] GetAllRoutes()
        {
            return this.routes.ToArray();
        }

        public Route ResolveRoute(string uri, string method = "GET")
        {
            throw new RoutingException("Could not resolve a route for the given uri");
        }
    }
}