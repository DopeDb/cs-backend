using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DopeDb.Shared.Cli {
    public class CliCall {
        public string ControllerName;

        public string ActionName;

        public Dictionary<string, List<string>> NamedArguments;

        public List<string> PositionalArguments;

        public bool HasArgument(string argumentName)
        {
            return this.NamedArguments.ContainsKey(argumentName) || this.NamedArguments.ContainsKey(GetCliParameterName(argumentName));
        }

        public bool HasArgument(int offset)
        {
            return PositionalArguments.Count > offset;
        }

        public string[] GetArgument(string argumentName)
        {
            var result = new List<string>();
            if (this.NamedArguments.ContainsKey(argumentName))
            {
                result.AddRange(NamedArguments[argumentName]);
            }
            var argumentVariant = GetCliParameterName(argumentName);
            if (this.NamedArguments.ContainsKey(argumentVariant))
            {
                result.AddRange(NamedArguments[argumentVariant]);
            }
            return result.ToArray();
        }

        public string GetArgument(int offset)
        {
            return HasArgument(offset) ? PositionalArguments[offset] : string.Empty;
        }

        public static string GetCliParameterName(string input)
        {
            var parts = Regex.Split(input, @"(?<!^)(?=[A-Z])");
            return string.Join("-", parts.Select(p => p.ToLower()));
        }
    }
}