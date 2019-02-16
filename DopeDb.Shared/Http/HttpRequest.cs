using System.Net;
using System.Collections.Specialized;
using System.Web;

namespace DopeDb.Shared.Http
{
    public class HttpRequest
    {
        protected HttpListenerRequest originalRequest;

        protected NameValueCollection queryArguments;

        public HttpRequest(HttpListenerRequest request)
        {
            this.originalRequest = request;
            this.queryArguments = request.QueryString;
        }

        public bool HasArgument(string argumentName)
        {
            var hasQueryArgument = this.queryArguments[argumentName] != null;
            return hasQueryArgument;
        }

        public object GetArgument(string argumentName)
        {
            if (!HasArgument(argumentName))
            {
                throw new System.ArgumentException($"No such argument {argumentName}");
            }
            return this.queryArguments[argumentName];
        }
    }
}