using System.Collections.Generic;
using System.Text.RegularExpressions;
using DopeDb.Shared.Mvc.Routing;
using DopeDb.Shared.Configuration;
using DopeDb.Shared.Caching.Frontend;
using DopeDb.Shared.Caching.Backend;
using Microsoft.Extensions.Configuration;

namespace DopeDb.Mvc.Routing
{
    class RouteResolver
    {
        protected List<Route> routes = new List<Route>();

        protected ConfigurationManager configurationManager;

        protected StringFrontend routeCache;

        public RouteResolver(ConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            this.routeCache = new StringFrontend("routes", new FileBackend("routes"));
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
            var controllerAction = configuration.GetSection("controllerAction").Value;
            if (controllerAction == null)
            {
                throw new System.ArgumentException("Missing controllerAction");
            }
            route.ControllerAction = controllerAction;
            var method = configuration.GetSection("method").Value;
            if (method == null)
            {
                route.Method = "*";
            }
            else
            {
                route.Method = method.ToUpper();
            }
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
            var cacheKey = $"[{method}] {uri}";
            Route result = null;
            if (this.routeCache.Has(cacheKey))
            {
                result = this.routes.Find(r => r.Identifier == this.routeCache.Get(cacheKey));
                if (result != null)
                {
                    routeCache.Set(cacheKey, result.Identifier);
                    return result;
                }
            }
            foreach (var route in this.routes)
            {
                if (route.Method != "*" && route.Method != method)
                {
                    continue;
                }
                var variables = Regex.Matches(route.UriPattern, "{([a-zA-Z_]+)}");
                var variableNames = new List<string>();
                var pattern = route.UriPattern;
                foreach (Match variable in variables)
                {
                    var variableName = variable.Groups[1].Value;
                    variableNames.Add(variableName);
                    pattern = pattern.Replace(variable.Groups[0].Value, @"([^/]+)");
                }
                var routePartMatches = Regex.Match(uri, pattern);
                if (!routePartMatches.Success) {
                    continue;
                }
                var queryArguments = new Dictionary<string, string>();
                for (int i = 1; i < routePartMatches.Groups.Count; i++) {
                    queryArguments.Add(variableNames[i - 1], routePartMatches.Groups[i].Value);
                }
                // TODO pass arguments
                routeCache.Set(cacheKey, route.Identifier);
                return route;
            }
            throw new RoutingException("Could not resolve a route for the given uri");
        }
    }
}