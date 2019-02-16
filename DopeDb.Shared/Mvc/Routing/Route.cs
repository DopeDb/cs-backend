using System.Collections.Generic;

namespace DopeDb.Shared.Mvc.Routing
{
    public class Route
    {
        public string Identifier;

        public string UriPattern;

        public string Position;

        public string ControllerName;

        public string ControllerAction;

        public Dictionary<string, object> ControllerOptions = new Dictionary<string, object>();

        protected Dictionary<string, string> nodePartMappers = new Dictionary<string, string>();

        public string GetNodePartMapper(string nodePartName)
        {
            return nodePartMappers.ContainsKey(nodePartName) ? nodePartMappers[nodePartName] : null;
        }

        public void SetNodePartMapper(string nodePartName, string mapperName)
        {
            nodePartMappers.Add(nodePartName, mapperName);
        }
    }
}