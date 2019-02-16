using System;
using System.Net;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using DopeDb.Mvc.Routing;
using DopeDb.Shared.Mvc.Routing;
using DopeDb.Shared.Http;

namespace DopeDb.Http
{
    class HttpRequestHandler
    {
        protected RouteResolver routeResolver;

        public HttpRequestHandler(RouteResolver routeResolver)
        {
            this.routeResolver = routeResolver;
        }

        public HttpListenerResponse HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var route = routeResolver.ResolveRoute(request.Url.ToString(), request.HttpMethod);
            }
            catch (RoutingException e)
            {
                response.StatusCode = 500;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(e.Message);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
            return response;
        }

        protected void dispatchRequest(Route route, HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var controllerType = Type.GetType(route.ControllerName);
                var actionMethodInfo = controllerType.GetMethod(route.ControllerAction + "Action", BindingFlags.Public);
                if (actionMethodInfo == null)
                {
                    throw new RoutingException($"The controller {route.ControllerName} does not have the action {route.ControllerAction}Action.");
                }
                var httpRequest = new HttpRequest(request);
                var mappedArguments = mapArguments(actionMethodInfo, httpRequest);
                var controller = Activator.CreateInstance(controllerType);
                actionMethodInfo.Invoke(controller, mappedArguments);
            }
            catch (TypeLoadException e)
            {
                throw new RoutingException($"Could not resolve controller {route.ControllerName}", e);
            }
        }

        protected object[] mapArguments(MethodInfo action, HttpRequest request)
        {
            var result = new List<object>();
            foreach (var parameter in action.GetParameters())
            {
                var targetName = parameter.Name;
                object targetValue = null;
                if (request.HasArgument(targetName))
                {
                    var typeConverter = TypeDescriptor.GetConverter(parameter.ParameterType);
                    targetValue = typeConverter.ConvertTo(request.GetArgument(targetName), parameter.ParameterType);
                }
                else if (parameter.IsOptional && parameter.HasDefaultValue)
                {
                    targetValue = parameter.DefaultValue;
                }
                else
                {
                    throw new System.ArgumentException($"Missing parameter {targetName}.");
                }
                result.Add(targetValue);
            }
            return result.ToArray();
        }
    }
}