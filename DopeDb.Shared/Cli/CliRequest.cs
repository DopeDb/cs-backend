using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DopeDb.Shared.Cli {
    public class CliRequest {
        public string ControllerName;

        public string ActionName;

        public Dictionary<string, object> Arguments;

        public List<string> AdditionalArguments;

        public object GetArgument(string name)
        {
            return Arguments.ContainsKey(name) ? Arguments[name] : null;
        }
    }
}