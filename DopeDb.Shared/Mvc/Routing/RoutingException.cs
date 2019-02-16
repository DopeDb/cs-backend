using System;
using System.Collections.Generic;

namespace DopeDb.Shared.Mvc.Routing
{
    public class RoutingException : System.Exception
    {
        public RoutingException()
        {
        }

        public RoutingException(string message) : base(message)
        {
        }

        public RoutingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}